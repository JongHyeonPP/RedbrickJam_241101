using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainManager : MonoBehaviour
{
    public bool isPresent { get; private set; } = true;
    CameraController cameraController;

    // UI
    public GameObject presentButton;
    public GameObject pastButton;



    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        
    }

    private void Start()
    {
        pastButton.SetActive(false);
        presentButton.SetActive(false);
    }

    public void GoPast()
    {
        isPresent = false;
        cameraController.PastVolumeOnOff(true);
        pastButton.SetActive(false);
        presentButton.SetActive(true);
    }

    public void GoPresent()
    {
        isPresent = true;
        cameraController.PastVolumeOnOff(false);
        pastButton.SetActive(true);
        presentButton.SetActive(false);
    }


}
