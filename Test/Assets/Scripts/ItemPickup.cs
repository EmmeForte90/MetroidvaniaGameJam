using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    [SerializeField] GameObject VFX;
    public int Value = 1;
    private int IDItem;

public void Awake()
    {
            IDItem = Item.id;
    }
    public void Pickup()
    {
        
        InventoryManager.Instance.AddItem(Item);
        InventoryManager.Instance.ListItem(IDItem);

        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        Pickup();
        Instantiate(VFX, transform.position, transform.rotation);

        }
    }



}
