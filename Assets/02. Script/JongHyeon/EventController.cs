using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    float distanceThreshold = 1f; // PushObject���� �Ÿ� �Ӱ谪
    float offsetDistance = 0.1f; // �߰� ����
    [SerializeField] Animator animator; // Animator ����
    private bool isPushing = false; // �б� ���� Ȯ��

    private void Update()
    {
        // ���鿡 ���� �Ÿ� �̳��� �ִ� PushObject�� Transform�� ������
        Transform pushObjectTransform = CheckForPushObjectInFront();

        // E Ű�� ������ �� �б� ����
        if (pushObjectTransform != null && Input.GetKeyDown(KeyCode.E))
        {
            // ���� y ��ġ�� �����ϸ鼭 x, z�� �̵��ϵ��� ��ġ ���
            Vector3 directionToPushObject = (pushObjectTransform.position - transform.position).normalized;
            Vector3 targetPosition = CalculateTargetPosition(pushObjectTransform, directionToPushObject);
            transform.position = targetPosition;

            // PushObject�� �ٶ󺸵��� ȸ�� ����
            transform.LookAt(new Vector3(pushObjectTransform.position.x, transform.position.y, pushObjectTransform.position.z));

            // Animator�� isPush �Ķ���͸� true�� ����
            animator.SetBool("isPush", true);
            isPushing = true;

            Debug.Log("PushObject�� �ٷ� �� ��ġ�� �̵��ϰ� isPush�� Ȱ��ȭ�߽��ϴ�.");
        }

        // S, A, D Ű�� ������ �� �б� �ߴ�
        if (isPushing && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            animator.SetBool("isPush", false);
            isPushing = false;

            Debug.Log("�б� �ߴ�, isPush�� ��Ȱ��ȭ�߽��ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "TimeZone":
                if (mainManager.isPresent)
                {
                    mainManager.pastButton.SetActive(true);
                }
                else
                {
                    mainManager.presentButton.SetActive(true);
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "TimeZone":
                mainManager.pastButton.SetActive(false);
                mainManager.presentButton.SetActive(false);
                break;
        }
    }

    // PushObject�� ������ ���� �Ÿ� ���� ������ Transform�� ��ȯ�ϴ� �Լ�
    private Transform CheckForPushObjectInFront()
    {
        // Raycast�� ����� �������� ���� �Ÿ� ���� PushObject üũ
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceThreshold))
        {
            // �浹�� ������Ʈ�� PushObject �±׸� ������ �ִ��� Ȯ��
            if (hit.collider.CompareTag("PushObject"))
            {
                return hit.collider.transform;
            }
        }
        return null; // PushObject�� ������ null ��ȯ
    }

    // PushObject ���⿡ ���� ĳ���� ��ġ ����
    private Vector3 CalculateTargetPosition(Transform pushObjectTransform, Vector3 directionToPushObject)
    {
        Vector3 pushObjectForward = pushObjectTransform.forward;
        Vector3 pushObjectRight = pushObjectTransform.right;

        float forwardDot = Vector3.Dot(pushObjectForward, directionToPushObject);
        float rightDot = Vector3.Dot(pushObjectRight, directionToPushObject);

        Vector3 targetPosition = pushObjectTransform.position;

        // ���� ����� �������� ��ġ�� ����
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            if (forwardDot > 0) // PushObject�� ���ʿ� ���� ��
            {
                targetPosition -= pushObjectForward * (distanceThreshold + offsetDistance);
            }
            else // PushObject�� ���ʿ� ���� ��
            {
                targetPosition += pushObjectForward * (distanceThreshold + offsetDistance);
            }
        }
        else
        {
            if (rightDot > 0) // PushObject�� ���ʿ� ���� ��
            {
                targetPosition -= pushObjectRight * (distanceThreshold + offsetDistance);
            }
            else // PushObject�� �����ʿ� ���� ��
            {
                targetPosition += pushObjectRight * (distanceThreshold + offsetDistance);
            }
        }

        // y ��ġ ����
        targetPosition.y = transform.position.y;
        return targetPosition;
    }
}
