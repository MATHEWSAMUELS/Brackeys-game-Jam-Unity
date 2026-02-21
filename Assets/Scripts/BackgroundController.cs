using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPosX, startPosY;
    private float lengthX, lengthY;

    public Transform cam;
    public float parallaxX = 0.5f;   // Horizontal parallax
    public float parallaxY = 0.5f;   // Vertical parallax

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        lengthX = sr.bounds.size.x;
        lengthY = sr.bounds.size.y;
    }

    void LateUpdate()
    {
        // Calculate parallax movement
        float distX = cam.position.x * parallaxX;
        float distY = cam.position.y * parallaxY;

        float tempX = cam.position.x * (1 - parallaxX);
        float tempY = cam.position.y * (1 - parallaxY);

        // Move background
        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        // Infinite scroll on X
        if (tempX > startPosX + lengthX) startPosX += lengthX;
        else if (tempX < startPosX - lengthX) startPosX -= lengthX;

        // Infinite scroll on Y
        if (tempY > startPosY + lengthY) startPosY += lengthY;
        else if (tempY < startPosY - lengthY) startPosY -= lengthY;
    }
}