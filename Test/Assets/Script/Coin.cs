using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Coin : MonoBehaviour
{
    [Header("Data")]
    public GameObject CoinOBJ;
    public GameObject VFXTake;
    public int value;
    private bool take = false;

    [Header("Gravity")]
    public float gravity = 12f;
    private float verticalVelocity = 0f;
    public float initialJumpForce = 3f; // Forza iniziale del salto

    [Header("Componenti")]
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        verticalVelocity = initialJumpForce; // Applica la spinta iniziale
    }

    void Update()
    {
        // Applica la gravità
        verticalVelocity -= gravity * Time.deltaTime;

        // Muove la moneta lungo l'asse Y (verticale)
        Vector3 velocity = new Vector3(0, verticalVelocity, 0);
        characterController.Move(velocity * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Take();
        }
    }

    public void Take()
    {
        if (!take)
        {
            GameManager.instance.Money += value;
            take = true;
            Instantiate(VFXTake, CoinOBJ.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    
}
