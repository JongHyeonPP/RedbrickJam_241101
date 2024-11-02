using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    float distanceThreshold = 3f;
    float offsetDistance = -1f;
    [SerializeField] Animator animator;

    private PushObject currentPushObject;
    private bool isMovingWithPushObject = false;
    private Quaternion targetRotation; // 고정할 회전값

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

                // 오브젝트를 바라보는 방향으로 회전 설정
                transform.LookAt(new Vector3(currentPushObject.transform.position.x, transform.position.y, currentPushObject.transform.position.z));
                targetRotation = transform.rotation; // 현재 회전을 고정
                animator.SetBool("isPush", true);
            }
        }

        if (currentPushObject != null && Input.GetKey(KeyCode.W) && !isMovingWithPushObject)
        {
            StartCoroutine(PushObjectOneStep());
        }

        // isMovingWithPushObject가 false일 때만 ASD 키로 밀기 해제 가능
        if(currentPushObject)
        if (!isMovingWithPushObject && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            animator.SetBool("isPush", false);
            currentPushObject = null;
        }

        // 고정된 회전을 유지
        if (currentPushObject != null)
        {
            transform.rotation = targetRotation;
        }
    }

    private IEnumerator PushObjectOneStep()
    {
        isMovingWithPushObject = true;
        moveController.canMove = false;
        animator.SetFloat("speed", 0.5f);

        int newRow = currentPushObject.currentRow;
        int newCol = currentPushObject.currentColumn;

        Vector3 offsetFromPushObject = transform.position - currentPushObject.transform.position;
        Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;

        // 방향에 따라 행 또는 열을 조정 (기존 방식 유지)
        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            // x 방향으로 더 많이 기울어져 있는 경우: 왼쪽 또는 오른쪽으로 이동
            if (directionToPushObject.x > 0)
            {
                // 오른쪽으로 이동
                newRow -= 1;
            }
            else
            {
                // 왼쪽으로 이동
                newRow += 1;
            }
        }
        else
        {
            // z 방향으로 더 많이 기울어져 있는 경우: 앞쪽 또는 뒤쪽으로 이동
            if (directionToPushObject.z > 0)
            {
                // 앞쪽으로 이동
                newCol += 1;
            }
            else
            {
                // 뒤쪽으로 이동
                newCol -= 1;
            }
        }

        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            Transform targetTransform = mainManager.gridPositions[newRow, newCol];
            Vector3 startPosition = currentPushObject.transform.position;
            Vector3 targetPosition = targetTransform.position;

            float moveDuration = 1f;
            float elapsedTime = 0f;

            mainManager.isPlaced[currentPushObject.currentRow, currentPushObject.currentColumn] = false;
            mainManager.isPlaced[newRow, newCol] = true;

            while (elapsedTime < moveDuration)
            {
                currentPushObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                transform.position = currentPushObject.transform.position + offsetFromPushObject;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentPushObject.transform.position = targetPosition;
            currentPushObject.currentRow = newRow;
            currentPushObject.currentColumn = newCol;

            transform.position = currentPushObject.transform.position + offsetFromPushObject;
        }

        animator.SetFloat("speed", 0f);
        moveController.canMove = true;
        isMovingWithPushObject = false;;
        var objects = mainManager.CheckConnectedPushObjects();
        foreach (var x in objects)
        {
            x.PrintRowColumn();
        }
    }



    private PushObject CheckForPushObjectInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceThreshold))
        {
            if (hit.collider.CompareTag("PushObject"))
            {
                return hit.collider.transform.parent.GetComponent<PushObject>();
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
