using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    [SerializeField] GameObject VFX;
public int Value = 1;

public bool KeyN = false;
public bool Lingotto = false;
public bool Seme = false;
    public void Pickup()
    {
        
        if(KeyN)
        {InventoryManager.Instance.KeyN = true;}
        else if(Lingotto)
        {InventoryManager.Instance.Lingotto = true;}
        else if(Seme)
        {InventoryManager.Instance.Seme = true;}

        Destroy(gameObject);
    }
    private void OnEnterExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        Pickup();
        Instantiate(VFX, transform.position, transform.rotation);

        }
    }



}
