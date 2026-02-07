using UnityEngine;

public class botParent : MonoBehaviour
{
    public TurnBaseManager turnBaseManager;
    void Start()
    {
        
    }

    void PlayerTakeDamage()
    {
        turnBaseManager.PlayerTakeDamage();
    }
}
