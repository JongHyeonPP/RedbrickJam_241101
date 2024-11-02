using System.Collections;
using System.Collections.Generic;
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
                Vector3 localPosition = new Vector3((i - halfSize) * pushTerm, 0, (j - halfSize) * pushTerm);
                GameObject positionObject = new GameObject($"Position_{i}_{j}");
                positionObject.transform.position = placedParent.position + localPosition;
                positionObject.transform.parent = placedParent;

                gridPositions[i, j] = positionObject.transform;
            }
        }

        // PushObjects를 initialPositions에 따라 위치에 배치
        for (int i = 0; i < initialPositions.Length; i++)
        {
            int row = initialPositions[i].Item1;
            int col = initialPositions[i].Item2;

            GameObject newPushObject = Instantiate(pushObjectPrefab, gridPositions[row, col].position, Quaternion.identity, placedParent);

            PushObject pushObject = newPushObject.GetComponent<PushObject>();
            pushObject.currentRow = row;
            pushObject.currentColumn = col;

            pushObjects.Add(pushObject);
            isPlaced[row, col] = true;
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
}
