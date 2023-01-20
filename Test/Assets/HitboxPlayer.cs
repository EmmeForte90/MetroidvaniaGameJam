using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPlayer : MonoBehaviour
{
    public GameObject Hit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            
            Debug.Log("hai colpito il nemico");
          // other.GetComponent<Enemy>().TakeDamage(attackDamage);
        

        }
    }
}
