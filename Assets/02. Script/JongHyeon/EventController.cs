using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    float distanceThreshold = 2f;
    float offsetDistance = 0f;
    [SerializeField] Animator animator;
    public bool isPushing { get; private set; } = false;

    private PushObject currentPushObject;
    private bool isMovingWithPushObject = false;

    MoveController moveController;

    private void Start()
    {
        moveController = GetComponent<MoveController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentPushObject = CheckForPushObjectInFront();

            if (currentPushObject != null)
            {
                Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;
                Vector3 targetPosition = CalculateTargetPosition(currentPushObject.transform, directionToPushObject);
                transform.position = targetPosition;

                transform.LookAt(new Vector3(currentPushObject.transform.position.x, transform.position.y, currentPushObject.transform.position.z));

                animator.SetBool("isPush", true);
                isPushing = true;
            }
        }

        if (isPushing && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            animator.SetBool("isPush", false);
            isPushing = false;
            currentPushObject = null;
            Debug.Log("�б� �ߴ�, isPush�� ��Ȱ��ȭ�߽��ϴ�.");
        }

        if (isPushing && Input.GetKeyDown(KeyCode.W) && !isMovingWithPushObject)
        {
            StartCoroutine(PushObjectOneStep());
        }
    }

    private IEnumerator PushObjectOneStep()
    {
        isMovingWithPushObject = true;
        moveController.canMove = false;
        animator.SetFloat("speed", 0.5f);

        int newRow = currentPushObject.currentRow;
        int newCol = currentPushObject.currentColumn;

        // �� ������ ���� ĳ���Ϳ� PushObject�� ��� ��ġ�� ���� ����
        Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;

        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            newRow += (directionToPushObject.x > 0) ? 1 : -1; // ���� �ƴ� ���� ����
        }
        else
        {
            newCol += (directionToPushObject.z > 0) ? 1 : -1; // ���� �ƴ� ���� ����
        }

        // �迭 ������ ����� �ʵ��� �˻�
        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            // ��ǥ ��ġ ����
            Transform targetTransform = mainManager.gridPositions[newRow, newCol];
            Vector3 startPosition = currentPushObject.transform.position;
            Vector3 targetPosition = targetTransform.position;

            float moveDuration = 1f;
            float elapsedTime = 0f;

            mainManager.isPlaced[currentPushObject.currentRow, currentPushObject.currentColumn] = false;
            mainManager.isPlaced[newRow, newCol] = true;

            // ��ġ ������Ʈ �ִϸ��̼�
            while (elapsedTime < moveDuration)
            {
                currentPushObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                transform.position = CalculateTargetPosition(currentPushObject.transform, directionToPushObject);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentPushObject.transform.position = targetPosition;
            currentPushObject.currentRow = newRow;
            currentPushObject.currentColumn = newCol;
            transform.position = CalculateTargetPosition(currentPushObject.transform, directionToPushObject);
        }

        animator.SetFloat("speed", 0f);
        moveController.canMove = true;
        isMovingWithPushObject = false;
    }


    private PushObject CheckForPushObjectInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceThreshold))
        {
            if (hit.collider.CompareTag("PushObject"))
            {
                return hit.collider.GetComponent<PushObject>();
            }
        }
        return null;
    }

    private Vector3 CalculateTargetPosition(Transform pushObjectTransform, Vector3 directionToPushObject)
    {
        Vector3 pushObjectForward = pushObjectTransform.forward;
        Vector3 pushObjectRight = pushObjectTransform.right;

        float forwardDot = Vector3.Dot(pushObjectForward, directionToPushObject);
        float rightDot = Vector3.Dot(pushObjectRight, directionToPushObject);

        Vector3 targetPosition = pushObjectTransform.position;

        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            targetPosition += (forwardDot > 0 ? -pushObjectForward : pushObjectForward) * (distanceThreshold + offsetDistance);
        }
        else
        {
            targetPosition += (rightDot > 0 ? -pushObjectRight : pushObjectRight) * (distanceThreshold + offsetDistance);
        }

        targetPosition.y = transform.position.y;
        return targetPosition;
    }
}
