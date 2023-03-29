using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject VFX;
    public GameObject Shield;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }


  private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Hitbox"))
    {
        currentHealth -= Move.instance.Damage;
        if (currentHealth <= 0)
        {
        Instantiate(VFX, transform.position, transform.rotation);
        Destroy(Shield);
        }
    }
}
}
