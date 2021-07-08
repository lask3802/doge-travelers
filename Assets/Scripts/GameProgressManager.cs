using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameProgressManager : MonoBehaviour
{
    #region Singleton

    public static GameProgressManager Instance => mInstance;

    private static GameProgressManager mInstance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RuntimeInit()
    {
        Application.targetFrameRate = 60;
        if (FindObjectOfType<GameProgressManager>() != null)
            return;

        var go = new GameObject {name = "[GameProgressManager]"};
        mInstance = go.AddComponent<GameProgressManager>();
        DontDestroyOnLoad(go);
    }

    #endregion

    public IReadOnlyReactiveProperty<GameState> GameProgressState => mGameProgressState;
    public IReadOnlyReactiveProperty<float> GamePlaySpeed => mGamePlaySpeed;
    public IReadOnlyReactiveProperty<float> GamePlayProgress => mGamePlayProgress;

    private ReactiveProperty<GameState> mGameProgressState;
    private ReactiveProperty<float> mGamePlaySpeed;
    private ReactiveProperty<float> mGamePlayProgress;
    private int mRound;

    private void Awake()
    {
        mGameProgressState = new ReactiveProperty<GameState>();
        mGamePlaySpeed = new ReactiveProperty<float>();
        mGamePlayProgress = new ReactiveProperty<float>();
    }

    public void UpdateGamePlaySpeed(float speed)
    {
        mGamePlaySpeed.Value = speed;
    }

    public void UpdateGamePlayProgress(float progress)
    {
        mGamePlayProgress.Value = progress;
    }

    public void OnStartClick()
    {
        mGameProgressState.Value = GameState.Intro1;
    }

    public void OnStartIntroEnd()
    {
        mGameProgressState.Value = GameState.Play1;
    }

    public async UniTask OnRoundEnd(int round)
    {
        mRound = round;
        switch (round)
        {
            case 1:
                mGameProgressState.Value = GameState.Intro2;
                GameEventMessage.SendEvent("RoundEnd");
                await RunCutSceneAsync("intro_2");
                mGameProgressState.Value = GameState.Play2;
                GameEventMessage.SendEvent("GamePlayReady");
                break;
            case 2:
                mGameProgressState.Value = GameState.Intro3;
                GameEventMessage.SendEvent("RoundEnd");
                await RunCutSceneAsync("intro_3");
                mGameProgressState.Value = GameState.Play3;
                GameEventMessage.SendEvent("GamePlayReady");
                break;
            case 3:
                mGameProgressState.Value = GameState.Failed;
                await OnFailedGame();
                break;
        }
    }

    public async UniTask OnWinGame(int round)
    {
        mGameProgressState.Value = GameState.Win;
        GameEventMessage.SendEvent("GameClear");
        await RunClearSceneAsync();
        GameEventMessage.SendEvent("LoadClearUI");
    }

    public async UniTask OnFailedGame()
    {
        mGameProgressState.Value = GameState.Failed;
        GameEventMessage.SendEvent("GameFailed");
    }

    public async UniTask OnRetryGame()
    {
        await SceneManager.LoadSceneAsync("main");
        mGameProgressState.Value = GameState.Play1;
    }

    public async UniTask OnBackToTitle()
    {
        await SceneManager.LoadSceneAsync("start");
        mGameProgressState.Value = GameState.Start;
    }

    private async UniTask RunCutSceneAsync(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        var director = FindObjectOfType<PlayableDirector>();
        director.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(director.duration));
        //await UniTask.WaitUntil(()=>director.state != PlayState.Playing, PlayerLoopTiming.Update, director.GetCancellationTokenOnDestroy());
        Debug.Log($"{sceneName} Director play done");
        await SceneManager.UnloadSceneAsync(sceneName);
    }

    private async UniTask RunClearSceneAsync()
    {
        await SceneManager.LoadSceneAsync("game_clear", LoadSceneMode.Additive);
        var director = FindObjectOfType<PlayableDirector>();
        director.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(director.duration));
        //await UniTask.WaitUntil(()=>director.state != PlayState.Playing, PlayerLoopTiming.Update, director.GetCancellationTokenOnDestroy());
        Debug.Log($"{"game_clear"} Director play done");
    }
}

public enum GameState
{
    Start,
    Intro1,
    Play1,
    Intro2,
    Play2,
    Intro3,
    Play3,
    Failed,
    Win,
    EndGame
    
}

public struct PlayStatus
{
    public float Progress;
    public float Speed;
}
