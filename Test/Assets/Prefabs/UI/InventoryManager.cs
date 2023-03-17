using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
   public static InventoryManager Instance;
   public List<Item> Items = new List<Item>();

   public Transform ItemContent;
   public GameObject InventoryItem;
   public TextMeshProUGUI itemDes;
   public Image itemPre;

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

         itemPre.sprite = item.icon;
         itemDes.text = item.Description;
         itemIcon.sprite = item.icon;
      }
   }
}

