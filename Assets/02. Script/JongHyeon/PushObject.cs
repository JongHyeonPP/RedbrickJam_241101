using UnityEngine;

public class PushObject : MonoBehaviour
{
    public int initialRow;
    public int initialColumn;
    private bool isPlayerInRange = false; // 플레이어가 범위 내에 있는지 확인하는 상태 변수
    private Collider playerCollider; // 범위 내의 플레이어 Collider
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // 범위 내에 있을 때만 입력 받음
        {
            TryPush(playerCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerCollider = other; // 범위 내에 들어온 플레이어의 Collider 저장
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerCollider = null; // 플레이어가 범위를 벗어났을 때 Collider 초기화
        }
    }

    private void TryPush(Collider collision)
    {
        Vector3 playerPosition = collision.transform.position;
        Vector3 directionToPlayer = (transform.position - playerPosition).normalized; // 물체에서 플레이어로 가는 방향 벡터
        Vector3 forward = transform.forward; // 물체의 앞 방향 벡터
        Vector3 right = transform.right; // 물체의 오른쪽 방향 벡터

        float forwardDot = Vector3.Dot(forward, directionToPlayer);
        float rightDot = Vector3.Dot(right, directionToPlayer);

        Vector3 pushDirection = Vector3.zero;

        // Dot Product로 방향을 판별
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            // 앞쪽 또는 뒤쪽에서 밀기
            if (forwardDot > 0)
            {
                Debug.Log("플레이어가 물체의 뒤쪽에 있습니다.");
                pushDirection = -forward; // 뒤쪽으로 미는 방향
            }
            else
            {
                Debug.Log("플레이어가 물체의 앞쪽에 있습니다.");
                pushDirection = forward; // 앞쪽으로 미는 방향
            }
        }
        else
        {
            // 왼쪽 또는 오른쪽에서 밀기
            if (rightDot > 0)
            {
                Debug.Log("플레이어가 물체의 왼쪽에 있습니다.");
                pushDirection = -right; // 왼쪽으로 미는 방향
            }
            else
            {
                Debug.Log("플레이어가 물체의 오른쪽에 있습니다.");
                pushDirection = right; // 오른쪽으로 미는 방향
            }
        }
    }
}
