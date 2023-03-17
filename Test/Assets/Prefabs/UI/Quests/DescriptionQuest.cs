using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DescriptionQuest : MonoBehaviour
{    
   public Quests Quests;
   public int DataId;
   private int id;
   public static DescriptionQuest Instance;
    public TextMeshProUGUI Title;
   public TextMeshProUGUI QuestsDescriptionText;
   public Image icon;

    private void Awake()
   {
    Instance = this;   
    id = Quests.id;
    Title.text = Quests.questName;
   }

public void DescriptionQuests()
    { 
        // Inserisci qui il codice che vuoi eseguire quando l'immagine viene cliccata
        QuestsDescriptionText.text = Quests.Description;
        id = Quests.id;
         // Imposta l'icona dell'abilità
        icon.sprite = Quests.Bigicon;
         
    }
}
