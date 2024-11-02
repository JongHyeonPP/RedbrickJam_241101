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
    private Quaternion targetRotation; // ������ ȸ����

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
                transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // y ����

                // ������Ʈ�� �ٶ󺸴� �������� ȸ�� ����
                transform.LookAt(new Vector3(currentPushObject.transform.position.x, transform.position.y, currentPushObject.transform.position.z));
                targetRotation = transform.rotation; // ���� ȸ���� ����
                animator.SetBool("isPush", true);
            }
        }

        if (currentPushObject != null && Input.GetKey(KeyCode.W) && !isMovingWithPushObject)
        {
            StartCoroutine(PushObjectOneStep());
        }

        // isMovingWithPushObject�� false�� ���� ASD Ű�� �б� ���� ����
        if (!isMovingWithPushObject)
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                animator.SetBool("isPush", false);
                currentPushObject = null;
                isMovingWithPushObject = false;
            }

        // ������ ȸ���� ����
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

        // y ��ǥ�� ������ ���·� ������ ���
        float originalY = transform.position.y; // �÷��̾��� ���� y ��ǥ
        Vector3 offsetFromPushObject = new Vector3(transform.position.x - currentPushObject.transform.position.x, 0, transform.position.z - currentPushObject.transform.position.z);
        Vector3 directionToPushObject = new Vector3(currentPushObject.transform.position.x - transform.position.x, 0, currentPushObject.transform.position.z - transform.position.z).normalized;

        rigidbody.isKinematic = true;

        // ���⿡ ���� �� �Ǵ� ���� ����
        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            if (directionToPushObject.x > 0)
            {
                newCol += 1; // ������ �̵�
            }
            else
            {
                newCol -= 1; // ���� �̵�
            }
        }
        else
        {
            if (directionToPushObject.z > 0)
            {
                newRow += 1; // ���� �̵�
            }
            else
            {
                newRow -= 1; // ���� �̵�
            }
        }

        if (newRow >= 0 && newRow < mainManager.gridPositions.GetLength(0) &&
            newCol >= 0 && newCol < mainManager.gridPositions.GetLength(1) &&
            !mainManager.isPlaced[newRow, newCol])
        {
            Transform targetTransform = mainManager.gridPositions[newRow, newCol];
            Vector3 startPosition = new Vector3(currentPushObject.transform.position.x, currentPushObject.transform.position.y, currentPushObject.transform.position.z); // ������Ʈ�� ���� y ����
            Vector3 targetPosition = new Vector3(targetTransform.position.x, currentPushObject.transform.position.y, targetTransform.position.z); // ������Ʈ�� ���� y ����

            float moveDuration = 1f;
            float elapsedTime = 0f;

            mainManager.isPlaced[currentPushObject.currentRow, currentPushObject.currentColumn] = false;
            mainManager.isPlaced[newRow, newCol] = true;

            while (elapsedTime < moveDuration)
            {
                // y ��ǥ�� ���� ��ġ�� ������ ���·� �̵�
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

        targetPosition.y = transform.position.y; // y ��ǥ ����
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
