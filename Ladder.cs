using UnityEngine;

public class Ladder : MonoBehaviour
{
    PlayerController controller;
    PlayerStats stats;
    public LayerMask playerLayer;
    public LayerMask platformLayer;
    Animator anim;

    Rigidbody2D rb;
    BoxCollider2D cld;
    bool canClimb;
    GameObject ladder;
    public bool isOnLadder;
    int i;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {
            ladder = collision.gameObject;
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {
            canClimb = false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cld = GetComponent<BoxCollider2D>();
        controller = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canClimb)
        {
            if (Input.GetKey(KeyCode.S) && (controller.isOnPlatform || !controller.isGrounded))
            {
                transform.position = new Vector2(ladder.transform.position.x, transform.position.y);
                isOnLadder = true;
                controller.canMove = false;
                controller.canFlip = false;
                cld.isTrigger = true;
            }

            if (Input.GetKey(KeyCode.W))
            {
                transform.position = new Vector2(ladder.transform.position.x, transform.position.y);
                isOnLadder = true;
                controller.canMove = false;
                controller.canFlip = false;
                cld.isTrigger = true;
            }

            if (controller.isGrounded && Input.GetKey(KeyCode.S) && !controller.isOnPlatform)
            {
                isOnLadder = false;
            }

            if (isOnLadder)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    isOnLadder = false;
                    rb.velocity = new Vector2(rb.velocity.x, stats.jumpForce);
                    anim.SetTrigger("Jump");
                }
            }
        }
        else { 
            isOnLadder = false;
        }
    }

    private void FixedUpdate()
    {
        if (isOnLadder)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0f, stats.ladderClimbSpeed * Input.GetAxisRaw("Vertical"));
            anim.SetBool("OnLadder", true);
            anim.SetFloat("LadderBlend", (int)Input.GetAxisRaw("Vertical"));
            i = 1;
        }
        else {
            rb.gravityScale = 3f;
            if (!controller.isRolling)
                controller.canMove = true;
            cld.isTrigger = false;
            if (i == 1)
            { 
                i = 0;
                controller.canFlip = true;
            }
            anim.SetBool("OnLadder", false);
        }
    }
}
