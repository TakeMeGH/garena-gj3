using UnityEngine;

public class WandBullet : MonoBehaviour
{
    public GameObject particle;
    public float speed = 15;
    public float damage = 3f; // determined by Player.cs
    public float explosionAtkRadius = 2f; // determined by Player.cs

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //collision.GetComponent<Enemy>().TakeDamage(damage, transform.forward * 10f);

            
            GameObject a = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(a, 3);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionAtkRadius);
            foreach (Collider col in colliders)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, (enemy.transform.position - transform.position).normalized * 5f); // atk damage & knockback
                }
            }

            Destroy(gameObject);
        }   
    }
}
