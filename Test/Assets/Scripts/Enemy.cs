using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform pointA, pointB;
    public float moveSpeed = 2f; // velocità di movimento
    public float chaseSpeed = 4f; // velocità di inseguimento
    public float attackDamage = 10f; // danno d'attacco
    public float sightRadius = 5f; // raggio di vista del nemico
    public float chaseThreshold = 2f; // soglia di distanza per iniziare l'inseguimento
    public float attackrange = 2f;
    private bool movingToA = false;

    private bool isAttacking = false;


    private Transform player; // riferimento al player
    private bool isChasing = false; // indica se il nemico sta inseguendo il player
    private bool isMove = false;

    private Animator animator; // riferimento all'animatore
    private Health health;
    private Vector2 knockbackDirection; // direzione del knockback


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // trova l'oggetto con tag "Player"
        animator = GetComponent<Animator>(); // ottiene l'animatore dall'oggetto
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (isChasing) // se il nemico sta inseguendo il player
        {
            //  // insegui il player
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            animator.SetBool("IsChasing", true); // imposta la variabile booleana "IsChasing" dell'animatore a true
            animator.SetBool("isMove", false);
        }
        else // altrimenti
        {
            MoveToPoint();
            animator.SetBool("IsChasing", false); // imposta la variabile booleana "IsChasing" dell'animatore a false
            isMove = true;
            animator.SetBool("isMove", true);

        }

        // se il player è nelle vicinanze del nemico
        if (Vector2.Distance(transform.position, player.position) < chaseThreshold)
        {
            isChasing = true; // inizia a inseguire il player
            isAttacking = false;
            if(player.transform.position.x > transform.position.x)
            {
                        transform.localScale = new Vector2(1f, 1f);
            }else if(player.transform.position.x < transform.position.x)
            {
                        transform.localScale = new Vector2(-1f, 1f);
            }


        } else
        if (Vector2.Distance(transform.position, player.position) > chaseThreshold)
        {
            isChasing = false; // inizia a inseguire il player
        } 

if (Vector2.Distance(transform.position, player.position) < attackrange)
        {
            isChasing = true; // inizia a inseguire il player
            isAttacking = true;
            transform.localScale = new Vector2(0f, 0f);

            if(player.transform.position.x > transform.position.x)
            {
                        transform.localScale = new Vector2(1f, 1f);
            }else if(player.transform.position.x < transform.position.x)
            {
                        transform.localScale = new Vector2(-1f, 1f);
            }
            animator.SetTrigger("attack");

        } 





    }

    public void TakeDamage(float damage)
    {
        health.currentHealth -= damage; // sottrai danno dalla salute
        animator.SetTrigger("TakeDamage"); // attiva il trigger "TakeDamage" dell'animatore

        if (health.currentHealth <= 0f) // se la salute è uguale o inferiore a 0
        {
            Die();
        }
    }

    private void MoveToPoint()
    {
        if (movingToA) {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, moveSpeed * Time.deltaTime);
            if (transform.position == pointA.position) {
                movingToA = false;


            }
        } else {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeed * Time.deltaTime);
            if (transform.position == pointB.position) {
                movingToA = true;


            }
        }
        if (movingToA)
        {
            
        transform.localScale = new Vector2(-1f, 1f);
        }
        else if (!movingToA)
        {
            
        transform.localScale = new Vector2(1f, 1f);
        }
    }
private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, chaseThreshold);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, attackrange);
        //Debug.DrawRay(transform.position, new Vector3(chaseThreshold, 0), Color.red);
    }

    public void Die()
    {
        // determina la direzione della morte in base alla posizione del player rispetto al nemico
        if (transform.position.x < player.position.x)
        {
            animator.SetTrigger("Die"); // attiva il trigger "DieFront" dell'animatore per la morte davanti al player
        }
        else
        {
            animator.SetTrigger("Die_1"); // attiva il trigger "DieBack" dell'animatore per la morte dietro al player
        }
        ObjectPooler.Instance.SpawnFromPool("EnemyDeathEffect", transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}