using UnityEngine;
using Unity.Cinemachine; // <--- The correct namespace for V3

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float normalFOV = 5f;
    public float runFOV = 4f; 
    public float zoomSpeed = 5f;

    private CinemachineCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (vcam == null) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetFOV = isRunning ? runFOV : normalFOV;

        // Accessing Lens
        vcam.Lens.OrthographicSize = Mathf.Lerp(vcam.Lens.OrthographicSize, targetFOV, zoomSpeed * Time.deltaTime);
    }
}