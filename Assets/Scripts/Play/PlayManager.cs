
using System;
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

    private void Start()
    {
        
    }

    public void StartGame()
    {
        MainCharacterController.StartGame();
    }

    public void StopGame()
    {
        MainCharacterController.EndGame();
    }
    
}
