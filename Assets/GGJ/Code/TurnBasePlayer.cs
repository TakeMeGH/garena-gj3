using UnityEngine;

public class TurnBasePlayer : MonoBehaviour
{
    public void EnemyTakeDamage()
    {
        FindFirstObjectByType<TurnBaseManager>().EnemyTakeDamage();
    }
}
