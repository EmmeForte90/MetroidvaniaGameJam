using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
   public static InventoryManager Instance;
   public List<Item> Items = new List<Item>();

   public Transform ItemContent;
   public GameObject InventoryItem;

   private void Awake()
   {
    Instance = this;
   }

   public void Add(Item item)
   {
    Items.Add(item);
   }

   public void Remove(Item item)
   {
     Items.Remove(item);
   }

   public void ListItems()
   {
      foreach (Transform child in ItemContent)
      {
         Destroy(child.gameObject);
      }

      foreach (var item in Items)
      {
         GameObject obj = Instantiate(InventoryItem, ItemContent);
         var itemIcon = obj.transform.Find("Item_icon").GetComponent<Image>();

         itemIcon.sprite = item.icon;
      }
   }
}

