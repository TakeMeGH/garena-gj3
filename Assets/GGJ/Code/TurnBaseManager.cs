using System.Collections;
using GGJ.Code.SlotMachine;
using GGJ.Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBaseManager : MonoBehaviour
{
    public Animator gameOverPanel;
    public TMP_Text waveReachedText;

    [System.Serializable]
    public class Enemy
    {
        public GameObject prefab;
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

    [SerializeField]
    SlotMachineManager slotMachineManager;

    [SerializeField]
    Slider playerHealthBar;

    [SerializeField]
    Slider enemyHealthBar;


    private GameObject instantiatedEnemy;
    private int wave = 1;
    private bool playerTurnDone = false; // temporary to wait until player done attacking from slot
    private float playerMaxHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMaxHealth = playerHealth;

        // Instantiate enemy
        if (wave > enemies.Length)
        {
            return;
        }

        SpawnEnemyForWave();
        UpdatePlayerHealthUI();

        // Start turn base loop
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        if (wave > enemies.Length)
        {
            yield break;
        }

        playerTurnDone = false;
        if (slotMachineManager == null)
        {
            slotMachineManager = FindObjectOfType<SlotMachineManager>();
        }

        if (slotMachineManager)
        {
            slotMachineManager.BeginPlayerTurn();
        }
        else
        {
            playerTurnDone = true;
        }

        yield return new WaitUntil(() => playerTurnDone);
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        if (wave > enemies.Length)
        {
            yield break;
        }

        enemyParent.GetComponent<Animator>().Play("botParentAtk"); // attack animation
        yield return new WaitForSeconds(1f);
        StartCoroutine(PlayerTurn());
    }

    public void PlayerTurnDone()
    {
        playerTurnDone = true;
    }

    public void PlayerTakeDamage()
    {
        if (wave > enemies.Length)
        {
            return;
        }

        TextPopupManager.Instance.CreateDamagePopup(
            instantiatedEnemy.transform.position + new Vector3(0, 2f, 0), enemies[wave - 1].damage);

        playerHealth -= enemies[wave - 1].damage;
        UpdatePlayerHealthUI();
        if (playerHealth <= 0)
        {
            gameOverPanel.Play("gameOverShow");
            waveReachedText.text = "Wave Reached: " + wave.ToString();
            Destroy(player);
        }
    }

    public void EnemyTakeDamage(float damage)
    {
        if (instantiatedEnemy)
            TextPopupManager.Instance.CreateDamagePopup(
                instantiatedEnemy.transform.position + new Vector3(0, 2f, 0), damage);
        enemyHealth -= damage;
        UpdateEnemyHealthUI();
        Debug.Log("Enemy Take Damage " + enemyHealth);
        if (enemyHealth <= 0)
        {
            if (instantiatedEnemy)
            {
                Destroy(instantiatedEnemy);
            }

            wave++;
            if (wave <= enemies.Length)
            {
                SpawnEnemyForWave();
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

    void SpawnEnemyForWave()
    {
        playerHealth = playerMaxHealth;
        UpdatePlayerHealthUI();
        instantiatedEnemy =
            Instantiate(enemies[wave - 1].prefab, enemyParent);
        enemyHealthBar = instantiatedEnemy.GetComponentInChildren<Slider>();
        enemyHealth = enemies[wave - 1].health;
        UpdateEnemyHealthUI();
    }

    void UpdatePlayerHealthUI()
    {
        if (!playerHealthBar) return;
        playerHealthBar.maxValue = playerMaxHealth;
        playerHealthBar.value = Mathf.Clamp(playerHealth, 0f, playerMaxHealth);
    }

    void UpdateEnemyHealthUI()
    {
        if (!enemyHealthBar) return;
        if (wave > enemies.Length) return;
        float enemyMaxHealth = enemies[wave - 1].health;
        enemyHealthBar.maxValue = enemyMaxHealth;
        enemyHealthBar.value = Mathf.Clamp(enemyHealth, 0f, enemyMaxHealth);
    }
}