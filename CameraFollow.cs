using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetTransform;
    Rigidbody2D targetRb;
    public Vector3 followOffset;
    public Vector2 greyArea; // Zone around the object within which it can move without the camera following it
    public Mesh mesh;
    public Color greyAreaColor;
    
    public float followSpeed;
    public bool recenterOnStop;
    public float timeBeforeRecenter;
    float tbrCooldown;

    private void Awake()
    {
        tbrCooldown = timeBeforeRecenter;
        
        transform.position = new Vector3(targetTransform.position.x - followOffset.x, targetTransform.position.y - followOffset.y, -followOffset.z);
        
        if (targetTransform.GetComponent<Rigidbody2D>() != null)
        {
            targetRb = targetTransform.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If the target is not moving, center the camera on it
        if (recenterOnStop)
        {
            if (targetRb.velocity == Vector2.zero)
            {
                tbrCooldown -= Time.deltaTime;
            }

            if (tbrCooldown <= 0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetTransform.position.x - followOffset.x, targetTransform.position.y - followOffset.y, -followOffset.z), followSpeed * .1f);
            }
        }

        if (targetRb.velocity != Vector2.zero)
        {
            tbrCooldown = timeBeforeRecenter;
        }

        if (targetTransform.position.x - transform.position.x >= greyArea.x)
        {
            transform.position = new Vector3(targetTransform.position.x - greyArea.x, transform.position.y, -followOffset.z);
        }
        else if (targetTransform.position.x - transform.position.x <= -greyArea.x)
        {
            transform.position = new Vector3(targetTransform.position.x + greyArea.x, transform.position.y, -followOffset.z);
        }

        if (targetTransform.position.y - transform.position.y >= greyArea.y)
        {
            transform.position = new Vector3(transform.position.x, targetTransform.position.y - greyArea.y, -followOffset.z);
        }
        else if (targetTransform.position.y - transform.position.y <= -greyArea.y)
        {
            transform.position = new Vector3(transform.position.x, targetTransform.position.y + greyArea.y, -followOffset.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = greyAreaColor;
        Gizmos.DrawMesh(mesh, 0, new Vector2(transform.position.x - followOffset.x, transform.position.y - followOffset.y), Quaternion.identity, new Vector3(greyArea.x*2, greyArea.y*2));
    }
}
