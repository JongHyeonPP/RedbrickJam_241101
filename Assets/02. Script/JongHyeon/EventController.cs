using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] MainManager mainManager;
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
}
