using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public bool isPresent { get; private set; } = true;
    private bool isOnCooldown = false;
    CameraController cameraController;

    // UI
    public GameObject presentButton;
    public GameObject pastButton;

    // PushObject 설정
    [SerializeField] Transform placedParent;
    [SerializeField] GameObject pushObjectPrefab;
    List<PushObject> pushObjects = new();
    public bool[,] isPlaced = new bool[6, 6];
    public float pushTerm = 4f;

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

    public Transform[,] gridPositions = new Transform[6, 6];

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {
        pastButton.SetActive(false);
        presentButton.SetActive(false);

        int size = 6;
        float halfSize = (size - 1) / 2.0f;

        // 각 위치에 대한 Transform을 생성하여 gridPositions 배열에 저장
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // 행 위치를 반전하여 실제 위치에 적용하지만 gridPositions 배열 인덱스는 그대로
                Vector3 localPosition = new Vector3((i - halfSize) * pushTerm, 0, (j - halfSize) * pushTerm);
                GameObject positionObject = new GameObject($"Position_{i}_{j}");
                positionObject.transform.position = placedParent.position + localPosition;
                positionObject.transform.parent = placedParent;

                // 반전된 위치를 적용하여 gridPositions에 저장
                gridPositions[5 - i, j] = positionObject.transform;
            }
        }

        // PushObjects를 initialPositions에 따라 위치에 배치
        for (int i = 0; i < initialPositions.Length; i++)
        {
            int row = initialPositions[i].Item1;
            int col = initialPositions[i].Item2;

            GameObject newPushObject = Instantiate(pushObjectPrefab, gridPositions[row, col].position, Quaternion.identity, placedParent);

            PushObject pushObject = newPushObject.GetComponent<PushObject>();
            pushObject.currentRow = row; // 원래 행 값을 유지
            pushObject.currentColumn = col; // 원래 열 값을 유지

            pushObjects.Add(pushObject);
            isPlaced[row, col] = true; // 원래 위치에 isPlaced 설정
        }
    }

    private void OnDrawGizmos()
    {
        if (gridPositions == null) return;

        Gizmos.color = Color.green;

        foreach (var position in initialPositions)
        {
            int row = position.Item1;
            int col = position.Item2;

            if (gridPositions[row, col] != null)
            {
                Gizmos.DrawSphere(gridPositions[row, col].position, 0.1f);
            }
        }
    }

    public void GoPast()
    {
        if (isOnCooldown) return;
        isPresent = false;
        cameraController.PastVolumeOnOff(true);
        pastButton.SetActive(false);
        presentButton.SetActive(true);

        StartCoroutine(StartCooldown());
    }

    public void GoPresent()
    {
        if (isOnCooldown) return;
        isPresent = true;
        cameraController.PastVolumeOnOff(false);
        pastButton.SetActive(true);
        presentButton.SetActive(false);

        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(2f);
        isOnCooldown = false;
    }
    public List<PushObject> CheckConnectedPushObjects()
    {
        List<PushObject> connectedObjects = new List<PushObject>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        Queue<(int, int)> queue = new Queue<(int, int)>();

        // 시작 위치 (0, 1)에서부터 연결된 PushObject들을 찾기 시작
        queue.Enqueue((0, 1));
        visited.Add((0, 1));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int row = current.Item1;
            int col = current.Item2;

            // 현재 위치에 PushObject가 있는지 확인
            PushObject pushObject = GetPushObjectAtPosition(row, col);
            if (pushObject != null)
            {
                connectedObjects.Add(pushObject);
            }

            // 인접한 위치를 검사하며, 아직 방문하지 않은 타겟 위치가 있는지 확인
            foreach (var direction in GetAdjacentPositions(row, col))
            {
                int newRow = direction.Item1;
                int newCol = direction.Item2;

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

        return connectedObjects;
    }

    private PushObject GetPushObjectAtPosition(int row, int col)
    {
        // 현재 row와 col에 위치한 PushObject를 찾아 반환합니다.
        foreach (var pushObject in pushObjects)
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
        // 상하좌우 네 방향을 반환합니다.
        return new List<(int, int)>
        {
            (row - 1, col), // 위
            (row + 1, col), // 아래
            (row, col - 1), // 왼쪽
            (row, col + 1)  // 오른쪽
        };
    }

    private bool IsValidPosition(int row, int col)
    {
        // row와 col이 유효한 그리드 범위 내에 있는지 확인
        return row >= 0 && row < gridPositions.GetLength(0) && col >= 0 && col < gridPositions.GetLength(1);
    }
}
