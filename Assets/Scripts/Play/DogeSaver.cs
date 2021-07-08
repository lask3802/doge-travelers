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
            
        }

        async UniTask CheckClear(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await PlayManager.OnRoundEnd.OnInvokeAsync(cts);
                mMarkSaved.Clear();
                mSavedDoges = 0;
            }
        }

        async UniTask CheckSaveDoge(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await UniTask
                    .WaitUntil(() => PlayManager.PrevDogeControllers.Any(d => d.IsReplayCompleted), PlayerLoopTiming.Update, cts);
                foreach (var playManagerPrevDogeController in PlayManager.PrevDogeControllers)
                {
                    if (!mMarkSaved.Contains(playManagerPrevDogeController)
                        && !playManagerPrevDogeController.IsReplayCompleted)
                    {
                        mMarkSaved.Add(playManagerPrevDogeController);
                        SaveDogeAnimation(playManagerPrevDogeController).Forget();
                    }
                }
            }
        }

        

        async UniTask SaveDogeAnimation(DogeController controller)
        {
            var textPrefab= Instantiate(DogeSaveTextPrefab, null, controller.transform);
            DogeSavedObjects[mSavedDoges++].SetActive(true);
            controller.gameObject.SetActive(false);
            
        }

    }
}