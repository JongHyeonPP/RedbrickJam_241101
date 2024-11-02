using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public bool isTileClear = false;
    public bool isPresent { get; private set; } = true;
    private bool isOnCooldown = false;
    CameraController cameraController;

    // UI
    public GameObject presentButton;
    public GameObject pastButton;
    public GameObject keyCodeE;

    // PushObject 설정
    [SerializeField] Transform presentPlacedParent;
    [SerializeField] Transform pastPlacedParent;
    [SerializeField] GameObject pushObjectPrefab;
    [SerializeField] GameObject firstOrLastPrefab;
    List<PushObject> presentObjects = new();
    public bool[,] isPlaced = new bool[6, 6];
    private float pushTerm = 7.5f;

    [SerializeField] GameObject pastParent;
    [SerializeField] GameObject presentParent;
    [SerializeField] Material pushObjectMat;
    [SerializeField] Material selectedMat;
    private readonly (int, int)[] initialPositions = new (int, int)[]
    {
        (0, 1), (1, 1), (1, 3), (2, 0), (2, 2),
        (2, 4), (3, 1), (3, 3), (3, 4), (3, 5),
        (4, 0), (4, 2), (4, 5), (5, 1), (5, 4)
    };
    private readonly (int, int)[] targetPositions = new (int, int)[]
    {
        (0, 0), (0, 1), (1, 0), (2, 0), (2, 1),
        (3, 1), (3, 3), (3, 4), (3, 5), (4, 1),
        (4, 3), (4, 5), (5, 1), (5, 2), (5, 3)
    };
    public readonly (int, int)[] firstLastPositions = new (int, int)[]
    {
        (0, 1), (4, 5)
    };
    
    public Transform[,] gridPositions = new Transform[6, 6];

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {
        pastButton.SetActive(true);
        pastButton.SetActive(false);
        presentButton.SetActive(false);

        // Initialize present objects using initialPositions
        InitializeGridPositions(presentPlacedParent);
        InitializePushObjects(presentPlacedParent, initialPositions);

        // Initialize past objects using targetPositions
        InitializeGridPositions(pastPlacedParent);
        InitializePushObjects(pastPlacedParent, targetPositions);

        pastParent.SetActive(false);
        presentParent.SetActive(true);
        CheckConnectedPushObjects();
    }

    private void InitializeGridPositions(Transform parent)
    {
        int size = 6;
        float halfSize = (size - 1) / 2.0f;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3 localPosition = new Vector3((i - halfSize) * pushTerm, 0, (j - halfSize) * pushTerm);
                GameObject positionObject = new GameObject($"Position_{i}_{j}");
                positionObject.transform.SetParent(parent, false);
                positionObject.transform.localPosition = localPosition;
                gridPositions[5 - i, j] = positionObject.transform;
            }
        }
    }

    private void InitializePushObjects(Transform parent, (int, int)[] positions)
    {
        bool isPastParent = parent == pastPlacedParent;

        for (int i = 0; i < positions.Length; i++)
        {
            int row = positions[i].Item1;
            int col = positions[i].Item2;

            GameObject prefabToInstantiate = firstLastPositions.Contains((row, col)) ? firstOrLastPrefab : pushObjectPrefab;
            bool isFirstOrLast = prefabToInstantiate == firstOrLastPrefab;

            Vector3 localPositionWithOriginalY = new Vector3(
                gridPositions[row, col].localPosition.x,
                prefabToInstantiate.transform.localPosition.y,
                gridPositions[row, col].localPosition.z
            );

            Quaternion localRotation = prefabToInstantiate.transform.localRotation;

            GameObject newPushObject = Instantiate(prefabToInstantiate, parent);
            newPushObject.transform.localPosition = localPositionWithOriginalY;
            newPushObject.transform.localRotation = localRotation;

            PushObject pushObject = newPushObject.GetComponent<PushObject>();
            pushObject.currentRow = row;
            pushObject.currentColumn = col;
            pushObject.isFirstOrLast = isFirstOrLast;
            pushObject.isPast = isPastParent;  // Set isPast based on the parent

            if (!isPastParent)
            {
                presentObjects.Add(pushObject);
                isPlaced[row, col] = true;
            }
            if (!firstOrLastPrefab)
                newPushObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }

        public void GoPast()
    {
        if (isOnCooldown) return;
        isPresent = false;
        cameraController.PastVolumeOnOff(true);

        pastButton.SetActive(false);
        presentButton.SetActive(true);

        pastParent.SetActive(true);
        presentParent.SetActive(false);
        StartCoroutine(StartCooldown());
    }

    public void GoPresent()
    {
        if (isOnCooldown) return;
        isPresent = true;
        cameraController.PastVolumeOnOff(false);

        pastButton.SetActive(true);
        presentButton.SetActive(false);

        pastParent.SetActive(false);
        presentParent.SetActive(true);
        ResetPresentObjectsToStartPosition();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(2f);
        isOnCooldown = false;
    }

    public void CheckConnectedPushObjects()
    {
        List<PushObject> connectedObjects = new List<PushObject>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        Queue<(int, int)> queue = new Queue<(int, int)>();

        // 모든 first/last 블록 중 targetPositions에 속하는 것을 시작점으로 큐에 추가
        foreach (var pushObject in presentObjects.Where(item => item.isFirstOrLast))
        {
            if (pushObject.isFirstOrLast && targetPositions.Contains((pushObject.currentRow, pushObject.currentColumn)))
            {
                var startPosition = (pushObject.currentRow, pushObject.currentColumn);
                queue.Enqueue(startPosition);
                visited.Add(startPosition);
            }
        }

        // BFS 탐색 시작
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int row = current.Item1;
            int col = current.Item2;

            // 현재 위치의 PushObject 가져오기
            PushObject pushObject = GetPushObjectAtPosition(row, col);

            // 시작점(firstOrLast인 오브젝트)은 connectedObjects에 추가하지 않음
            if (pushObject != null && !pushObject.isFirstOrLast)
            {
                connectedObjects.Add(pushObject);
            }

            // 상하좌우 인접 위치 탐색
            foreach (var direction in GetAdjacentPositions(row, col))
            {
                int newRow = direction.Item1;
                int newCol = direction.Item2;

                // 인접 위치가 유효하고, targetPositions에 포함되며, 아직 방문하지 않았고, isPlaced가 true인 경우에만 큐에 추가
                if (IsValidPosition(newRow, newCol) &&
                    targetPositions.Contains((newRow, newCol)) &&
                    !visited.Contains((newRow, newCol)) &&
                    isPlaced[newRow, newCol])
                {
                    queue.Enqueue((newRow, newCol));
                    visited.Add((newRow, newCol));
                }
            }
        }

        // 방문한 객체는 selectedMat을, 방문하지 않은 객체는 pushObjectMat을 할당
        foreach (var pushObject in presentObjects)
        {
            if (pushObject.isFirstOrLast)
                continue;
            if (visited.Contains((pushObject.currentRow, pushObject.currentColumn)))
            {
                pushObject.renderer.material = selectedMat;
            }
            else
            {
                pushObject.renderer.material = pushObjectMat;
            }
        }

        // 모든 firstOrLast가 아닌 presentObjects가 탐색된 경우 로그 출력
        int nonFirstOrLastCount = presentObjects.Count(po => !po.isFirstOrLast);
        if (connectedObjects.Count == nonFirstOrLastCount)
        {
            isTileClear = true;
        }
    }


    private PushObject GetPushObjectAtPosition(int row, int col)
    {
        foreach (var pushObject in presentObjects)
        {
            if (pushObject.currentRow == row && pushObject.currentColumn == col)
            {
                return pushObject;
            }
        }
        return null;
    }

    private List<(int, int)> GetAdjacentPositions(int row, int col)
    {
        return new List<(int, int)>
        {
            (row - 1, col),
            (row + 1, col),
            (row, col - 1),
            (row, col + 1)
        };
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < gridPositions.GetLength(0) && col >= 0 && col < gridPositions.GetLength(1);
    }
    public void ResetPresentObjectsToStartPosition()
    {
        isTileClear = false;
        for (int row = 0; row < isPlaced.GetLength(0); row++)
        {
            for (int col = 0; col < isPlaced.GetLength(1); col++)
            {
                isPlaced[row, col] = false;
            }
        }

        for (int i = 0; i < presentObjects.Count; i++)
        {
            PushObject pushObject = presentObjects[i];

            // Retrieve the initial position for this object from the initialPositions array
            int initialRow = initialPositions[i].Item1;
            int initialCol = initialPositions[i].Item2;

            // Update the pushObject's row and column to match its initial position
            pushObject.currentRow = initialRow;
            pushObject.currentColumn = initialCol;

            // Set the corresponding isPlaced position to true
            isPlaced[initialRow, initialCol] = true;

            // Calculate the reset local position based on the initial row and column
            Vector3 resetPosition = new Vector3(
                gridPositions[initialRow, initialCol].localPosition.x,
                pushObject.transform.localPosition.y, // Preserve the Y position
                gridPositions[initialRow, initialCol].localPosition.z
            );

            // Move the object directly to its initial position
            pushObject.transform.localPosition = resetPosition;
            CheckConnectedPushObjects();
        }
    }


}
