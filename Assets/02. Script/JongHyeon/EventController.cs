using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    public float distanceThreshold = 10f;
    public float offsetDistance = 5.5f;
    [SerializeField] Animator animator;

    private PushObject currentPushObject;
    private bool isMovingWithPushObject = false;
    private bool isAttachedToPushObject = false;
    private bool isKeycodeEDisplayed = false; // E 키 표시 상태
    private Quaternion targetRotation;

    MoveController moveController;
    Rigidbody rb;
    GameObject keyCodeE;

    private void Start()
    {
        moveController = GetComponent<MoveController>();
        rb = GetComponent<Rigidbody>();
        keyCodeE = mainManager.keyCodeE;
    }

    private void Update()
    {
        // 플레이어 앞에 있는 밀 수 있는 오브젝트를 확인하고 표시
        currentPushObject = CheckForPushObjectInFront();
        if (currentPushObject != null && CanPushObject(currentPushObject))
        {
            if (!isKeycodeEDisplayed)
                ToggleKeyCodeE(true);

            if (Input.GetKeyDown(KeyCode.E) && !mainManager.isTileClear)
            {
                AttachToPushObject();
            }
        }
        else
        {
            if (isKeycodeEDisplayed)
                ToggleKeyCodeE(false);
        }

        if (isAttachedToPushObject && Input.GetKey(KeyCode.W) && !isMovingWithPushObject)
        {
            StartCoroutine(PushObjectOneStep());
        }

        if (!isMovingWithPushObject && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            DetachFromPushObject();
        }

        if (isAttachedToPushObject)
        {
            transform.rotation = targetRotation; // 유지 회전
        }
    }
    private bool CanPushObject(PushObject pushObject)
    {
        Vector3 directionToPushObject = (pushObject.transform.position - transform.position).normalized;
        int newRow = pushObject.currentRow;
        int newCol = pushObject.currentColumn;

        // 방향에 따라 다음 위치를 계산
        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            newCol += directionToPushObject.x > 0 ? 1 : -1;
        }
        else
        {
            newRow += directionToPushObject.z > 0 ? 1 : -1;
        }

        // 다음 위치가 그리드 안에 있고 비어 있는지 확인
        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            return true;
        }
        return false;
    }

    private void AttachToPushObject()
    {
        Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;
        Vector3 targetPosition = CalculateTargetPosition(currentPushObject.transform, directionToPushObject);
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // y 고정

        // Set rotation to face the object
        transform.LookAt(new Vector3(currentPushObject.transform.position.x, transform.position.y, currentPushObject.transform.position.z));
        targetRotation = transform.rotation;
        animator.SetBool("isPush", true);

        isAttachedToPushObject = true;
    }

    private void DetachFromPushObject()
    {
        animator.SetBool("isPush", false);
        currentPushObject = null;
        isMovingWithPushObject = false;
        isAttachedToPushObject = false;
    }

    private void ToggleKeyCodeE(bool state)
    {
        keyCodeE.SetActive(state);
        isKeycodeEDisplayed = state;
    }

    private IEnumerator PushObjectOneStep()
    {
        isMovingWithPushObject = true;
        moveController.canMove = false;
        animator.SetFloat("speed", 0.5f);

        int newRow = currentPushObject.currentRow;
        int newCol = currentPushObject.currentColumn;

        float originalY = transform.position.y;
        Vector3 offsetFromPushObject = new Vector3(transform.position.x - currentPushObject.transform.position.x, 0, transform.position.z - currentPushObject.transform.position.z);
        Vector3 directionToPushObject = new Vector3(currentPushObject.transform.position.x - transform.position.x, 0, currentPushObject.transform.position.z - transform.position.z).normalized;

        rb.isKinematic = true;

        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            newCol += directionToPushObject.x > 0 ? 1 : -1;
        }
        else
        {
            newRow += directionToPushObject.z > 0 ? 1 : -1;
        }

        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            Transform targetTransform = mainManager.gridPositions[newRow, newCol];
            Vector3 startPosition = currentPushObject.transform.position;
            Vector3 targetPosition = new Vector3(targetTransform.position.x, currentPushObject.transform.position.y, targetTransform.position.z);

            float moveDuration = 1f;
            float elapsedTime = 0f;

            mainManager.isPlaced[currentPushObject.currentRow, currentPushObject.currentColumn] = false;
            mainManager.isPlaced[newRow, newCol] = true;

            while (elapsedTime < moveDuration)
            {
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

        rb.isKinematic = false;

        animator.SetFloat("speed", 0f);
        moveController.canMove = true;
        isMovingWithPushObject = false;
    }

    private PushObject CheckForPushObjectInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceThreshold * 1.2f))
        {
            if (hit.collider.CompareTag("PushObject"))
            {
                PushObject pushObject = hit.collider.transform.GetComponent<PushObject>();
                if (!pushObject.isFirstOrLast && !pushObject.isPast)
                {
                    return pushObject;
                }
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
        switch (other.tag)
        {
            case "TimeZone":
                    mainManager.presentButton.SetActive(false);
                    mainManager.pastButton.SetActive(false);
                break;
        }
    }
}
