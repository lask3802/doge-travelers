
using System;
using System.Collections.Generic;
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

    private List<List<DogeCommand>> mAllDogeCommands;
    private List<float> mAllDogeStartX;

    private int mRoundCount;

    private void Start()
    {
        mAllDogeCommands = new List<List<DogeCommand>>();
        mAllDogeStartX = new List<float>();
        mRoundCount = 0;
    }

    public void StartGame()
    {
        mAllDogeStartX.Add(0);
        MainCharacterController.StartGame(0);
        mRoundCount++;
    }

    public void StopGame()
    {
        mAllDogeCommands.Add(MainCharacterController.EndGame());
    }

    public void Replay()
    {
        MainCharacterController.Replay(mAllDogeCommands[mRoundCount - 1], mAllDogeStartX[mRoundCount - 1]);
    }
}
