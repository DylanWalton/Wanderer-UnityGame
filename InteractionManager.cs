using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    //public Sprite activeSprite;
    //public Sprite inactiveSprite;
    //SpriteRenderer m_renderer;

    Animator anim;
    public bool open;
    public bool isChest;
    public bool isDoor;
    public Transform goToDoor;
    public bool goThroughDoor;
    public GameObject itemToDrop;
    bool hasDroppedItem = false;
    
    // Awake is called upon initialisation
    void Awake()
    {
        //m_renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {

        if (open)
        {
            anim.SetTrigger("Close");
        }
        else {
            anim.SetTrigger("Open");
        }

        // do interaction stuff
        if (isChest && !hasDroppedItem)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
            hasDroppedItem = true;
        }

        if (isDoor && open)
        {
            goThroughDoor = true;
        }
        else {
            goThroughDoor = false;
        }
    }

    public void SetState()
    {
        open = !open;
    }
}
