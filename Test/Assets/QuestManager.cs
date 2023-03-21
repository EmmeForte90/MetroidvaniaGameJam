using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{    

// Riferimento al VFX da attivare
//public GameObject questCompleteVFX;

public bool Quest1 = false;
[HideInInspector] public bool QuestComplete1 = false;
[SerializeField] GameObject  Quest_1;
[SerializeField] GameObject  Quest_1_Complete;

public bool Quest2 = false;
[HideInInspector] public bool QuestComplete2 = false;
[SerializeField] GameObject  Quest_2;
[SerializeField] GameObject  Quest_2_Complete;

public bool Quest3 = false;
[HideInInspector] public bool QuestComplete3 = false;
[SerializeField] GameObject  Quest_3;
[SerializeField] GameObject  Quest_3_Complete;

public static QuestManager Instance;


private void Awake()
   {
    Instance = this;   
    
   }

private void Update()
{
   if(Quest1)
   {
      Quest_1.gameObject.SetActive(true);
   }
if(Quest2)
   {
      Quest_2.gameObject.SetActive(true);
   }

if(Quest3)
   {
      Quest_3.gameObject.SetActive(true);
   }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

if(QuestComplete3)
   {
      Quest_3_Complete.gameObject.SetActive(true);
   }
}

 // Metodo per attivare una quest
   /* public void ActivateQuest(string questTitle)
    {
        currentQuest = quests[questTitle];

        // Attiva il pannello delle informazioni sulla quest
        questInfoPanel.SetActive(true);

        // Mostra il titolo e la descrizione della quest corrente
        questTitleText.text = currentQuest.title;
        questDescriptionText.text = currentQuest.description;

        // Attiva il dialogo della quest corrente
        questDialogPanel.SetActive(true);
        questDialogPanel.GetComponent<DialogManager>().StartDialog(currentQuest.dialog);
    }

    // Metodo per completare una quest
    public void CompleteQuest()
    {
        // Attiva il VFX di completamento
        Instantiate(questCompleteVFX, transform.position, Quaternion.identity);

        // Disattiva la quest corrente
        currentQuest.isActive = false;

        // Mostra il pannello di completamento della quest
        questCompletePanel.SetActive(true);

        // Rimuove la quest dal dizionario
        quests.Remove(currentQuest.title);
    }*/
}






