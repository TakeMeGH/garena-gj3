using GGJ.Code.Audio;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace GGJ.Code.Utils.LevelManager
{

    public class InGameManager : SerializedMonoBehaviour
    {
        void Start()
        {
            AudioManager.Instance.PlayBgm("BGMGameplay");
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}