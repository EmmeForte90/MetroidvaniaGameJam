using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class buttontest : MonoBehaviour, IPointerClickHandler
{
    public Skill Skill;
    public TextMeshProUGUI SkillDescriptionText;
    
    

void Awake()
    { 
        // Cerca l'oggetto chiamato "Skilldes"
    //GameObject skillDesObject = GameObject.Find("Skilldes");

    // Recupera il componente TextMeshProUGUI dell'oggetto
   // TextMeshProUGUI SkillDescriptionText = skillDesObject.GetComponent<TextMeshProUGUI>();
    }

 public void Add(Skill ab)
   {
    Skill = ab;
   }

    public void MyFunction()
    { 
        // Inserisci qui il codice che vuoi eseguire quando l'immagine viene cliccata
         SkillDescriptionText.text = Skill.Description;
         // Imposta l'icona dell'abilità
      

       // var abIcon = obj.transform.Find("skill_icon").GetComponent<Image>();
        //abIcon.sprite = ab.icon;
         
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MyFunction();
    }
}

// Nella finestra di ispezione, trascina l'immagine "MyImage" sul campo "gameObject" dello script "MyScript"
// Quindi, seleziona "MyFunction" nella finestra "Metodo" del componente "EventTrigger" di "MyImage"

