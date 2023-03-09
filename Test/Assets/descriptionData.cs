using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class descriptionData : MonoBehaviour
{
   public Skill Skill;

   public static descriptionData Instance;
    public TextMeshProUGUI SkillDescriptionText;
   //public Image icon;
public Image icon;

    private void Awake()
   {
    Instance = this;   
    
   }
public void DescriptionSkill()
    { 
        // Inserisci qui il codice che vuoi eseguire quando l'immagine viene cliccata
         SkillDescriptionText.text = Skill.Description;
         // Imposta l'icona dell'abilità

            icon.sprite = Skill.icon;
       // var abIcon = obj.transform.Find("skill_icon").GetComponent<Image>();
        //abIcon.sprite = ab.icon;
         
    }

}
