using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;             // 이동 속도
    public float jumpForce = 5f;             // 점프 힘
    public float groundCheckDistance = 0.1f; // 지면 체크를 위한 Ray 길이

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 lastMoveDirection; // 마지막 이동 방향

    private Animator animator;

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody가 없습니다. Rigidbody를 추가해주세요.");
        }

        // Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator가 없습니다. Animator를 추가해주세요.");
        }
    }

    void Update()
    {
        // 착지 여부 판정
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // 이동 여부를 초기화
        bool isMoving = false;

        // 착지 상태일 때만 WASD 입력을 감지하여 이동 방향 설정
        if (isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // 이동 벡터 계산 및 정규화
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            // 카메라의 y축 회전을 적용하여 이동 방향을 카메라가 보는 방향으로 변환
            if (Camera.main != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0; // y축 값은 제거하여 평면 이동을 유지
                Quaternion rotation = Quaternion.LookRotation(cameraForward);
                direction = rotation * direction;
            }

            // 이동 방향이 있을 때만 방향 저장 및 회전 적용
            if (direction != Vector3.zero)
            {
                lastMoveDirection = direction;
                transform.rotation = Quaternion.LookRotation(direction);
                isMoving = true; // 이동 중으로 설정
            }

            // Rigidbody를 사용하여 이동 적용
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 공중에 있을 때 마지막 이동 방향으로 이동 유지
            rb.MovePosition(transform.position + lastMoveDirection * moveSpeed * Time.deltaTime);
        }

        // 점프 입력 처리
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Animator의 isMove 파라미터 설정
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);
        }
    }

    void OnDrawGizmos()
    {
        // Gizmos로 지면 체크 Ray를 시각화합니다.
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
