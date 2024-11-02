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
            Vector3 triggerForward = transform.forward; // 트리거의 정면 방향
            Vector3 playerToTrigger = (transform.position - other.transform.position).normalized; // 플레이어에서 트리거로의 방향

            // 내적을 통해 방향 판별
            float dotProduct = Vector3.Dot(triggerForward, playerToTrigger);

            if (dotProduct > 0)
            {
                Debug.Log("플레이어가 트리거의 앞에서 닿았습니다.");
                cameraController.ToggleView(true);
            }
            else
            {
                Debug.Log("플레이어가 트리거의 뒤에서 닿았습니다.");
                cameraController.ToggleView(false);
            }
        }
    }
}
