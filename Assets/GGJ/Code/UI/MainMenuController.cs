using UnityEngine;

namespace GGJ.Code.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public void GoToGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TurnBasedSlot 1");
        }
    }
}