
using System;
using System.Collections.Generic;
using Meteoroid;
using UniRx;
using UnityEngine;

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
                mInstance = FindObjectOfType<PlayManager>();
            }

            return mInstance;
        }
    }
    #endregion

    public DogeController MainCharacterController;
    public GameObject CharacterRoot;
    public GameObject CharacterPrefab;
    private MeteoroidPatternController mMeteoroidPatternController;

    private List<DogeController> mPreviousDoges;
    private List<List<DogeCommand>> mAllDogeCommands;
    private List<float> mAllDogeStartX;
    private float mCurrentStartX;

    private int mRoundCount;

    private void Start()
    {
        mMeteoroidPatternController = FindObjectOfType<MeteoroidPatternController>();
        mPreviousDoges = new List<DogeController>();
        mAllDogeCommands = new List<List<DogeCommand>>();
        mAllDogeStartX = new List<float>();
        mRoundCount = 0;

        mMeteoroidPatternController.ProgressEndAsObservable()
            .Subscribe(_ => StopGame());
        mMeteoroidPatternController.MeteoroidHitTargetAsObservable()
            .Subscribe(_ => StopGame());
    }

    public void StartGame()
    {
        mCurrentStartX = mRoundCount * 2;
        MainCharacterController.StartGame(mCurrentStartX);
        mPreviousDoges.ForEach(c => c.Replay());
        mMeteoroidPatternController.PatternStart(1);
        mRoundCount++;
    }

    public void StopGame()
    {
        mMeteoroidPatternController.PatternStop();
        mPreviousDoges.ForEach(c => c.EndGame());
        var commands = MainCharacterController.EndGame();
        mPreviousDoges.Add(CreatePreviousDoge(commands, mCurrentStartX));
    }

    public void Replay()
    {
        mPreviousDoges.ForEach(c => c.Replay());
    }

    private DogeController CreatePreviousDoge(List<DogeCommand> commands, float startX)
    {
        var newDoge = Instantiate(CharacterPrefab, CharacterRoot.transform, false);
        var controller = newDoge.GetComponent<DogeController>();
        controller.SetReplay(commands, startX);
        return controller;
    }
}
