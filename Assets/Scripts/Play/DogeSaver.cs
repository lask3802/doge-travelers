using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Play
{
    public class DogeSaver : MonoBehaviour
    {
        public List<GameObject> DogeSavedObjects;
        public GameObject DogeSaveTextPrefab;
        public PlayManager PlayManager;
        private List<DogeController> mMarkSaved = new List<DogeController>();
        private int mSavedDoges = 0; 
        void Start()
        {
            CheckSaveDoge(this.GetCancellationTokenOnDestroy()).Forget();
            CheckClear(this.GetCancellationTokenOnDestroy()).Forget();
        }

        async UniTask CheckClear(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await PlayManager.OnRoundEnd.OnInvokeAsync(cts);
                mMarkSaved.Clear();
                foreach (var dogeSavedObject in DogeSavedObjects)
                {
                    dogeSavedObject.SetActive(false);
                }
                mSavedDoges = 0;
            }
        }

        async UniTask CheckSaveDoge(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                while (PlayManager.PrevDogeControllers.Count == 0)
                {
                    await UniTask.NextFrame(cts);
                }
                var triggeredIdx = await UniTask.WhenAny(PlayManager.PrevDogeControllers
                    .Select(c => c.OnReplyCompleted.OnInvokeAsync(cts)));
                var dogeController = PlayManager.PrevDogeControllers[triggeredIdx];

                if (!mMarkSaved.Contains(dogeController)
                    && dogeController.IsReplayCompleted)
                {
                    mMarkSaved.Add(dogeController);
                    SaveDogeAnimation(dogeController).Forget();
                }
                await UniTask.NextFrame(cts);
            }
        }

        async UniTask SaveDogeAnimation(DogeController controller)
        {
            Debug.Log("Saved doge");
            var textPrefab= Instantiate(DogeSaveTextPrefab, null, controller.transform);
            DogeSavedObjects[mSavedDoges++].SetActive(true);
            controller.gameObject.SetActive(false);
            
        }

    }
}