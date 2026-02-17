using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MotionBlurEffect : MonoBehaviour
{
    [Header("References")]
    // We don't even need the Rigidbody anymore
    public VolumeProfile profile; // Drag your BlurProfile here
    
    [Header("Settings")]
    public float blurAmount = 1.0f; 
    public KeyCode dashKey = KeyCode.E; // The button to trigger blur

    private MotionBlur blur;

    void Start()
    {
        if (profile == null)
        {
            Debug.LogError("Drag the BlurProfile file into the 'Profile' slot!");
            return;
        }

        if (profile.TryGet(out MotionBlur tempBlur))
        {
            blur = tempBlur;
        }
    }

    void Update()
    {
        if (blur == null) return;

        // If we press the Dash Key, turn Blur ON
        if (Input.GetKey(dashKey))
        {
            blur.intensity.value = Mathf.Lerp(blur.intensity.value, blurAmount, Time.deltaTime * 10f);
        }
        else
        {
            // Otherwise, turn Blur OFF
            blur.intensity.value = Mathf.Lerp(blur.intensity.value, 0f, Time.deltaTime * 10f);
        }
    }
}