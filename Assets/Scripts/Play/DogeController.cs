
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects.Weapon;

public class DogeController:MonoBehaviour
{
    public Camera DogeCamera;
    public GameObject CharacterBody;
    public float CameraUpperBound;
    public float CameraLowerBound;
    public float CameraLeftBound;
    public float CameraRightBound;
    public float DogeUpperBound;
    public float DogeLowerBound;
    public float DogeLeftBound;
    public float DogeRightBound;

    private List<DogeCommand> mFrameCommands;
    private Vector3 mStartPos;
    private WeaponManager mWeaponManager;
    public GunHolder GunHolder;

    private bool mIsPlaying;
    private bool mIsReplay;
    private int mReplayFrameCount;

    private const float SpeedConst = 0.1f;
    
    private void Start()
    {
        mIsPlaying = false;
        mIsReplay = false;
        mWeaponManager = FindObjectOfType<WeaponManager>();
    }

    private void ResetPosition(Vector3 start)
    {
        if(DogeCamera != null) DogeCamera.transform.localPosition = new Vector3(0f, 0f, -2f);
        gameObject.transform.localPosition = start;
    }

    public void StartGame(Vector3 startPos)
    {
        mFrameCommands = new List<DogeCommand>();
        ResetPosition(startPos);
        mIsPlaying = true;
        mIsReplay = false;
    }

    public List<DogeCommand> EndGame()
    {
        mIsPlaying = false;
        mIsReplay = false;
        return mFrameCommands;
    }

    public void SetReplay(List<DogeCommand> commands, Vector3 start)
    {
        mFrameCommands = commands;
        mStartPos = start;
        CharacterBody.SetActive(false);
    }

    public void Replay()
    {
        CharacterBody.SetActive(true);
        mIsPlaying = false;
        mIsReplay = true;
        mReplayFrameCount = 0;
        ResetPosition(mStartPos);
    }

    private void Update()
    {
        if (mIsPlaying)
            PlayingUpdate();
        if (mIsReplay)
            ReplayUpdate();
        
        
    }

    private void PlayingUpdate()
    {
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

        mFrameCommands.Add(command);
    }

    public void RecordGunFire(Vector3 vector)
    {
        if (mIsReplay) return;
        var index = mFrameCommands.Count - 1;
        mFrameCommands[index] = new DogeCommand
        {
            Type = mFrameCommands[index].Type | DogeCommandType.Shoot,
            ShootEndPoint = vector
        };
    }

    private void ReplayUpdate()
    {
        if (mReplayFrameCount >= mFrameCommands.Count)
        {
            mIsReplay = false;
            return;
        }
            
        var command = mFrameCommands[mReplayFrameCount];
        gameObject.transform.localPosition += DeserializeDirection(command.Type);

        if (command.Type.HasFlag(DogeCommandType.Shoot))
        {
            mWeaponManager.RegisterGunHolder(GunHolder);
            mWeaponManager.GunFire(command.ShootEndPoint);
        }
            

        mReplayFrameCount++;
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

    private Vector3 DeserializeDirection(DogeCommandType type)
    {
        var direction = Vector3.zero;
        if(type.HasFlag(DogeCommandType.Up))
            direction += Vector3.up * SpeedConst;
        if(type.HasFlag(DogeCommandType.Down))
            direction += Vector3.down * SpeedConst;
        if(type.HasFlag(DogeCommandType.Left))
            direction += Vector3.left * SpeedConst;
        if(type.HasFlag(DogeCommandType.Right))
            direction += Vector3.right * SpeedConst;
        return direction;
    }
}
