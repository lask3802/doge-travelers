
using System;
using UnityEngine;

public class DogeController:MonoBehaviour
{
    public Camera DogeCamera;
    public float CameraUpperBound;
    public float CameraLowerBound;
    public float CameraLeftBound;
    public float CameraRightBound;

    private void Start()
    {
        DogeCamera.transform.localPosition = new Vector3(0, 0, -5);
        gameObject.transform.localPosition = new Vector3(-6.64f, -0.25f, 3.46f);
    }

    private void Update()
    {
        var transformDirection = InputDirection();
        var cameraPosition = DogeCamera.transform.localPosition;
        var cameraPositionAfter = cameraPosition + new Vector3(transformDirection.x * -1, transformDirection.y * -1, 0);
        if (cameraPositionAfter.x > CameraLeftBound &&
            cameraPositionAfter.x < CameraRightBound &&
            cameraPositionAfter.y < CameraUpperBound &&
            cameraPositionAfter.y > CameraLowerBound)
            DogeCamera.transform.localPosition = cameraPositionAfter;
        gameObject.transform.localPosition += transformDirection;
    }

    private Vector3 InputDirection()
    {
        var direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.up * 0.02f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.down * 0.02f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left * 0.02f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right * 0.02f;
        }

        return direction;
    }
    
}
