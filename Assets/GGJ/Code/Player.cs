using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject particle;
    
    public Slider healthBar;

    public float health = 10f;
    public float whipDamage = 2f;
    public float speed = 5f;

    public InputActionReference move;
    public InputActionReference interact;

    public float moveMaxRange = 40f;

    private Vector2 _moveDirection;


    public GameObject[] whipAtkAreas;
    public float whipAtkRadius;
    private float whipDelay = 1f;
    private float whipTimer;

    void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
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
            // Calculate the angle in radians using Atan2, which handles all quadrants correctly
            float angleRadians = Mathf.Atan2(_moveDirection.y, _moveDirection.x);

            // Convert the angle from radians to degrees
            float angleDegrees = - angleRadians * Mathf.Rad2Deg;

            // Create a Quaternion rotation around the Z-axis (forward direction for 2D)
            // Note: If your sprite's "forward" is actually 'up', you may need to adjust the angle (e.g., subtract 90 degrees)
            Quaternion targetRotation = Quaternion.AngleAxis(angleDegrees, Vector3.up);

            // Apply the rotation to the GameObject's transform
            transform.rotation = targetRotation;
        }

        // clamp position, can't move outside border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveMaxRange, moveMaxRange), transform.position.y, Mathf.Clamp(transform.position.z, -moveMaxRange, moveMaxRange));

        // whip
        whipTimer -= Time.deltaTime;
        if (whipTimer < 0)
        {
            whipTimer = whipDelay;

            // whip
            Instantiate(particle, transform.position, Quaternion.identity);

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
    }
}
