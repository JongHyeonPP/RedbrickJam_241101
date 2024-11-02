using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
    float distanceThreshold = 1f; // PushObject와의 거리 임계값
    float offsetDistance = 0.1f; // 추가 간격
    [SerializeField] Animator animator; // Animator 참조
    private bool isPushing = false; // 밀기 상태 확인

    private void Update()
    {
        // 정면에 일정 거리 이내에 있는 PushObject의 Transform을 가져옴
        Transform pushObjectTransform = CheckForPushObjectInFront();

        // E 키를 눌렀을 때 밀기 시작
        if (pushObjectTransform != null && Input.GetKeyDown(KeyCode.E))
        {
            // 현재 y 위치를 유지하면서 x, z만 이동하도록 위치 계산
            Vector3 directionToPushObject = (pushObjectTransform.position - transform.position).normalized;
            Vector3 targetPosition = CalculateTargetPosition(pushObjectTransform, directionToPushObject);
            transform.position = targetPosition;

            // PushObject를 바라보도록 회전 설정
            transform.LookAt(new Vector3(pushObjectTransform.position.x, transform.position.y, pushObjectTransform.position.z));

            // Animator의 isPush 파라미터를 true로 설정
            animator.SetBool("isPush", true);
            isPushing = true;

            Debug.Log("PushObject의 바로 앞 위치로 이동하고 isPush를 활성화했습니다.");
        }

        // S, A, D 키를 눌렀을 때 밀기 중단
        if (isPushing && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            animator.SetBool("isPush", false);
            isPushing = false;

            Debug.Log("밀기 중단, isPush를 비활성화했습니다.");
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

    // PushObject가 정면의 일정 거리 내에 있으면 Transform을 반환하는 함수
    private Transform CheckForPushObjectInFront()
    {
        // Raycast를 사용해 정면으로 일정 거리 내의 PushObject 체크
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceThreshold))
        {
            // 충돌한 오브젝트가 PushObject 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag("PushObject"))
            {
                return hit.collider.transform;
            }
        }
        return null; // PushObject가 없으면 null 반환
    }

    // PushObject 방향에 따른 캐릭터 위치 설정
    private Vector3 CalculateTargetPosition(Transform pushObjectTransform, Vector3 directionToPushObject)
    {
        Vector3 pushObjectForward = pushObjectTransform.forward;
        Vector3 pushObjectRight = pushObjectTransform.right;

        float forwardDot = Vector3.Dot(pushObjectForward, directionToPushObject);
        float rightDot = Vector3.Dot(pushObjectRight, directionToPushObject);

        Vector3 targetPosition = pushObjectTransform.position;

        // 가장 가까운 방향으로 위치를 설정
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            if (forwardDot > 0) // PushObject의 뒤쪽에 있을 때
            {
                targetPosition -= pushObjectForward * (distanceThreshold + offsetDistance);
            }
            else // PushObject의 앞쪽에 있을 때
            {
                targetPosition += pushObjectForward * (distanceThreshold + offsetDistance);
            }
        }
        else
        {
            if (rightDot > 0) // PushObject의 왼쪽에 있을 때
            {
                targetPosition -= pushObjectRight * (distanceThreshold + offsetDistance);
            }
            else // PushObject의 오른쪽에 있을 때
            {
                targetPosition += pushObjectRight * (distanceThreshold + offsetDistance);
            }
        }

        // y 위치 고정
        targetPosition.y = transform.position.y;
        return targetPosition;
    }
}
