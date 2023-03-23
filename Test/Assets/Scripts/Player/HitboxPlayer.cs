using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPlayer : MonoBehaviour
{
    //[SerializeField] GameObject Clang;
    [SerializeField] public Transform Pos;
    [SerializeField] public int attackDamage = 10;
    public bool pesante;
    public bool normal;
    [SerializeField] AudioSource SClang;

public static HitboxPlayer Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Enemy")
        //Se il proiettile tocca il nemico
        {       

            IDamegable hit = other.GetComponent<IDamegable>();
            hit.Damage(Move.instance.Damage);
            //Debug.Log("Damage:" + Player.Damage);
            if(Move.instance.rb.velocity.y > 0)
            {               
                Move.instance.isBump = true;
                Move.instance.Bump();
            }

        }
         if(other.gameObject.tag == "Hitbox_E")
        //Se il proiettile tocca il nemico
        {       
            SClang.Play();
                            
            Move.instance.Knockback();

            if(Move.instance.rb.velocity.y > 0)
            {
                Move.instance.isBump = true;
                Move.instance.Bump();
            }

        }
       /* if(other.gameObject.tag == "Bound")
        //Se il proiettile tocca il nemico
        {            
            Instantiate(Clang, Pos.transform.position, transform.rotation);
            if(Move.instance.rb.velocity.y > 0)
            {
                Move.instance.Bump();
            }
        }
        }
        
*/
    }    
}