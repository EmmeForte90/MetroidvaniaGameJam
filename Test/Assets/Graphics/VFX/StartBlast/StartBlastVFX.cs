using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlastVFX : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private float bulletSpeed = 1f;
    //Variabile della velocità del proiettile
    Rigidbody2D rb;
    //Il corpo rigido
    //Per permettere al proiettile di emularne l'andamento
    float xSpeed;
    //L'andatura
    public bool charge = false;
    public bool heal = false;

    [Header("Tempo di esplosione")]
    [SerializeField] public float lifeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //Recupera i componenti del rigidbody
        //Recupera i componenti dello script
        if(!Move.instance.isCharging && !Move.instance.isHeal)
        {
        xSpeed = Move.instance.transform.localScale.x;
        }
        //La variabile è uguale alla scala moltiplicata la velocità del proiettile
        //Se il player si gira  anche lo spawn del proittile farà lo stesso
    }

#region Update
    void Update()
    {
         rb.velocity = new Vector2 (xSpeed, 0f);
        //La velocità e la direzione del proiettile
        FlipSprite();
        if(charge)
        {
        if(!Move.instance.isCharging)
        {
            Destroy(gameObject, lifeTime);
        }
        } else if(heal)
        {
        if(!Move.instance.isHeal)
        {
            Destroy(gameObject, lifeTime);
        }
        }
    }
#endregion

#region  FlipSprite
    void FlipSprite()
    {
        bool bulletHorSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        //se il player si sta muovendo le sue coordinate x sono maggiori di quelle e
        //di un valore inferiore a 0

        if (bulletHorSpeed) //Se il player si sta muovendo
        {
            transform.localScale = new Vector2 (Mathf.Sign(rb.velocity.x), 1f);
            //La scala assume un nuovo vettore e il rigidbody sull'asse x 
            //viene modificato mentre quello sull'asse y no. 
        }
        
    }

#endregion


}
