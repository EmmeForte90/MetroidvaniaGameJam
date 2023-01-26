using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPlayer : MonoBehaviour
{
public CharacterController2D Stats;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.CompareTag("Enemy"))
    {
        IDamegable hit = other.GetComponent<IDamegable>();
        if (hit != null)
        {
            hit.Damage(Stats.attackDamage);        
        }       
    }
}


}