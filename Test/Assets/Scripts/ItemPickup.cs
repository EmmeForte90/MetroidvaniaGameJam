using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
     

    void Pickup()
    {
        InventoryManager.Instance.Add(Item);
        InventoryManager.Instance.ListItems();
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        Pickup();
        }
    }



}
