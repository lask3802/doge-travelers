using System;
using Cysharp.Threading.Tasks;
using DogeTraveler.UI;
using Doozy.Engine;
using Doozy.Engine.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace DogeTraveler
{
    public class StartSceneLoader : MonoBehaviour
    {
        private Scene mIntroScene;
        private static GameObject MasterCanvas;
        private UniTaskCompletionSource mIntroTcs;
        void Start()
        {
            LoadScenes().Forget();
        }

        async UniTask LoadScenes()
        {
            GameEventManager.Instance.enabled = true;
            if (!MasterCanvas)
            {
                await SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
                MasterCanvas = GameObject.FindWithTag("MasterCanvas");
            }

            var mainMenuView = MasterCanvas.GetComponentInChildren<MainMenuView>(true);
            ListenStartButton(mainMenuView.StartButton).Forget();
            //GameObject.FindWithTag("MainCamera").SetActive(false);
            var introUIView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<IntroSkipView>(true);
            introUIView.Skip.Button.OnClickAsObservable()
                .Subscribe(_ => OnIntroSkip()).AddTo(this);

            
            await SceneManager.LoadSceneAsync("intro_1", LoadSceneMode.Additive);
            mIntroScene = SceneManager.GetSceneByName("intro_1");
        }

        private async UniTask ListenStartButton(UIButton button)
        {
            mIntroTcs = new UniTaskCompletionSource();
            GameProgressManager.Instance.OnStartClick();
            await button.Button.OnClickAsync();
            var director = FindObjectOfType<PlayableDirector>();
            director.Play();
            var disposable = UniTask.Delay(TimeSpan.FromSeconds(director.duration)).ToObservable()
                .Subscribe(_ => mIntroTcs.TrySetResult());
            await mIntroTcs.Task;
            disposable.Dispose();            
            await SceneManager.LoadSceneAsync("main");
            GameProgressManager.Instance.OnStartIntroEnd();
            GameEventMessage.SendEvent("GamePlayReady");
        } 
        
        private void OnIntroSkip()
        {
            if (mIntroTcs != null && mIntroTcs.UnsafeGetStatus() == UniTaskStatus.Pending)
                mIntroTcs.TrySetResult();
        }
        
        

    }

}
