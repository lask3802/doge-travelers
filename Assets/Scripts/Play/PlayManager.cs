﻿
using System;
using System.Collections.Generic;
using EasyButtons;
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
    private Vector3 mCurrentPosition;

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

    [Button]
    public void StartGame()
    {
        mCurrentPosition = new Vector3(0, 0, mRoundCount*-5);
        MainCharacterController.StartGame(mCurrentPosition);
        mPreviousDoges.ForEach(c => c.Replay());
        mMeteoroidPatternController.PatternStart(1);
        mRoundCount++;
    }

    [Button]
    public void StopGame()
    {
        mMeteoroidPatternController.PatternStop();
        mPreviousDoges.ForEach(c => c.EndGame());
        var commands = MainCharacterController.EndGame();
        mPreviousDoges.Add(CreatePreviousDoge(commands, mCurrentPosition));
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
