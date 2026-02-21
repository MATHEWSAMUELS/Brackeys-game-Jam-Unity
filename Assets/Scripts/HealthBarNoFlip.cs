using UnityEngine;

public class HealthBarNoFlip : MonoBehaviour
{
    private Quaternion rotationFixed;
    private Vector3 scaleFixed;

    void Start()
    {
        rotationFixed = transform.rotation;
        scaleFixed = transform.localScale;
    }

    void LateUpdate()
    {
        transform.rotation = rotationFixed;   // freeze rotation
        transform.localScale = scaleFixed;    // freeze scale
    }
}