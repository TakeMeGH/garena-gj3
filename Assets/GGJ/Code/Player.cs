using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using System;


public class Player : MonoBehaviour
{
    public GameObject particle;
    public Animator animator;
    
    public TMP_Text coinText;
    public Slider healthBar;
    public InputActionReference move;
    public InputActionReference interact;

    public float health = 10f;
    public float speed = 5f;
    public float moveMaxRange = 40f;




    public GameObject[] whipAtkAreas;
    public GameObject wandBullet;
    
    [Header("Skill Level")]
    public int whipLevel = 10;
    public int garlicLevel = 10;
    public int magicWandLevel = 10;



    // Attack Stats -> derived from 'skill level'
    public float whipDamage = 4f;
    public float whipAtkRadius = 1.5f;

    public float garlicDamage = 4f;
    public float garlicAtkRadius = 3f;
    public float magicWandDamage = 3f;
    public float  magicWandRadius;



    
    private Vector2 _moveDirection;
    private float whipDelay = 1f;
    private float whipTimer;
    private int coin;

    void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;

        RefreshAttackStats();
    }
    void Update()
    {
        // movement input system
        _moveDirection = move.action.ReadValue<Vector2>();

        // move
        transform.Translate(new Vector3(_moveDirection.x, 0, _moveDirection.y).normalized * speed * Time.deltaTime, Space.World);

        // rotate
        if (_moveDirection != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            Vector3 moveDir3D = new Vector3(_moveDirection.x, 0, _moveDirection.y);
            if (moveDir3D.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir3D, Vector3.up);
            }
        } else {
            animator.SetBool("isWalking", false);
        }

        // clamp position, can't move outside border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveMaxRange, moveMaxRange), transform.position.y, Mathf.Clamp(transform.position.z, -moveMaxRange, moveMaxRange));

        // whip
        // whipTimer -= Time.deltaTime;
        // if (whipTimer < 0)
        // {
        //     whipTimer = whipDelay;
        //
        //     MagicWand();
        // }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (whipAtkAreas == null) return;
        Gizmos.color = Color.yellow;
        foreach (GameObject atkArea in whipAtkAreas)
        {
            if (atkArea != null)
            {
                Gizmos.DrawWireSphere(atkArea.transform.position, whipAtkRadius);
            }
        }
        Gizmos.DrawWireSphere(transform.position, garlicAtkRadius);
    }

    public void Whip()
    {
        // whip
        GameObject a = Instantiate(particle, transform.position + transform.right * 2f, Quaternion.identity);
        Destroy(a, 3);
        a = Instantiate(particle, transform.position + transform.right * 4f, Quaternion.identity);
        Destroy(a, 3);
        a = Instantiate(particle, transform.position + transform.right * 6f, Quaternion.identity);
        Destroy(a, 3);

        // Collect all unique enemies in whip attack areas
        HashSet<Enemy> enemies = new HashSet<Enemy>();
        foreach (GameObject atkArea in whipAtkAreas)
        {
            Collider[] colliders = Physics.OverlapSphere(atkArea.transform.position, whipAtkRadius);
            foreach (Collider col in colliders)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemies.Add(enemy);
                }
            }
        }
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(whipDamage, (enemy.transform.position - transform.position).normalized * 10f); // atk damage & knockback
        }
    }

    public void Garlic()
    {
        GameObject a = Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(a, 3);

        Collider[] colliders = Physics.OverlapSphere(transform.position, garlicAtkRadius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(garlicDamage, (enemy.transform.position - transform.position).normalized * 10f); // atk damage & knockback
            }
        }
    }

    public void MagicWand()
    {
        // Find nearest enemy
        Enemy nearestEnemy = null;
        float minDist = float.MaxValue;
        foreach (Enemy enemy in Enemy.allEnemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            print("nearest: " + nearestEnemy.name);
            Vector3 dir = (nearestEnemy.transform.position - transform.position).normalized;
            Quaternion bulletRotation = Quaternion.LookRotation(dir, Vector3.up);
            GameObject instantiatedWandBullet = Instantiate(wandBullet, transform.position, bulletRotation);
            instantiatedWandBullet.GetComponent<WandBullet>().damage = magicWandDamage;
            instantiatedWandBullet.GetComponent<WandBullet>().explosionAtkRadius = magicWandRadius;
        }
    }

    public void RefreshAttackStats(){
        // Approximation to attack stats progression (still need to be adjusted after playtesting)
        // Whip
        whipDamage = Mathf.Clamp(5.2f - whipLevel*0.2f, 0.2f, Mathf.Infinity); // 4 to 0.2 in 27 level
        whipAtkRadius = 1.5f; // maybe don't decrease radius, it can ruin the atk area

        // Garlic
        garlicDamage = Mathf.Clamp(5.2f - garlicLevel*0.2f, 0.2f, Mathf.Infinity);
        garlicAtkRadius = Mathf.Clamp(4f - garlicLevel*0.1f, 0.2f, Mathf.Infinity);

        // Magic Wand
        magicWandDamage = Mathf.Clamp(5.2f - garlicLevel*0.2f, 0.2f, Mathf.Infinity);
        magicWandRadius = Mathf.Clamp(3f - garlicLevel*0.1f, 0.2f, Mathf.Infinity);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Coin"))
        {
            coin += 1;
            coinText.text = "Coin: " + coin.ToString();
            Destroy(col.gameObject);
        }
    }

    
}
