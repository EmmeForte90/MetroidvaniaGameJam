using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    Enemy enemy;
   public float maxHealth = 100f;
    public float currentHealth = 100f;

void Start()
    {
        enemy = FindObjectOfType<Enemy>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        enemy.anmHurt();
        
    }


}
