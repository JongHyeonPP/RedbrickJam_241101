using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    public float distanceThreshold = 10f;
    public float offsetDistance = 5f;
    [SerializeField] Animator animator;

    private PushObject currentPushObject;
    private bool isMovingWithPushObject = false;
    private Quaternion targetRotation; // 고정할 회전값

    MoveController moveController;

    Rigidbody rigidbody;
    private bool isOnCooldown;
    [SerializeField] CameraController cameraController;

    private void Start()
    {
        moveController = GetComponent<MoveController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {;
        if (Input.GetKeyDown(KeyCode.E)&&!mainManager.isTileClear)
        {
            currentPushObject = CheckForPushObjectInFront();

            if (currentPushObject != null)
            {
                Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;
                Vector3 targetPosition = CalculateTargetPosition(currentPushObject.transform, directionToPushObject);
                transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // y 고정

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
        if (!isMovingWithPushObject)
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                animator.SetBool("isPush", false);
                currentPushObject = null;
                isMovingWithPushObject = false;
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

        // y 좌표를 고정한 상태로 오프셋 계산
        float originalY = transform.position.y; // 플레이어의 원래 y 좌표
        Vector3 offsetFromPushObject = new Vector3(transform.position.x - currentPushObject.transform.position.x, 0, transform.position.z - currentPushObject.transform.position.z);
        Vector3 directionToPushObject = new Vector3(currentPushObject.transform.position.x - transform.position.x, 0, currentPushObject.transform.position.z - transform.position.z).normalized;

        rigidbody.isKinematic = true;

        // 방향에 따라 행 또는 열을 조정
        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            if (directionToPushObject.x > 0)
            {
                newCol += 1; // 오른쪽 이동
            }
            else
            {
                newCol -= 1; // 왼쪽 이동
            }
        }
        else
        {
            if (directionToPushObject.z > 0)
            {
                newRow += 1; // 앞쪽 이동
            }
            else
            {
                newRow -= 1; // 뒤쪽 이동
            }
        }

        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            Transform targetTransform = mainManager.gridPositions[newRow, newCol];
            Vector3 startPosition = new Vector3(currentPushObject.transform.position.x, currentPushObject.transform.position.y, currentPushObject.transform.position.z); // 오브젝트의 원래 y 고정
            Vector3 targetPosition = new Vector3(targetTransform.position.x, currentPushObject.transform.position.y, targetTransform.position.z); // 오브젝트의 원래 y 고정

            float moveDuration = 1f;
            float elapsedTime = 0f;

            mainManager.isPlaced[currentPushObject.currentRow, currentPushObject.currentColumn] = false;
            mainManager.isPlaced[newRow, newCol] = true;

            while (elapsedTime < moveDuration)
            {
                // y 좌표를 원래 위치로 고정한 상태로 이동
                currentPushObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                transform.position = new Vector3(currentPushObject.transform.position.x, originalY, currentPushObject.transform.position.z) + offsetFromPushObject;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentPushObject.transform.position = targetPosition;
            currentPushObject.currentRow = newRow;
            currentPushObject.currentColumn = newCol;

            transform.position = new Vector3(currentPushObject.transform.position.x, originalY, currentPushObject.transform.position.z) + offsetFromPushObject;
            mainManager.CheckConnectedPushObjects();
        }

        rigidbody.isKinematic = false;

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
                PushObject pushObject = hit.collider.transform.GetComponent<PushObject>();
                if (!pushObject.isFirstOrLast&&!pushObject.isPast)
                return pushObject;
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

        targetPosition.y = transform.position.y; // y 좌표 고정
        return targetPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "TimeZone":
                if (mainManager.isPresent)
                {
                    mainManager.pastButton.SetActive(true);
                    mainManager.presentButton.SetActive(false);
                }
                else
                {
                    mainManager.pastButton.SetActive(false);
                    mainManager.presentButton.SetActive(true);
                }
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        mainManager.pastButton.SetActive(false);
        mainManager.presentButton.SetActive(false);
    }
}
