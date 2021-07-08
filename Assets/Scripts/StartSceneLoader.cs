using Cysharp.Threading.Tasks;
using DogeTraveler.UI;
using Doozy.Engine;
using Doozy.Engine.UI;
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
            
            await SceneManager.LoadSceneAsync("intro_1", LoadSceneMode.Additive);
            mIntroScene = SceneManager.GetSceneByName("intro_1");
        }

        private async UniTask ListenStartButton(UIButton button)
        {
            GameProgressManager.Instance.OnStartClick();
            await button.Button.OnClickAsync();
            var director = FindObjectOfType<PlayableDirector>();
            director.Play();
            await UniTask.WaitUntil(()=>director.state != PlayState.Playing, PlayerLoopTiming.Update, director.GetCancellationTokenOnDestroy());
            await SceneManager.LoadSceneAsync("main");
            GameProgressManager.Instance.OnStartIntroEnd();
            GameEventMessage.SendEvent("GamePlayReady");
        } 
        
       
        
        

    }

}
