using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10f;
    public float damage = 2f;
    public float speed = 5f;

    public float moveMaxRange = 40f;



    private bool isMovingToTarget = false;
    private Vector3 targetPos;


    private GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 diffToGoal;
        if (!isMovingToTarget)
        {
            // move to player
            diffToGoal = player.transform.position - transform.position;
        } else
        {
            // move to targetPos
            diffToGoal = targetPos - transform.position;
            if (diffToGoal.magnitude < 5f)
            {
                Destroy(gameObject);
            }
        }
        
        
        diffToGoal = new Vector3(diffToGoal.x, 0, diffToGoal.z).normalized;
        transform.Translate(diffToGoal * speed * Time.deltaTime);

        // clamp position, can't move outside border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -moveMaxRange, moveMaxRange), transform.position.y, Mathf.Clamp(transform.position.z, -moveMaxRange, moveMaxRange));
    }

    public void SetTarget(Vector3 targetPoss)
    {
        isMovingToTarget = true;
        targetPos = targetPoss;
    }
}
