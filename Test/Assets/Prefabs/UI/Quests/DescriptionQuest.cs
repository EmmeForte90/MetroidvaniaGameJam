using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DescriptionQuest : MonoBehaviour
{    
    Dictionary<int, Quests> QuestMap = new Dictionary<int, Quests>();

   private Quests quests;
   [HideInInspector] public int id;
   public static DescriptionQuest Instance;
   private TextMeshProUGUI QuestDescriptionText;
   private Image icon;

    [HideInInspector]
    public int selectedId = -1; // Id dell'abilità selezionata

    private void Awake()
   {
    Instance = this;   
    icon = GameObject.FindWithTag("Icon").GetComponent<Image>();
    QuestDescriptionText = GameObject.FindWithTag("Description").GetComponent<TextMeshProUGUI>();

   }

void Start()
    {
        // Aggiungi le tue skill alla mappa
        QuestMap.Add(-1, new Quests());//NoQuest
        QuestMap.Add(1, new Quests());
        QuestMap.Add(2, new Quests());
        QuestMap.Add(3, new Quests());


    }

 public void AssignId(int id)
    {
        selectedId = id; // Assegna l'id dell'abilità selezionata
    }

public void DescriptionQ()
    { 
        // Recupera la quest corrispondente all'id selezionato
        Quests selectedQuest = QuestMap[selectedId];

        // Inserisci qui il codice che vuoi eseguire quando l'immagine viene cliccata
        QuestDescriptionText.text = selectedQuest.Description;

         // Imposta l'icona della quest
        icon.sprite = selectedQuest.Bigicon;
         
    }
}
