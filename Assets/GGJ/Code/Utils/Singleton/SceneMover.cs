using UnityEngine.SceneManagement;

namespace GGJ.Code.Utils.Singleton
{
    public class SceneMover : Singleton<SceneMover>
    {
        public void MoveScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void MoveSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}