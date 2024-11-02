using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    float distanceThreshold = 3f;
    float offsetDistance = -1f;
    [SerializeField] Animator animator;
    [SerializeField] Transform player_0;
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
            Debug.Log("밀기 중단, isPush를 비활성화했습니다.");
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

        // Save the original local position of player_0
        Vector3 originalLocalPosition = player_0.localPosition;

        // Define the offset to maintain the initial distance to the PushObject
        Vector3 offsetFromPushObject = transform.position - currentPushObject.transform.position;

        // Determine push direction based on character's position relative to the PushObject
        Vector3 directionToPushObject = (currentPushObject.transform.position - transform.position).normalized;

        if (Mathf.Abs(directionToPushObject.x) > Mathf.Abs(directionToPushObject.z))
        {
            newRow += (directionToPushObject.x > 0) ? 1 : -1;
        }
        else
        {
            newCol += (directionToPushObject.z > 0) ? 1 : -1;
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
                // Move the PushObject
                currentPushObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);

                // Maintain the offset distance during the push movement
                transform.position = currentPushObject.transform.position + offsetFromPushObject;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentPushObject.transform.position = targetPosition;
            currentPushObject.currentRow = newRow;
            currentPushObject.currentColumn = newCol;

            // Set character position to maintain offset after movement
            transform.position = currentPushObject.transform.position + offsetFromPushObject;
        }

        // Restore player_0 to its original local position
        player_0.localPosition = originalLocalPosition;
        animator.SetFloat("speed", 0f);

        moveController.canMove = true;
        isMovingWithPushObject = false;
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
