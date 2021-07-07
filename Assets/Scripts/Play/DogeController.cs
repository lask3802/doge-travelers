
using System;
using System.Collections.Generic;
using UnityEngine;

public class DogeController:MonoBehaviour
{
    public Camera DogeCamera;
    public float CameraUpperBound;
    public float CameraLowerBound;
    public float CameraLeftBound;
    public float CameraRightBound;

    private List<DogeCommand> mFrameCommands;

    private void Start()
    {
        DogeCamera.transform.localPosition = new Vector3(0, 0, -5);
        gameObject.transform.localPosition = new Vector3(-6.64f, -0.25f, 3.46f);
        mFrameCommands = new List<DogeCommand>();
    }

    private void Update()
    {
        var command = new DogeCommand();
        var transformDirection = InputDirection();
        var cameraPosition = DogeCamera.transform.localPosition;
        var cameraPositionAfter = cameraPosition + new Vector3(transformDirection.x * -1, transformDirection.y * -1, 0);
        if (cameraPositionAfter.x > CameraLeftBound &&
            cameraPositionAfter.x < CameraRightBound &&
            cameraPositionAfter.y < CameraUpperBound &&
            cameraPositionAfter.y > CameraLowerBound)
            DogeCamera.transform.localPosition = cameraPositionAfter;
        gameObject.transform.localPosition += transformDirection;
        command.Type |= SerializeDirection(transformDirection.x, transformDirection.y);
        
        if (Input.GetMouseButtonUp(0))
        {
            var mouseViewport = DogeCamera.ScreenToViewportPoint(Input.mousePosition);
            Debug.Log(mouseViewport.x + " " + mouseViewport.y);
            command.ShootingX = mouseViewport.x;
            command.ShootingY = mouseViewport.y;
        }
        
        mFrameCommands.Add(command);
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

    private DogeCommandType SerializeDirection(float x, float y)
    {
        var type = DogeCommandType.None;
        if(x < 0) type |= DogeCommandType.Left;
        if(x > 0) type |= DogeCommandType.Right;
        if(y < 0) type |= DogeCommandType.Down;
        if (y > 0) type |= DogeCommandType.Up;
        return type;
    }
}
