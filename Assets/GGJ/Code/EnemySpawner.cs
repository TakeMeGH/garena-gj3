using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Mono.CSharp;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyBatSolo;
    public GameObject enemyBatCrowd;
    public GameObject enemyZombie;

    public TMP_Text waveText;

    public float spawnMaxRange = 30f; // because crowded may overflow to the side

    private GameObject player;
    
    public static int currentWave = 1; // accessed by Player.cs when death

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(SpawnWave(1));
    }

    void Update()
    {
        
    }

    IEnumerator SpawnWave(int waveNumber)
    {
        currentWave = waveNumber;

        waveText.text = "Wave: " + waveNumber.ToString();


        int batSoloCount = 5;
        int batCrowdCount = 1; Mathf.Max(0, -2 + 2 * waveNumber);
        int zombieCount = 1; Mathf.Clamp(-6 + 3 * waveNumber, 0, 1000);

        float spawnDelay = 0.1f;

        Debug.Log($"Starting Wave {waveNumber}. Total enemies: {batSoloCount + batCrowdCount + zombieCount}");

        while (batSoloCount > 0 || batCrowdCount > 0 || zombieCount > 0)
        {
            // 1. Create a list of available "choices" based on what is left
            List<int> availablePool = new List<int>();
            
            if (batSoloCount > 0) availablePool.Add(0);  // 0 represents Solo Bat
            if (batCrowdCount > 0) availablePool.Add(1); // 1 represents Crowd Bat
            if (zombieCount > 0) availablePool.Add(2);   // 2 represents Zombie

            // 2. Pick a random index from our available options
            int choice = availablePool[UnityEngine.Random.Range(0, availablePool.Count)];

            // 3. Spawn and decrement
            if (choice == 0) {
                SpawnEnemy(0);
                batSoloCount--;
            }
            else if (choice == 1) {
                SpawnEnemy(1);
                batCrowdCount--;
            }
            else {
                SpawnEnemy(2);
                zombieCount--;
            }

            

            // 4. Wait so they don't all pop in at the exact same frame
            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"Wave {waveNumber} complete. Waiting for next wave...");
        yield return new WaitForSeconds(5f);

        if (player == null) yield break; // player has died

        yield return StartCoroutine(SpawnWave(waveNumber + 1));
    }

    void SpawnEnemy(int enemyType)
    {
        if (player == null) return; // if player is dead, can't spawn enemy (player's pos is needed to spawn enemies far away)
        Vector3 spawnPos;
        do
        {
            spawnPos = new Vector3(
            UnityEngine.Random.Range(-spawnMaxRange, spawnMaxRange),
            0,
            UnityEngine.Random.Range(-spawnMaxRange, spawnMaxRange)
            );
        } while (Mathf.Pow(spawnPos.x - player.transform.position.x, 2) + Mathf.Pow(spawnPos.z - player.transform.position.z, 2) < 10f); // far away from player
        


        switch (enemyType)
        {
            case 0:
                SpawnEnemyIndividually(enemyBatSolo, spawnPos);
                break;
            case 1:
                for (int i = -2; i < 3; i++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        GameObject a = SpawnEnemyIndividually(enemyBatCrowd, spawnPos + new Vector3(i, 0, j) * 1f);
                        
                        Vector3 targetPos = spawnPos + 2 * spawnMaxRange * (player.transform.position - spawnPos).normalized;
                        a.GetComponent<Enemy>().SetTarget(targetPos);
                    }
                }
                
                break;
            case 2:
                SpawnEnemyIndividually(enemyZombie, spawnPos);
                break;
        }

    }

    GameObject SpawnEnemyIndividually(GameObject prefab, Vector3 spawnPos){
        return Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
