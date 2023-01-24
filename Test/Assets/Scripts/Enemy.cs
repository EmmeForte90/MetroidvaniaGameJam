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
Rigidbody2D rb;
public float attackCooldown = 2f; // durata del cooldown dell'attacco
private float attackTimer; // timer per il cooldown dell'attacco
    private bool movingToA = false;

    private bool isAttacking = false;
    [SerializeField] GameObject Death;
    [SerializeField] GameObject DeathBack;
    [SerializeField] GameObject Brain;
    private GameplayManager gM;


    private Transform player; // riferimento al player
    private bool isChasing = false; // indica se il nemico sta inseguendo il player
    private bool isMove = false;

    private Animator animator; // riferimento all'animatore
    private Health health;

    [Header("Knockback")]
    private bool kb = false;
    public float knockbackForce; // la forza del knockback
    public float knockbackTime; // il tempo di knockback
    public float jumpHeight; // l'altezza del salto
    public float fallTime; // il tempo di caduta


    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform; // trova l'oggetto con tag "Player"
        animator = GetComponent<Animator>(); // ottiene l'animatore dall'oggetto
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>(); // prende il rigidbody del nemico
        if (gM == null)
        {
            gM = GameObject.FindObjectOfType<GameplayManager>();
        }
    }


    void Update()
    {
        if(!gM.PauseStop)
        {
        if(!kb)
        {
        if (isChasing) // se il nemico sta inseguendo il player
        {
            Chase();
        }
        else // altrimenti
        {
            NoChase();
        }

        //
        if (movingToA)
        {
            
        transform.localScale = new Vector2(-1f, 1f);
        }
        else if (!movingToA)
        {
            
        //    
        transform.localScale = new Vector2(1f, 1f);
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


        } else if (Vector2.Distance(transform.position, player.position) > chaseThreshold)
        {
            isChasing = false; // Smette di inseguire il player
        } 

if (Vector2.Distance(transform.position, player.position) < attackrange)
        {
            isChasing = false; 
            isAttacking = true; //inizia ad attaccare il player

            if(player.transform.position.x > transform.position.x)
            {
                        transform.localScale = new Vector2(1f, 1f);
            }else if(player.transform.position.x < transform.position.x)
            {
                        transform.localScale = new Vector2(-1f, 1f);
            }

if (attackTimer <= 0f) {
            // Esegui la mossa d'attacco
            //animator.SetTrigger("attack");
            animator.SetBool("attacking", isAttacking);
            Stop();
            attackTimer = attackCooldown;
        } else {
            attackTimer -= Time.deltaTime;
        }
        } else if (Vector2.Distance(transform.position, player.position) < attackrange)
        {
            isAttacking = false;
        }


        }
        }
    }
    public void anmHurt()
    {
        animator.SetTrigger("TakeDamage"); // attiva il trigger "TakeDamage" dell'animatore
        if (rb != null)
            {
                StartCoroutine(JumpBackCo(rb)); // avvia la routine per il salto
            }
        if (health.currentHealth <= 0f) // se la salute è uguale o inferiore a 0
        {
            Die();
        }
    }

#region Movimento
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
        
    }
public void  Stop()
{
rb.velocity = new Vector3(0, 0, 0);
}
    #endregion
public void  Chase()
{
    //  // insegui il player
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            animator.SetBool("IsChasing", true); // imposta la variabile booleana "IsChasing" dell'animatore a true
            animator.SetBool("isMove", false);
}
public void  NoChase()
{
    MoveToPoint();
            animator.SetBool("IsChasing", false); // imposta la variabile booleana "IsChasing" dell'animatore a false
            isMove = true;
            animator.SetBool("isMove", true);
}
    #region Gizmos
private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, chaseThreshold);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, attackrange);
        //Debug.DrawRay(transform.position, new Vector3(chaseThreshold, 0), Color.red);
    }
#endregion


private IEnumerator JumpBackCo(Rigidbody2D rb)
    {
        if (rb != null)
        {
            isChasing = false;
            kb = true;
            Vector2 knockbackDirection = new Vector2(0f, jumpHeight); // direzione del knockback verso l'alto
            if (rb.transform.position.x < player.transform.position.x) // se la posizione x del nemico è inferiore a quella del player
                knockbackDirection = new Vector2(-1, jumpHeight); // la direzione del knockback è verso sinistra
            else if (rb.transform.position.x > player.transform.position.x) // se la posizione x del nemico è maggiore a quella del player
                knockbackDirection = new Vector2(1, jumpHeight); // la direzione del knockback è verso destra
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse); // applica il knockback
            yield return new WaitForSeconds(knockbackTime); // aspetta il tempo di knockback
            kb = false;
            isChasing = true;

        }
    }

#region Die
    public void Die()
    {
        // determina la direzione della morte in base alla posizione del player rispetto al nemico
        if (movingToA)//transform.position.x < player.position.x)
        {
            Instantiate(Death, transform.position, transform.rotation);
            Destroy(Brain);

            //animator.SetTrigger("Die"); // attiva il trigger "DieFront" dell'animatore per la morte davanti al player
        }
        else if (!movingToA)
        {
            Instantiate(DeathBack, transform.position, transform.rotation);
            Destroy(Brain);

            //animator.SetTrigger("Die_1"); // attiva il trigger "DieBack" dell'animatore per la morte dietro al player
        }
        
    }
    #endregion
}