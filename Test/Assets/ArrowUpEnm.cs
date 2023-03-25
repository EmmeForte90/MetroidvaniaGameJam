using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUpEnm : MonoBehaviour
{
     public float initialSpeed = 10f;
public float gravity = 9.8f;
    public float attackDamage = 10; // danno d'attacco

private Rigidbody2D rb;
private Vector2 startPosition;
private float startTime;
private float lifeTime = 0.5f;
[SerializeField] GameObject Explode;
private Vector2 initialDirection;
    private Transform player;

private void Start()
{        player = GameObject.FindWithTag("Player").transform;

    rb = GetComponent<Rigidbody2D>();
    startPosition = transform.position;
    startTime = Time.time;
     if(AiEnemycrossbow.instance.transform.localScale.x > 0)
        {
        initialDirection = Vector2.right;
        } 
        else if(AiEnemycrossbow.instance.transform.localScale.x < 0)
        {
        initialDirection = Vector2.left;
        }
    
}

private void FixedUpdate()
{
    float elapsedTime = Time.time - startTime;
    float verticalDisplacement = initialSpeed * elapsedTime * Mathf.Sin(45) - 0.5f * gravity * elapsedTime * elapsedTime;
    float horizontalDisplacement = initialSpeed * elapsedTime * Mathf.Cos(45);

    transform.position = startPosition + initialDirection * horizontalDisplacement + Vector2.up * verticalDisplacement;

    float angle = Mathf.Atan2(verticalDisplacement, horizontalDisplacement * initialDirection.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
}



void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {  
            Instantiate(Explode, transform.position, transform.rotation);
            if (!Move.instance.isDeath)
            {
                if (!Move.instance.isHurt)
            {
            player.GetComponent<PlayerHealth>().Damage(attackDamage);
            }
            }
            Destroy(gameObject);

        }

        if (other.gameObject.tag == "Ground")
        { 
            Instantiate(Explode, transform.position, transform.rotation);
        Destroy(gameObject);
        }
       
    }
}