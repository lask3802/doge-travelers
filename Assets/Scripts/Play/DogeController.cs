
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityTemplateProjects.Weapon;

public class DogeController:MonoBehaviour
{
    public Camera DogeCamera;
    public GameObject CharacterBody;
    public GameObject ExplodeEffect;
    public float CameraUpperBound;
    public float CameraLowerBound;
    public float CameraLeftBound;
    public float CameraRightBound;
    public float DogeUpperBound;
    public float DogeLowerBound;
    public float DogeLeftBound;
    public float DogeRightBound;
    public UnityEvent OnReplyCompleted;
    private List<DogeCommand> mFrameCommands;
    private Vector3 mStartPos;
    private WeaponManager mWeaponManager;
    public GunHolder GunHolder;
    public LaserHolder LaserHolder;
    public AudioSource MoveSoundSource;

    public bool ShouldPlayExplode;

    private bool mIsPlaying;
    private bool mIsReplay;
    public bool IsReplayCompleted => mReplayFrameCount >= mFrameCommands.Count && mFrameCommands.Count>0;
    private int mReplayFrameCount;

    private bool mMoving;

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
        CharacterBody.SetActive(true);
        if (DogeCamera != null) DogeCamera.gameObject.SetActive(true);
        mFrameCommands = new List<DogeCommand>();
        ResetPosition(startPos);
        mIsPlaying = true;
        mIsReplay = false;
        mMoving = false;
        MoveSoundSource.Stop();
        CharacterBody.SetActive(true);
        ExplodeEffect.SetActive(false);
    }

    public void StopPlay()
    {
        mIsPlaying = false;
        mIsReplay = false;
        mMoving = false;
        MoveSoundSource.Stop();
    }

    public List<DogeCommand> EndGame()
    {
        CharacterBody.SetActive(false);
        mFrameCommands.Add(new DogeCommand());
        return mFrameCommands;
    }

    public void DisableDogeCamera()
    {
        if (DogeCamera != null) DogeCamera.gameObject.SetActive(false);
    }

    public void Pause()
    {
        mIsPlaying = false;
        if (mMoving)
        {
            MoveSoundSource.Pause();
        }
    }
    
    public void Resume()
    {
        mIsPlaying = true;
        if (mMoving)
        {
            MoveSoundSource.Play();
        }
    }
    
    public void PauseReplay()
    {
        mIsReplay = false;
        if (mMoving)
        {
            MoveSoundSource.Pause();
        }
    }
    
    public void ResumeReplay()
    {
        mIsReplay = true;
        if (mMoving)
        {
            MoveSoundSource.Play();
        }
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

        if (mMoving && !IsAnyMoveKeysHold())
        {
            command.Type |= DogeCommandType.MoveEnd;
            MoveSoundSource.Stop();
            mMoving = false;
        }
        else if(!mMoving && IsAnyMoveKeysHold())
        {
            command.Type |= DogeCommandType.MoveBegin;
            MoveSoundSource.Play();
            mMoving = true;
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

    public void RecordLaserBegin()
    {
        if (mIsReplay) return;
        var index = mFrameCommands.Count - 1;
        mFrameCommands[index] = new DogeCommand
        {
            Type = mFrameCommands[index].Type | DogeCommandType.LaserBegin,
        };
    }

    public void RecordLaserFire(Vector3 vector)
    {
        if (mIsReplay) return;
        var index = mFrameCommands.Count - 1;
        mFrameCommands[index] = new DogeCommand
        {
            Type = mFrameCommands[index].Type | DogeCommandType.Laser,
            ShootEndPoint = vector
        };
    }

    public void RecordLaserEnd()
    {
        if (mIsReplay) return;
        var index = mFrameCommands.Count - 1;
        mFrameCommands[index] = new DogeCommand
        {
            Type = mFrameCommands[index].Type | DogeCommandType.LaserEnd,
        };
    }

    private void ReplayUpdate()
    {
        if (mReplayFrameCount >= mFrameCommands.Count)
        {
            //Just fire at first completed
            if (mIsReplay)
            {
                OnReplyCompleted?.Invoke();
            }
            mIsReplay = false;
            if (mMoving)
            {
                MoveSoundSource.Stop();
            }
            return;
        }
            
        var command = mFrameCommands[mReplayFrameCount];
        gameObject.transform.localPosition += DeserializeDirection(command.Type);

        if (command.Type.HasFlag(DogeCommandType.Shoot))
        {
            mWeaponManager.RegisterGunHolder(GunHolder);
            mWeaponManager.GunFire(command.ShootEndPoint);
        }

        if (command.Type.HasFlag(DogeCommandType.Laser))
        {
            mWeaponManager.RegisterLaserHolder(LaserHolder);
            mWeaponManager.LaserFire(command.ShootEndPoint);
        }

        if (command.Type.HasFlag(DogeCommandType.LaserBegin))
        {
            mWeaponManager.RegisterLaserHolder(LaserHolder);
            mWeaponManager.LaserBegin();
        }

        if (command.Type.HasFlag(DogeCommandType.LaserEnd))
        {
            mWeaponManager.RegisterLaserHolder(LaserHolder);
            mWeaponManager.LaserEnd();
        }

        if (command.Type.HasFlag(DogeCommandType.MoveBegin))
        {
            MoveSoundSource.Play();
        }

        if (command.Type.HasFlag(DogeCommandType.MoveEnd))
        {
            MoveSoundSource.Stop();
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

    private bool IsAnyMoveKeysHold()
    {
        return Input.GetKey(KeyCode.W) 
               || Input.GetKey(KeyCode.S) 
               || Input.GetKey(KeyCode.A) 
               || Input.GetKey(KeyCode.D);
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

    public async UniTask ExplodeCharacter()
    {
        CharacterBody.SetActive(false);
        ExplodeEffect.SetActive(true); // animation play 2 sec
        await UniTask.Delay(3100); // audio play 3.1 sec
        ExplodeEffect.SetActive(false);
    }
}
