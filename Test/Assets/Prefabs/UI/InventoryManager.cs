using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
   public static InventoryManager Instance;
   //public List<Item> Items = new List<Item>();

   public Transform ItemContent;
   public GameObject InventoryItem;
   public TextMeshProUGUI itemDes;
   public Image itemPre;

public bool KeyN = false;
[SerializeField] GameObject  Key_N;
public bool Lingotto = false;
[SerializeField] GameObject  Lingotto_N;
public bool Seme = false;
[SerializeField] GameObject  Seme_N;


private void Awake()
   {
    Instance = this;   
    
   }

private void Update()
{
   if(KeyN)
   {
      Key_N.gameObject.SetActive(true);
   }
if(Lingotto)
   {
      Lingotto_N.gameObject.SetActive(true);
   }

if(Seme)
   {
      Seme_N.gameObject.SetActive(true);
   }

}
}

