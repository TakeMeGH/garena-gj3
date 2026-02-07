using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtkArea : MonoBehaviour
{
    List<Enemy> enemiesInArea;

    void Start()
    {
        enemiesInArea = new List<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInArea.Add(other.GetComponent<Enemy>());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInArea.Add(other.GetComponent<Enemy>());
        }
    }

    public List<Enemy> GetEnemiesInArea()
    {
        enemiesInArea.RemoveAll(enemy => enemy == null);
        // foreach (Enemy enemy in Enemy.allEnemies)
        // {
        //     if (GetComponent<Collider>())
        // }
        return enemiesInArea;


    }
}
