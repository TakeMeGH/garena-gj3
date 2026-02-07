using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;
using TMPro;

public class TurnBaseManager : MonoBehaviour
{
    public Animator gameOverPanel;
    public TMP_Text waveReachedText;
    [System.Serializable]
    public class Enemy
    {
        public bool isZombiebot = false;
        public float health = 10;
        public float damage = 3;

    }
    public Enemy[] enemies;

    public float playerHealth = 10f;
    private float enemyHealth;

    public GameObject player;
    public GameObject batbotPrefab;
    public GameObject zombotPrefab;
    public Transform enemyParent;
    


    private GameObject instantiatedEnemy;
    private int wave = 1;
    private bool playerTurnDone = false; // temporary to wait until player done attacking from slot
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Instantiate enemy
        if (wave >= enemies.Length)
        {
            return;
        }
        GameObject instantiatedEnemy = Instantiate((enemies[wave-1].isZombiebot) ? zombotPrefab : batbotPrefab, enemyParent);
        enemyHealth = enemies[wave-1].health;

        // Start turn base loop
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        //yield return new WaitUntil(() => playerTurnDone); // TODO: replace with slot mechanic
        //playerTurnDone = false;
        yield return new WaitForSeconds(5f);
        
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        enemyParent.GetComponent<Animator>().Play("botParentAtk"); // attack animation
        yield return new WaitForSeconds(6f);
        StartCoroutine(PlayerTurn());
    }

    public void PlayerTurnDone()
    {
        playerTurnDone = true;
    }

    public void PlayerTakeDamage()
    {
        playerHealth -= enemies[wave-1].damage;
        if (playerHealth <= 0)
        {
            gameOverPanel.Play("gameOverShow");
            waveReachedText.text = "Wave Reached: " + wave.ToString();
            Destroy(player);
        }
    }
    public void EnemyTakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Destroy(instantiatedEnemy);
        }
    }
}
