using UnityEngine;
using MilkShake;

public class Bullet : MonoBehaviour
{
    public float fireForce;
    public Vector2 damageRange;
    public float knockBackForce;
    public float timeBetweenShots;
    float damageToDeal;
    Rigidbody2D rb;
    public string[] destroyOnCollisionWith;
    public string[] entitiesToDamage;
    int dir;
    float yDir;
    public Vector2 accuracyRange;

    public GameObject collisionEffect;

    // Camera Shake
    public Shaker camShaker;
    public ShakePreset shakePreset;
    
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageToDeal = Random.Range(damageRange.x, damageRange.y);
    }

    public void Direction(int direction)
    {
        dir = direction;
        yDir = Random.Range(accuracyRange.x, accuracyRange.y);
        transform.Rotate(0f, 0f, yDir*10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(fireForce * dir, yDir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string strtag in destroyOnCollisionWith)
        {
            if (collision.gameObject.tag == strtag)
            {
                Instantiate(collisionEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        foreach (string strtag in entitiesToDamage)
        {
            if (collision.gameObject.tag == strtag)
            {
                camShaker.Shake(shakePreset);
                Health ennmyHealth = collision.GetComponent<Health>();
                ennmyHealth.TakeDamage(damageToDeal);
            }
        }
    }
}
