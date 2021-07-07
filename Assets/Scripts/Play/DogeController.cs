
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
    public float DogeUpperBound;
    public float DogeLowerBound;
    public float DogeLeftBound;
    public float DogeRightBound;

    private List<DogeCommand> mFrameCommands;

    private bool mIsPlaying;

    private const float SpeedConst = 0.1f;
    
    private void Start()
    {
        DogeCamera.transform.localPosition = new Vector3(0, 0, -5);
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        mIsPlaying = false;
    }

    public void StartGame()
    {
        mFrameCommands = new List<DogeCommand>();
        mIsPlaying = true;
    }

    public List<DogeCommand> EndGame()
    {
        mIsPlaying = false;
        return mFrameCommands;
    }

    private void Update()
    {
        if (!mIsPlaying) return;
        
        var command = new DogeCommand();
        var transformDirection = InputDirection();
        var cameraPosition = DogeCamera.transform.localPosition;
        var cameraPositionAfter = cameraPosition + new Vector3(transformDirection.x * -1, transformDirection.y * -1, 0);
        if (cameraPositionAfter.x > CameraLeftBound &&
            cameraPositionAfter.x < CameraRightBound)
            DogeCamera.transform.localPosition += new Vector3(transformDirection.x * -1, 0, 0);
        if (cameraPositionAfter.y < CameraUpperBound &&
            cameraPositionAfter.y > CameraLowerBound)
            DogeCamera.transform.localPosition += new Vector3(0, transformDirection.y * -1, 0);

        var dogePositionAfter = gameObject.transform.localPosition + transformDirection;
        if (dogePositionAfter.x > DogeLeftBound &&
            dogePositionAfter.x < DogeRightBound)
        {
            gameObject.transform.localPosition += new Vector3(transformDirection.x, 0, 0);
            command.Type |= transformDirection.x > 0 ? DogeCommandType.Right :
                transformDirection.x < 0 ? DogeCommandType.Left : DogeCommandType.None;
        }

        if (dogePositionAfter.y < DogeUpperBound &&
            dogePositionAfter.y > DogeLowerBound)
        {
            gameObject.transform.localPosition += new Vector3(0, transformDirection.y, 0);
            command.Type |= transformDirection.y > 0 ? DogeCommandType.Up :
                transformDirection.y < 0 ? DogeCommandType.Down : DogeCommandType.None;
        }

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
            direction += Vector3.up * SpeedConst;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.down * SpeedConst;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left * SpeedConst;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right * SpeedConst;
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
