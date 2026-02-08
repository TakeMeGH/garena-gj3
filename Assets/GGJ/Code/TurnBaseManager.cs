using System.Collections;
using GGJ.Code.Ability;
using GGJ.Code.SlotMachine;
using GGJ.Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBaseManager : MonoBehaviour
{
    public static TurnBaseManager Instance { get; private set; }

    public Animator gameOverPanel;
    public TMP_Text waveReachedText;

    [System.Serializable]
    public class Enemy
    {
        public GameObject prefab;
        public float health = 10;
        public float damage = 3;
        public int coinReward = 1;
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

    [SerializeField]
    TMP_Text coinText;

    private GameObject instantiatedEnemy;
    private int wave = 1;
    private bool playerTurnDone = false; // temporary to wait until player done attacking from slot
    private float playerMaxHealth;
    private int coin;
    private bool waitingForWaveUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMaxHealth = playerHealth;
        UpdateCoinUI();

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
        if (waitingForWaveUI) yield break;
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
        if (waitingForWaveUI) yield break;
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

            AddCoins(enemies[wave - 1].coinReward);
            if (!waitingForWaveUI)
            {
                // waitingForWaveUI = true;
                StopAllCoroutines();
                StartCoroutine(HandleWaveClear());
            }
        }
    }

    IEnumerator HandleWaveClear()
    {
        // LevelDownSelectorUI selector = LevelDownSelectorUI.Instance;
        // if (selector != null)
        // {
        //     AbilityShopManager shop = AbilityShopManager.Instance;
        //     SharedAbilityData[] options = shop != null ? shop.GenerateShopOptions() : new SharedAbilityData[0];
        //     bool closed = false;
        //     System.Action onClosed = () => closed = true;
        //     selector.Closed += onClosed;
        //     selector.Show(options);
        //     yield return new WaitUntil(() => closed);
        //     selector.Closed -= onClosed;
        // }
        yield return null;
        waitingForWaveUI = false;
        wave++;
        if (wave <= enemies.Length)
        {
            SpawnEnemyForWave();
            StartCoroutine(PlayerTurn());
        }
        else
        {
            StopAllCoroutines();
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

    void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coin += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (!coinText) return;
        coinText.text = "Coin: " + coin.ToString();
    }

    public int Coin => coin;

    public bool CanAfford(int cost)
    {
        return cost <= coin;
    }

    public bool SpendCoins(int cost)
    {
        if (cost <= 0) return true;
        if (!CanAfford(cost)) return false;
        coin -= cost;
        UpdateCoinUI();
        return true;
    }
}
