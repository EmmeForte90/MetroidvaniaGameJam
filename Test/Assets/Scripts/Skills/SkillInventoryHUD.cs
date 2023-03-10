using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventoryHUD : MonoBehaviour
{
     //public Skill Skill;

   public static SkillInventoryHUD Instance;
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

}



