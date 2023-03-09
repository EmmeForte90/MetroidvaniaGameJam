using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventoryHUD : MonoBehaviour
{
     public static SkillInventoryHUD Instance;
   public List<Skill> abil = new List<Skill>();

   public Transform SkillContent;
   public GameObject InventorySkill;

   private void Awake()
   {
    Instance = this;
   }

   public void Add(Skill ab)
   {
    abil.Add(ab);
   }

   public void Remove(Skill ab)
   {
     abil.Remove(ab);
   }

   public void Listabil()
   {
      foreach (Transform child in SkillContent)
      {
         Destroy(child.gameObject);
      }

      foreach (var ab in abil)
      {
         GameObject obj = Instantiate(InventorySkill, SkillContent);
         var abIcon = obj.transform.Find("skill_icon").GetComponent<Image>();

         abIcon.sprite = ab.icon;
      }
   }
}



