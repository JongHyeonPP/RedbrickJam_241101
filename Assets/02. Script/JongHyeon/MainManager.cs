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
    [SerializeField] GameObject pushObjectPrefab; // PushObject 프리팹 참조
    List<Transform> pushObjects = new();
    bool[,] isPlaced = new bool[6, 6];
    private float pushTerm = 1f;

    private readonly (int, int)[] initialPositions = new (int, int)[]
    {
        (0, 1), (1, 1), (1, 3), (2, 0), (2, 2),
        (2, 4), (3, 1), (3, 3), (3, 4), (3, 5),
        (4, 0), (4, 2), (4, 5), (5, 1), (5, 4)
    };

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

        // PushObjects를 initialPositions에 따라 위치에 배치
        for (int i = 0; i < initialPositions.Length; i++)
        {
            int row = initialPositions[i].Item1;
            int col = initialPositions[i].Item2;

            // PushObject 프리팹을 생성하여 위치 설정
            GameObject newPushObject = Instantiate(pushObjectPrefab, placedParent);
            Vector3 localPosition = new Vector3((row - halfSize) * pushTerm, 0, (col - halfSize) * pushTerm);
            newPushObject.transform.position = placedParent.position + localPosition;

            // 생성한 오브젝트의 Transform을 pushObjects 리스트에 추가
            pushObjects.Add(newPushObject.transform);

            // isPlaced 배열에 해당 위치가 true로 설정되었음을 표시
            isPlaced[row, col] = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (initialPositions == null) return;

        Gizmos.color = Color.green;

        int size = 6;
        float halfSize = (size - 1) / 2.0f;

        foreach (var position in initialPositions)
        {
            int row = position.Item1;
            int col = position.Item2;

            Vector3 localPosition = new Vector3((row - halfSize) * pushTerm, 0, (col - halfSize) * pushTerm);
             Gizmos.DrawSphere(placedParent.position + localPosition, 0.1f);
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
