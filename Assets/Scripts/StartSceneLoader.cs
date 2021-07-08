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

        void Start()
        {
            LoadScenes().Forget();        
        }

        async UniTask LoadScenes()
        {
            await SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
            var mainMenuView = GameObject.FindWithTag("MasterCanvas").GetComponentInChildren<MainMenuView>(true);
            ListenStartButton(mainMenuView.StartButton).Forget();
            GameObject.FindWithTag("MainCamera").SetActive(false);
            
            await SceneManager.LoadSceneAsync("intro_1", LoadSceneMode.Additive);
            mIntroScene = SceneManager.GetSceneByName("intro_1");
        }

        private async UniTask ListenStartButton(UIButton button)
        {
            await button.Button.OnClickAsync();
            var director = FindObjectOfType<PlayableDirector>();
            director.Play();
            await UniTask.WaitUntil(()=>director.state != PlayState.Playing);
            await SceneManager.LoadSceneAsync("main", LoadSceneMode.Additive);
            await SceneManager.UnloadSceneAsync("start");
            await SceneManager.UnloadSceneAsync("intro_1");
        } 
        
       
        
        

    }

}
