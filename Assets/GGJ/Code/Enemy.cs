using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10f;
    public float damage = 2f;
    public float speed = 5f;

    public float moveMaxRange = 40f;
    public float hitDuration = 0.1f; // white shader duration when enemy is hit




    private bool isMovingToTarget = false;
    private Vector3 targetPos;


    private GameObject player;
    private Rigidbody rb;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 diffToGoal;
        if (!isMovingToTarget)
        {
            // move to player
            if (player == null)
            {
                return;
            }
            diffToGoal = player.transform.position - transform.position;
            diffToGoal = new Vector3(diffToGoal.x, 0, diffToGoal.z);
        } else
        {
            // move to targetPos
            diffToGoal = targetPos - transform.position;
            diffToGoal = new Vector3(diffToGoal.x, 0, diffToGoal.z);
            if (diffToGoal.magnitude < 1f)
            {
                Destroy(gameObject);
                return;
            }
        }
        
        //transform.Translate(diffToGoal * speed * Time.deltaTime);
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, diffToGoal.normalized * speed, 5 * speed * Time.deltaTime);

        // clamp position, can't move outside border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveMaxRange, moveMaxRange), transform.position.y, Mathf.Clamp(transform.position.z, -moveMaxRange, moveMaxRange));
    }

    public void SetTarget(Vector3 targetPoss)
    {
        isMovingToTarget = true;
        targetPos = targetPoss;
    }

    void OnCollisionStay(Collision col) {

        if (col.gameObject.CompareTag("Player")){
            col.gameObject.GetComponent<Player>().TakeDamage(damage * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage, Vector3 knockbackVelocity)
    {
        rb.linearVelocity = knockbackVelocity;
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
