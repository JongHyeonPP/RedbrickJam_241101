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

    // PushObject ����
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

        // �� ��ġ�� ���� Transform�� �����Ͽ� gridPositions �迭�� ����
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // �� ��ġ�� �����Ͽ� ���� ��ġ�� ���������� gridPositions �迭 �ε����� �״��
                Vector3 localPosition = new Vector3((i - halfSize) * pushTerm, 0, (j - halfSize) * pushTerm);
                GameObject positionObject = new GameObject($"Position_{i}_{j}");
                positionObject.transform.position = placedParent.position + localPosition;
                positionObject.transform.parent = placedParent;

                // ������ ��ġ�� �����Ͽ� gridPositions�� ����
                gridPositions[5 - i, j] = positionObject.transform;
            }
        }

        // PushObjects�� initialPositions�� ���� ��ġ�� ��ġ
        for (int i = 0; i < initialPositions.Length; i++)
        {
            int row = initialPositions[i].Item1;
            int col = initialPositions[i].Item2;

            GameObject newPushObject = Instantiate(pushObjectPrefab, gridPositions[row, col].position, Quaternion.identity, placedParent);

            PushObject pushObject = newPushObject.GetComponent<PushObject>();
            pushObject.currentRow = row; // ���� �� ���� ����
            pushObject.currentColumn = col; // ���� �� ���� ����

            pushObjects.Add(pushObject);
            isPlaced[row, col] = true; // ���� ��ġ�� isPlaced ����
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

        // ���� ��ġ (0, 1)�������� ����� PushObject���� ã�� ����
        queue.Enqueue((0, 1));
        visited.Add((0, 1));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int row = current.Item1;
            int col = current.Item2;

            // ���� ��ġ�� PushObject�� �ִ��� Ȯ��
            PushObject pushObject = GetPushObjectAtPosition(row, col);
            if (pushObject != null)
            {
                connectedObjects.Add(pushObject);
            }

            // ������ ��ġ�� �˻��ϸ�, ���� �湮���� ���� Ÿ�� ��ġ�� �ִ��� Ȯ��
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
        // ���� row�� col�� ��ġ�� PushObject�� ã�� ��ȯ�մϴ�.
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
        // �����¿� �� ������ ��ȯ�մϴ�.
        return new List<(int, int)>
        {
            (row - 1, col), // ��
            (row + 1, col), // �Ʒ�
            (row, col - 1), // ����
            (row, col + 1)  // ������
        };
    }

    private bool IsValidPosition(int row, int col)
    {
        // row�� col�� ��ȿ�� �׸��� ���� ���� �ִ��� Ȯ��
        return row >= 0 && row < gridPositions.GetLength(0) && col >= 0 && col < gridPositions.GetLength(1);
    }
}
