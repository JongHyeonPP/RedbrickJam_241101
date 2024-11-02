using UnityEngine;

public class Stage3Trigger : MonoBehaviour
{
    CameraController cameraController;
    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        
        if (other.CompareTag("Player"))
        {
            Vector3 triggerForward = transform.forward; // Ʈ������ ���� ����
            Vector3 playerToTrigger = (transform.position - other.transform.position).normalized; // �÷��̾�� Ʈ���ŷ��� ����

            // ������ ���� ���� �Ǻ�
            float dotProduct = Vector3.Dot(triggerForward, playerToTrigger);

            if (dotProduct > 0)
            {
                Debug.Log("�÷��̾ Ʈ������ �տ��� ��ҽ��ϴ�.");
                cameraController.ToggleView(true);
            }
            else
            {
                Debug.Log("�÷��̾ Ʈ������ �ڿ��� ��ҽ��ϴ�.");
                cameraController.ToggleView(false);
            }
        }
    }
}
