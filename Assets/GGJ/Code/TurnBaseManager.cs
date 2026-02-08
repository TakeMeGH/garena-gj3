using System.Collections;
using GGJ.Code.Ability;
using GGJ.Code.SlotMachine;
using GGJ.Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using GGJ.Code.Ability;
using System.Runtime.ExceptionServices;

public class TurnBaseManager : MonoBehaviour
{
    public static TurnBaseManager Instance { get; private set; }

    public Animator shopPanel;
    public Animator gameOverPanel;
    public TMP_Text waveReachedText;
    public TMP_Text waveReachedTextWhenGameover;
    public TMP_Text coinText;

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
    private int currentWave = 1;
    private bool playerTurnDone = false; // temporary to wait until player done attacking from slot
    private bool shopDone = false;
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

    public GameObject emptyDraggable;

    public SharedAbilityData[] allTokenAbility;
    public class TokenItem
    {
        public SharedAbilityData ability;
        public TokenItem(SharedAbilityData ability)
        {
            this.ability = ability;
        }
    }
    public List<TokenItem> tokensInInventory = new List<TokenItem>();
    public TokenItem[][] tokensInDeck = new TokenItem[4][];
    public Transform deckSlotParent; // contains 16 children : deck slots
    public GameObject deckSlotPrefab;
    private List<DeckSlot> deckSlots = new List<DeckSlot>();
    public DeckSlot inventorySlot;


    void Start()
    {
        for (int i = 0; i < 16; i++)
        {
            GameObject instantiatedDeckSlot = Instantiate(deckSlotPrefab);
            instantiatedDeckSlot.transform.SetParent(deckSlotParent);
            DeckSlot ds = instantiatedDeckSlot.GetComponent<DeckSlot>();
            ds.index = i;
            deckSlots.Add(ds);
        }

        playerMaxHealth = playerHealth;
        UpdateCoinUI();

        // Instantiate enemy
        if (currentWave > enemies.Length)
        {
            return;
        }

        SpawnEnemyForWave();
        UpdatePlayerHealthUI();

        // Initialize inventory (empty)
        tokensInInventory = new List<TokenItem>();

        // Initialize deck with 4x4 grid, all using first index of allTokenAbility
        tokensInDeck = new TokenItem[4][];
        for (int i = 0; i < 4; i++)
        {
            tokensInDeck[i] = new TokenItem[4];
            for (int j = 0; j < 4; j++)
            {
                tokensInDeck[i][j] = new TokenItem(allTokenAbility[0]);
                int idx = i * 4 + j;
                GameObject newDraggable = Instantiate(emptyDraggable);
                newDraggable.transform.SetParent(deckSlots[idx].transform);
                newDraggable.transform.localPosition = Vector3.zero;
                deckSlots[idx].occupied = newDraggable.GetComponent<Draggable>();
                newDraggable.GetComponent<Image>().sprite = tokensInDeck[i][j].ability.Icon;
                newDraggable.GetComponent<Draggable>().tokenItem = tokensInDeck[i][j];
            }
        }
        //Start turn base loop
        StartCoroutine(Shop());
    }
    
    // Adds a token to inventory from SharedAbilityData
    public void AddTokenToInventory(SharedAbilityData abilityData)
    {
        if (abilityData == null) return;
        TokenItem newToken = new TokenItem(abilityData);
        tokensInInventory.Add(newToken);
        // Optionally: update inventory UI here
    }

    IEnumerator Shop()
    {
        // Player do shopping & picking deck
        shopPanel.Play("shopPanelShow");
        yield return new WaitUntil(() => shopDone);
        shopDone = false;

        // Save inventory
        tokensInInventory.Clear();
        foreach (Draggable draggable in inventorySlot.occupieds)
        {
            if (draggable != null && draggable.tokenItem != null)
                tokensInInventory.Add(draggable.tokenItem);
        }

        // Save deck (4x4 grid)
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int idx = i * 4 + j;
                if (idx < deckSlots.Count && deckSlots[idx].occupied != null && deckSlots[idx].occupied.tokenItem != null)
                {
                    tokensInDeck[i][j] = deckSlots[idx].occupied.tokenItem;
                }
                else
                {
                    tokensInDeck[i][j] = null;
                }
            }
        }


        shopPanel.Play("shopPanelHide");
        waveReachedText.text = "Wave Reached: " + currentWave.ToString();
        StartCoroutine(PlayerTurn());
    }

    

    public void ShopDone()
    {
        shopDone = true;
    }

    IEnumerator PlayerTurn()
    {
        if (currentWave > enemies.Length)
        {
            yield break;
        }

        playerTurnDone = false;
        if (slotMachineManager == null)
        {
            slotMachineManager = FindFirstObjectByType<SlotMachineManager>();
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
        playerTurnDone = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        if (currentWave > enemies.Length)
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
        if (currentWave > enemies.Length)
        {
            return;
        }

        TextPopupManager.Instance.CreateDamagePopup(
            instantiatedEnemy.transform.position + new Vector3(0, 2f, 0), enemies[wave - 1].damage);

        playerHealth -= enemies[currentWave - 1].damage;
        UpdatePlayerHealthUI();
        if (playerHealth <= 0)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.Play("gameOverShow");
            waveReachedTextWhenGameover.text = "Wave Reached: " + currentWave.ToString();
            GainCoin(currentWave*100);
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
            currentWave++;
            if (currentWave <= enemies.Length)
            {
                SpawnEnemyForWave();
            }
            else
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
        instantiatedEnemy = Instantiate(enemies[currentWave - 1].prefab, enemyParent);
        enemyHealthBar = instantiatedEnemy.GetComponentInChildren<Slider>();
        enemyHealth = enemies[currentWave - 1].health;
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
        if (currentWave > enemies.Length) return;
        float enemyMaxHealth = enemies[currentWave - 1].health;
        enemyHealthBar.maxValue = enemyMaxHealth;
        enemyHealthBar.value = Mathf.Clamp(enemyHealth, 0f, enemyMaxHealth);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GainCoin(int amount)
    {
        coin += amount;
        coinText.text = "Coin: " + coin.ToString();
    }
    
}
