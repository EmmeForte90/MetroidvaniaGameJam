using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxPlayer : MonoBehaviour
{
    [SerializeField] GameObject Clang;
    [SerializeField] GameObject Sdeng;
    [SerializeField] public Transform Pos;
    [SerializeField] public int attackDamage = 10;
    private Move Player;
    public bool pesante;
    public bool normal;

private void Awake()
    {
        Player = GetComponentInParent<Move>();

    }

void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Enemy")
        //Se il proiettile tocca il nemico
        {       

            Instantiate(Sdeng, Pos.transform.position, transform.rotation);
            IDamegable hit = other.GetComponent<IDamegable>();
            hit.Damage(Player.Damage);
            //Debug.Log("Damage:" + Player.Damage);

        }
        if(other.gameObject.tag == "Bound")
        //Se il proiettile tocca il nemico
        {            
            Instantiate(Clang, Pos.transform.position, transform.rotation);
        }
        }
        

        
}