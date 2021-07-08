
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DogeTraveler.UI;
using EasyButtons;
using Meteoroid;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityTemplateProjects.Weapon;

public class PlayManager : MonoBehaviour
{
    #region Singleton
    private static PlayManager mInstance;
    public static PlayManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                Application.targetFrameRate = 60;
                mInstance = FindObjectOfType<PlayManager>();
            }

            return mInstance;
        }
    }
    #endregion

    public DogeController MainCharacterController;
    public GameObject CharacterRoot;
    public GameObject CharacterPrefab;
    public GameObject Speedline;
    public UnityEvent OnGameStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnWinGame;
    public AudioSource Bgm;
    
    private MeteoroidPatternController mMeteoroidPatternController;
    private WeaponManager mWeaponManager;

    private List<DogeController> mPreviousDoges;
    public IReadOnlyList<DogeController> PrevDogeControllers => mPreviousDoges;
    private List<List<DogeCommand>> mAllDogeCommands;
    private List<float> mAllDogeStartX;
    private Vector3 mCurrentPosition;

    private int mRoundCount;

    void Awake()
    {
        mPreviousDoges = new List<DogeController>();
        mAllDogeCommands = new List<List<DogeCommand>>();
        mAllDogeStartX = new List<float>();
    }
    private void Start()
    {
        mMeteoroidPatternController = FindObjectOfType<MeteoroidPatternController>();
        mWeaponManager = FindObjectOfType<WeaponManager>();
        
        mRoundCount = 0;
        var playUIView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<GamePlayUIView>(true);
        playUIView.Pause.Button.OnClickAsObservable().Subscribe(_ => Pause()).AddTo(this);
        var pauseUIView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<GamePauseUIView>(true);
        pauseUIView.Resume.Button.OnClickAsObservable().Subscribe(_ => Resume()).AddTo(this);
        pauseUIView.BackToTitle.Button.OnClickAsObservable()
            .Subscribe(_ => GameProgressManager.Instance.OnBackToTitle().Forget()).AddTo(this);

        var gameOverUIView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<GameOverUIView>(true);
        gameOverUIView.Retry.Button.OnClickAsObservable()
            .Subscribe(_ => GameProgressManager.Instance.OnRetryGame().Forget()).AddTo(this);
        gameOverUIView.BackToTitle.Button.OnClickAsObservable()
            .Subscribe(_ => GameProgressManager.Instance.OnBackToTitle().Forget()).AddTo(this);
        
        var gameClearUIView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<GameClearUIView>(true);
        gameClearUIView.BackToTitle.Button.OnClickAsObservable()
            .Subscribe(_ => GameProgressManager.Instance.OnBackToTitle().Forget()).AddTo(this);


        mMeteoroidPatternController.ProgressEndAsObservable()
            .Subscribe(_ => WinGame()).AddTo(this);
        mMeteoroidPatternController.MeteoroidHitTargetAsObservable()
            .Subscribe(_ => StopGame().Forget()).AddTo(this);
        mMeteoroidPatternController.SpeedChanged()
            .Subscribe(GameProgressManager.Instance.UpdateGamePlaySpeed).AddTo(this);
        mMeteoroidPatternController.ProgressChanged()
            .Subscribe(GameProgressManager.Instance.UpdateGamePlayProgress).AddTo(this);
        GameProgressManager.Instance.GameProgressState
            .Where(s => s == GameState.Play1 || s == GameState.Play2 || s == GameState.Play3)
            .Subscribe(s => StartGame()).AddTo(this);
    }

    [Button]
    public void StartGame()
    {
        mCurrentPosition = new Vector3(0, 0, mRoundCount*-5);
        MainCharacterController.StartGame(mCurrentPosition);
        if (mRoundCount >= 1)
            mWeaponManager.SetMainGunAvailable();
        if (mRoundCount >= 2)
            mWeaponManager.SetMainLaserAvailable();
        mWeaponManager.RunWeapon();
        mPreviousDoges.ForEach(c => c.Replay());
        mMeteoroidPatternController.PatternStart(1);
        mRoundCount++;
        Speedline.SetActive(true);
        Bgm.Play();
        OnGameStart?.Invoke();
    }
    
    public async UniTaskVoid StopGame()
    {
        mMeteoroidPatternController.PatternStop();
        mWeaponManager.StopWeapon();
        Speedline.SetActive(false);
        mPreviousDoges.ForEach(c => c.StopPlay());
        MainCharacterController.StopPlay();
        Bgm.Stop();
        
        await CheckExplodeDoge();
        
        mPreviousDoges.ForEach(c => c.EndGame());
        var commands = MainCharacterController.EndGame();
        
        mPreviousDoges.Add(CreatePreviousDoge(commands, mCurrentPosition));
        if(mRoundCount != 3)
            MainCharacterController.DisableDogeCamera();
        GameProgressManager.Instance.OnRoundEnd(mRoundCount).Forget();
        OnRoundEnd?.Invoke();
    }

    private async UniTask CheckExplodeDoge()
    {
        if (MainCharacterController.ShouldPlayExplode)
        {
            await MainCharacterController.ExplodeCharacter();
            MainCharacterController.ShouldPlayExplode = false;
        }
        else
        {
            var doge = mPreviousDoges.FirstOrDefault(d => d.ShouldPlayExplode);
            if (doge != null)
            {
                await doge.ExplodeCharacter();
                doge.ShouldPlayExplode = false;
            }
        }
    }

    public void Pause()
    {
        mMeteoroidPatternController.PatternPause();
        MainCharacterController.Pause();
        mWeaponManager.PauseWeapon();
        mPreviousDoges.ForEach(c => c.PauseReplay());
    }

    public void Resume()
    {
        mMeteoroidPatternController.PatternResume();
        MainCharacterController.Resume();
        mWeaponManager.RunWeapon();
        mPreviousDoges.ForEach(c => c.ResumeReplay());
    }

    public void WinGame()
    {
        mMeteoroidPatternController.PatternStop();
        mWeaponManager.StopWeapon();
        Speedline.SetActive(false);
        Bgm.Stop();
        GameProgressManager.Instance.OnWinGame(mRoundCount).Forget();
        OnWinGame?.Invoke();
    }

    public void Replay()
    {
        mPreviousDoges.ForEach(c => c.Replay());
    }

    private DogeController CreatePreviousDoge(List<DogeCommand> commands, Vector3 start)
    {
        var newDoge = Instantiate(CharacterPrefab, CharacterRoot.transform, false);
        var controller = newDoge.GetComponent<DogeController>();
        controller.SetReplay(commands, start);
        return controller;
    }
}
