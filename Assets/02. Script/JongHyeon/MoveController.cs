using UnityEngine;
using System.Collections;
using System;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckDistance = 0.1f;
    public float acceleration = 20f;
    public float airControlFactor = 0.5f;
    public float rotationSpeed = 10f;
    public float groundedLockTime = 1f;

    private Rigidbody rb;
    private bool isGrounded;
    public bool canMove = true;
    private Vector3 currentMoveDirection;
    private Animator animator;
    private EventController eventController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        eventController = GetComponent<EventController>();
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (isGrounded && !wasGrounded)
        {
            StartCoroutine(LockMovementAfterLanding());
        }

        if (canMove)
        {
            Move();
            Jump();
        }

        if (animator != null)
        {
            animator.SetBool("isGround", isGrounded);
        }
    }

    void Move()
    {
        bool isMoving = false;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (Camera.main != null && targetDirection != Vector3.zero)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            Quaternion rotation = Quaternion.LookRotation(cameraForward);
            targetDirection = rotation * targetDirection;

            isMoving = true;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float effectiveMoveSpeed = isGrounded ? moveSpeed : moveSpeed * airControlFactor;
        float effectiveAcceleration = isGrounded ? acceleration : acceleration * airControlFactor;

        currentMoveDirection = Vector3.MoveTowards(currentMoveDirection, targetDirection, effectiveAcceleration * Time.deltaTime);
        Vector3 movement = new Vector3(currentMoveDirection.x * effectiveMoveSpeed * Time.deltaTime, 0, currentMoveDirection.z * effectiveMoveSpeed * Time.deltaTime);

        transform.Translate(movement, Space.World);

        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);
            float currentSpeed = movement.magnitude / (moveSpeed * Time.deltaTime);
            animator.SetFloat("speed", Mathf.Clamp01(currentSpeed));
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    IEnumerator LockMovementAfterLanding()
    {
        canMove = false;
        yield return new WaitForSeconds(groundedLockTime);
        canMove = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
    public void MoveFromTo(Tuple<int, int> from, Tuple<int, int> to)
    {

    }
}
