using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
   public float maxHealth = 100f;
    public float currentHealth = 100f;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // handle enemy death
        GetComponent<Enemy>().Die();
    }
}
