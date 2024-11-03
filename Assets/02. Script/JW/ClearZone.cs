using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearZone : MonoBehaviour
{
    public GameObject clearScreen;
    bool isClear;

    private void Start()
    {
        isClear = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.gem1 == 1 && GameManager.instance.gem2 == 1 && GameManager.instance.gem3 == 1)
        {
            Time.timeScale = 0;
            clearScreen.SetActive(true);
            isClear = true;
        }
    }

    private void Update()
    {
        if (isClear && Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1;
            isClear = false;
            ButtonManager.instance.MoveScene("Lobby");
        }
    }

}
