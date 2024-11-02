using UnityEngine;

public class PushObject : MonoBehaviour
{
    public int initialRow;
    public int initialColumn;
    private bool isPlayerInRange = false; // �÷��̾ ���� ���� �ִ��� Ȯ���ϴ� ���� ����
    private Collider playerCollider; // ���� ���� �÷��̾� Collider
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // ���� ���� ���� ���� �Է� ����
        {
            TryPush(playerCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerCollider = other; // ���� ���� ���� �÷��̾��� Collider ����
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerCollider = null; // �÷��̾ ������ ����� �� Collider �ʱ�ȭ
        }
    }

    private void TryPush(Collider collision)
    {
        Vector3 playerPosition = collision.transform.position;
        Vector3 directionToPlayer = (transform.position - playerPosition).normalized; // ��ü���� �÷��̾�� ���� ���� ����
        Vector3 forward = transform.forward; // ��ü�� �� ���� ����
        Vector3 right = transform.right; // ��ü�� ������ ���� ����

        float forwardDot = Vector3.Dot(forward, directionToPlayer);
        float rightDot = Vector3.Dot(right, directionToPlayer);

        Vector3 pushDirection = Vector3.zero;

        // Dot Product�� ������ �Ǻ�
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            // ���� �Ǵ� ���ʿ��� �б�
            if (forwardDot > 0)
            {
                Debug.Log("�÷��̾ ��ü�� ���ʿ� �ֽ��ϴ�.");
                pushDirection = -forward; // �������� �̴� ����
            }
            else
            {
                Debug.Log("�÷��̾ ��ü�� ���ʿ� �ֽ��ϴ�.");
                pushDirection = forward; // �������� �̴� ����
            }
        }
        else
        {
            // ���� �Ǵ� �����ʿ��� �б�
            if (rightDot > 0)
            {
                Debug.Log("�÷��̾ ��ü�� ���ʿ� �ֽ��ϴ�.");
                pushDirection = -right; // �������� �̴� ����
            }
            else
            {
                Debug.Log("�÷��̾ ��ü�� �����ʿ� �ֽ��ϴ�.");
                pushDirection = right; // ���������� �̴� ����
            }
        }
    }
}
