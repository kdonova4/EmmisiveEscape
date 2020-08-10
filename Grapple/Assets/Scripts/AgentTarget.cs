using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTarget : MonoBehaviour
{
    public float health = 100f;
    public AudioSource dieSound;
    private bool isDead;
    // Start is called before the first frame update
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        
        if (isDead == false)
        {
            
            isDead = true;
            
            
        }
        
        
    }
}
