using UnityEngine;
using MilkShake;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    PlayerStats stats;
    BoxCollider2D cld;
    Ladder ladder;
    public Animator firePointAnim;

    [Header("Camera Shake")]
    public Shaker camShake;
    public ShakePreset shootPreset;

    float horizontalMove;
    float verticalMove;
    public bool canMove = true;

    bool isCrouching;
    public bool isOnPlatform;
    CompositeCollider2D platfCollider;

    public bool canFlip = true;

    bool canJump = true;

    public bool isRolling;
    float timeBtwnRolls;

    // Shooting Stuff
    [Header("Shooting Stuff")]
    float actualTimeBtwnShots;
    public Transform firePoint;
    public GameObject bullet;

    public float groundCheckRadius;
    public Transform groundCheckPos;
    public LayerMask groundLayer;
    public bool m_FacingRight = true;
    public bool isGrounded;

    [Header("Interactables")]
    public float interactableCheckRadius;
    public Vector2 interactableCheckPos;
    public LayerMask interactableLayer;
    bool inRangeOfInteractable;
    public SpriteRenderer popUpRenderer;
    public Sprite interactablePopUp;
    Collider2D[] interactables = new Collider2D[0];
    ContactFilter2D contactFilter2D;

    float moveSpeed;

    int jumpsLeft;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cld = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        ladder = GetComponent<Ladder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (horizontalMove > 0f && !m_FacingRight)
        {
            Flip();
        }
        else if (horizontalMove < 0f && m_FacingRight)
        {
            Flip();
        }

        CheckInput();
        Interact();
        PlatformManager();
    }

    void CheckInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (verticalMove == -1f && isGrounded)
        {
            moveSpeed = stats.crouchMoveSpeed;
            isCrouching = true;
        }
        else {
            moveSpeed = stats.runSpeed;
            isCrouching = false;
        }

        //Shooting stuff
        if (Input.GetMouseButton(0) && isGrounded) { 
            Shoot();
            canMove = false;
            anim.SetTrigger("Shoot");
            rb.velocity = Vector3.zero;
        }
        
        if (Input.GetMouseButtonUp(0)) { canMove = true; }

        if (Input.GetKeyDown(KeyCode.LeftShift) && timeBtwnRolls <= 0f)
        { 
            Roll();
        }
    }

    void Roll()
    {
        anim.SetTrigger("Roll");
        isRolling = true;
        anim.SetBool("isRolling", isRolling);
        canMove = false;
        timeBtwnRolls = stats.timeBetweenRolls;
        canFlip = false;
        if (m_FacingRight)
            rb.velocity = new Vector2(stats.rollSpeed, 4f);
        else
        {
            rb.velocity = new Vector2(-stats.rollSpeed, 4f);
        }
    }
    void Rolling()
    {
        isRolling = false; 
        anim.SetBool("isRolling", isRolling);
        rb.velocity = new Vector2(0f, rb.velocity.y);
        canMove = true;
        canFlip = true;
    }

    void Shoot()
    {
        if (actualTimeBtwnShots <= 0)
        {
            isRolling = false;
            GameObject m_bullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
            Bullet bulletScript = m_bullet.GetComponent<Bullet>();
            if (m_FacingRight) { bulletScript.Direction(1); } 
            else { bulletScript.Direction(-1); }
            bulletScript.camShaker = camShake;
            camShake.Shake(shootPreset);
            firePointAnim.SetTrigger("FireRing");
            actualTimeBtwnShots = bulletScript.timeBetweenShots;
        }
    }

    void Interact()
    {
        if (inRangeOfInteractable)
        {
            popUpRenderer.sprite = interactablePopUp;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Collider2D[] obj = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + interactableCheckPos.x, transform.position.y + interactableCheckPos.y), interactableCheckRadius, interactableLayer);
                InteractionManager manager = obj[0].GetComponent<InteractionManager>();
                manager.Interact();

                if (manager.goThroughDoor)
                {
                    transform.position = manager.goToDoor.position;
                    manager.goToDoor.GetComponent<InteractionManager>().Interact();
                }
            }
        }
        else
        {
            popUpRenderer.sprite = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    void PlatformManager()
    {
        if (isCrouching && isOnPlatform)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    // FixedUpdate is called a set amount of times per second, by default 60
    private void FixedUpdate()
    {
        if (isRolling)
        {
            if (m_FacingRight)
                rb.velocity = new Vector2(stats.rollSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(-stats.rollSpeed, rb.velocity.y);
        }
        else if (timeBtwnRolls > 0)
        {
            timeBtwnRolls -= .1f;
            canFlip = true; ;
        }

        AnimationHandler();
        CheckForLayers();
        Move();

        if (actualTimeBtwnShots > 0)
        {
            actualTimeBtwnShots -= .1f;
        }
    }

    void Move()
    {
        if (canMove)
        {
            // Ground Movement
            if (isGrounded)
                rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);
            // Aerial Movement
            else
            {
                if (rb.velocity.x <= stats.maxAirMoveSpeed && rb.velocity.x >= -stats.maxAirMoveSpeed)
                    rb.AddForce(new Vector2(stats.airMoveSpeed * 100f * horizontalMove * Time.deltaTime, 0f));
                else if (horizontalMove == 0)
                    rb.velocity = new Vector2(rb.velocity.x * .95f, rb.velocity.y);

            }
        }
    }

    void CheckForLayers()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, groundLayer);
        inRangeOfInteractable = Physics2D.OverlapCircle(new Vector2(transform.position.x + interactableCheckPos.x, transform.position.y + interactableCheckPos.y), interactableCheckRadius, interactableLayer);

        if (isGrounded && jumpsLeft < 1) { jumpsLeft = stats.airJumps; }
    }

    void AnimationHandler()
    {
        anim.SetInteger("HorizontalMove", (int)Mathf.Ceil(horizontalMove));
        anim.SetInteger("VerticalMove", (int)Mathf.Ceil(rb.velocity.y));
        anim.SetBool("Grounded", isGrounded);
        anim.SetInteger("Crouch", (int)verticalMove);

        if (!isGrounded)
        {
            anim.SetTrigger("Land");
        }
    }

    public void Jump()
    {
        if (canJump)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, stats.jumpForce);
                anim.SetTrigger("Jump");
            }
            else if (jumpsLeft > 0)
            {
                jumpsLeft--;
                rb.velocity = new Vector2(rb.velocity.x, stats.jumpForce);
                anim.SetTrigger("Jump");
            }
        }
    }

    void Flip()
    {
        if (canFlip)
        {
            m_FacingRight = !m_FacingRight;
            transform.Rotate(0, 180f, 0);
            popUpRenderer.transform.Rotate(0, 180f, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + interactableCheckPos.x, transform.position.y + interactableCheckPos.y), interactableCheckRadius);
    }
}
