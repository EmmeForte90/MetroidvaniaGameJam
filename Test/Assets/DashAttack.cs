using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack : MonoBehaviour
{
    [SerializeField] int damage = 50;
    [SerializeField] float lifeTime = 0.5f;

    [Header("Audio")]
    [SerializeField] AudioSource SExp;
    [SerializeField] AudioSource SBomb;

    public void Start()
    {
        Move.instance.attackDash();
        Invoke("Destroy", lifeTime);

    } 
    
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
