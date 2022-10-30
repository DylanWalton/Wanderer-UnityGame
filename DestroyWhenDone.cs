using UnityEngine;

public class DestroyWhenDone : MonoBehaviour
{
    void FixedUpdate()
    {
       if(!GetComponent<ParticleSystem>().isPlaying)
            Destroy(gameObject);
    }
}
