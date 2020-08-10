using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    public float health = 100f;
    public AudioSource dieSound;
    private bool isDead = false;
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
            dieSound.Play();
            Debug.Log("SOUNDON");
            isDead = true;
            

        }
        
        
    }
}
