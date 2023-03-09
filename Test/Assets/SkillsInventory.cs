using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillsInventory : MonoBehaviour
{        
   //public Skill Skill;

   public static SkillsInventory Instance;
   // public TextMeshProUGUI SkillDescriptionText;
   // public Image imageSkill;
   //public List<Skill> abil = new List<Skill>();

public bool IsGlobe = false;
[SerializeField] GameObject  GlobeSkill;
public bool IsDashAtk = false;
[SerializeField] GameObject  DashSkill;
public bool IsSlashSword = false;
[SerializeField] GameObject  slashsk;


private void Awake()
   {
    Instance = this;   
    
   }

private void Update()
{
   if(IsGlobe)
   {
      GlobeSkill.gameObject.SetActive(true);
   }
if(IsDashAtk)
   {
      DashSkill.gameObject.SetActive(true);
   }

if(IsSlashSword)
   {
      slashsk.gameObject.SetActive(true);
   }
}


/*

    public static SkillsInventory Instance;
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
   }*/
}



