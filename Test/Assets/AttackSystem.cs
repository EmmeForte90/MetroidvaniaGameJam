using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class AttackSystem : MonoBehaviour
{
    
    public AnimationClip chargeAnimation; // L'animazione di carica dell'attacco
    public AnimationClip attackAnimation; // L'animazione di attacco vera e propria

    private Animator animator;
    public float maxChargeTime = 3f; // Tempo massimo di carica in secondi
    public float maxDamage = 30f; // Danno massimo dell'attacco
    private float chargeTime;
    private bool isCharging;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && !isCharging)
        {
            isCharging = true;
            chargeTime = 0f;
            animator.Play(chargeAnimation.name);
        }

        if (Input.GetKey(KeyCode.X) && isCharging)
        {
            chargeTime += Time.deltaTime;

            if (chargeTime > maxChargeTime)
            {
                chargeTime = maxChargeTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.X) && isCharging)
        {
            float chargeRatio = chargeTime / maxChargeTime;
            float damage = maxDamage * chargeRatio;
            Debug.Log("Charge ratio: " + chargeRatio + ", Damage: " + damage);
            animator.Play(attackAnimation.name);
            isCharging = false;
        }
    }
}
