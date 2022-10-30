using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform player;
    
    void Start()
    {
        foreach (EnemyAI child in gameObject.GetComponentsInChildren<EnemyAI>())
        {
            child.player = player;
        }
    }
}
