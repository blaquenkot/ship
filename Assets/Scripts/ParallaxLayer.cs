using System;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour 
{
    private Camera Camera;
    private Vector3 CameraLastPosition = Vector3.zero;
    private float MovementFactor = 0f;

    void Awake() 
    {
        Camera = UnityEngine.Object.FindObjectOfType<Camera>();
        CameraLastPosition = Camera.transform.position;
        MovementFactor = Mathf.Abs(1/transform.position.z);

        if(MovementFactor == Mathf.Infinity) 
        {
            throw new Exception();
        }
    }

    void FixedUpdate() 
    {
        Vector3 diff = Camera.transform.position - CameraLastPosition;
        transform.position += diff * MovementFactor;
        CameraLastPosition = Camera.transform.position;
    }
}