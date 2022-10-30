using UnityEngine;

public class WeaponItemSpawn : MonoBehaviour
{
    Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0f, 5f);
    }

    void FixedUpdate()
    {
        if (rb != null && rb.velocity.y < 1f)
        {
            Destroy(rb);
        }
    }
}
