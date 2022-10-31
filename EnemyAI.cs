using UnityEngine;
using MilkShake;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyStats stats;
    bool foundPlayer;
    bool inAttackRange;
    bool m_FacingRight;
    float moveTime;
    public float waitTime;
    float currWaitTime;
    Animator anim;
    int dir = -1;
    PlayerHealth playerHealth;
    public float playerCheckDistance;
    public LayerMask playerLayer;
    float attackCooldown;
    public Transform hitbox;
    public Mesh mesh;
    Health health;
    public GameObject attackEffect;
    public Color effectColor;
    public Shaker shaker;
    public ShakePreset shakePreset;
    bool shouldMove = true;
    GameObject player;
    bool isGrounded;
    public Animator indicatorAnim;
    float indicatorTime;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        moveTime = stats.patrollingRange;
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        health.health = stats.maxHealth;
        currWaitTime = waitTime;
        indicatorTime = stats.attackCooldown + .4f;
        attackEffect.GetComponent<ParticleSystem>().startColor = effectColor;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            AnimationHandler();
            TimerHandler();
            CheckForPlayer();
            if (shouldMove)
                Movement();
        }
    }

    void Movement()
    {
        if (!foundPlayer)
        {
            if (moveTime > 0f) {
                moveTime -= Time.deltaTime;
                rb.velocity = new Vector2(stats.moveSpeed * dir, rb.velocity.y);
            }
            else
            {
                if (currWaitTime > 0f) {
                    currWaitTime -= Time.deltaTime;
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    moveTime = stats.patrollingRange;
                    currWaitTime = waitTime;
                    dir *= -1;
                }
            }    
        }
        else
        {
            if (inAttackRange)
            {
                rb.velocity = Vector2.zero;
                Attack();
            }
            else {
                rb.velocity = new Vector2(stats.runSpeed * dir, rb.velocity.y);
                player = null;
            }
        }

        if (rb.velocity.x < 0f && m_FacingRight)
            Flip();
        else if (rb.velocity.x > 0f && !m_FacingRight)
            Flip();
    }

    void TurnAroundOnHit()
    {
        if (!foundPlayer)
            dir *= -1;
    }

    void TimerHandler()
    {
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (indicatorTime > 0f)
            indicatorTime -= Time.deltaTime;
    }

    void Attack()
    {
        if (attackCooldown <= 0f)
        {
            attackCooldown = stats.attackCooldown;
            anim.SetTrigger("Attack");
        }

        if (indicatorTime <= 0f)
        {
            indicatorAnim.SetTrigger("Indicate");
            indicatorTime = stats.attackCooldown - .4f;
        }
    }

    public void DealDamage()
    {
        if (player != null)
        {
            playerHealth.TakeDamage(stats.attackDamage);
            Instantiate(attackEffect, new Vector2(hitbox.position.x, hitbox.position.y + Random.Range(-.5f, .5f)), Quaternion.identity);
            StartCoroutine(player.GetComponent<PlayerController>().Knockback(stats.knockbackForce, dir, 1f));
            shaker.Shake(shakePreset);
        }
    }

    public void Knockback(Vector2 force, int direction)
    {
        shouldMove = false;

        if (isGrounded)
            rb.velocity = new Vector2(force.x * direction, force.y);
        else
            rb.velocity = Vector2.zero;
    }

    #region Ground Check
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            shouldMove = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            shouldMove = false;
        }
    }

    void AnimationHandler()
    {
        anim.SetInteger("XVelocity", Mathf.CeilToInt(rb.velocity.x));
    }
    #endregion

    void CheckForPlayer()
    {
        foundPlayer = Physics2D.Raycast(transform.position, new Vector3(dir, 0f, 0f), playerCheckDistance, playerLayer);
        inAttackRange = Physics2D.BoxCast(hitbox.position, hitbox.localScale, 0f, new Vector2(0, 0), 0f, playerLayer);

        if (inAttackRange && player == null) {
            player = Physics2D.OverlapBox(hitbox.position, hitbox.localScale, 0f, playerLayer).gameObject;
            playerHealth = player.GetComponent<PlayerHealth>();
            shaker = playerHealth.GetComponent<PlayerController>().camShake;
        }
    }

    void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0, 180f, 0);
    }

    void Death()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, new Vector3(playerCheckDistance*dir, 0f, 0f));
        Gizmos.DrawCube(hitbox.position, hitbox.localScale);
}
}
