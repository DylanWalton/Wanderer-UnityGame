using UnityEngine;

public class Platforms : MonoBehaviour
{
    BoxCollider2D cld;
    bool activate;
    BoxCollider2D plr;
    public float passTime;
    float currPassTime;
    
    private void Awake()
    {
        cld = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (activate)
        {
            if (Input.GetKey(KeyCode.S) && Input.GetButtonDown("Jump"))
            {
                Physics2D.IgnoreCollision(cld, plr, true);
                currPassTime = passTime;
            }
        }

        if (currPassTime > 0f)
        {
            currPassTime -= .1f;
        }
        else if (Input.GetButtonDown("Jump")) {
            if (plr != null)
            {
                Physics2D.IgnoreCollision(cld, plr, false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            plr = collision.gameObject.GetComponent<BoxCollider2D>();
            activate = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            activate = false;
        }
    }
}
