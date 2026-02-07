using GGJ.Code.Audio;
using Sirenix.OdinInspector;

namespace GGJ.Code.Utils.LevelManager
{
    public class InGameManager : SerializedMonoBehaviour
    {
        void Start()
        {
            AudioManager.Instance.PlayBgm("BGMGameplay");
        }
    }
}