using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{    
    // Riferimento al contenitore dei pulsanti delle quest
    public Transform QuestContent;
   public GameObject InventoryQuest;
   [HideInInspector] public int qID;
    // Scriptable Object delle quest
public List<Quests> questDatabase;

    // Riferimenti ai componenti delle immagini di preview e delle descrizioni
    public Image previewImages;
    public TextMeshProUGUI descriptions;


    // Array di booleani che mantengono lo stato delle quest
    public bool[] quest;
    public bool[] _QuestComplete;

     // Id unico del bottone della quest selezionata
    private static int uniqueId;
//public TextMeshProUGUI Title;
//public TextMeshProUGUI QuestsDescriptionText;
//public Image icon;

// Singleton pattern per accedere a QuestManager da altre classi
public static QuestManager Instance;

private void Awake()
{
    Instance = this;
}


   

     // Metodo per aggiungere una nuova quest al database
    public void AddQuest(Quests newQuest)
{
    questDatabase.Add(newQuest);
}

public void ListQuest(int questId)
{
    // Cerca la quest con l'id specificato
    Quests quest = questDatabase.Find(q => q.id == questId);

    if (quest != null)
    {
        // Istanzia il prefab del bottone della quest nella lista UI
        GameObject obj = Instantiate(InventoryQuest, QuestContent);

        // Recupera il riferimento al componente del titolo della quest e del bottone
        var questName = obj.transform.Find("Title_quest").GetComponent<TextMeshProUGUI>();

        // Assegna l'id univoco al game object istanziato
        obj.name = "QuestButton_" + quest.id;

        // Assegna il nome della quest al componente del titolo
        questName.text = quest.questName;

        // Assegna i valori desiderati ai componenti dell'immagine di preview e della descrizione del pulsante della quest
        previewImages.sprite = quest.Bigicon;
        descriptions.text = quest.Description;

        // Aggiungi un listener per il click del bottone
        var button = obj.GetComponent<Button>();
        button.onClick.AddListener(() => OnQuestButtonClicked(quest.id, previewImages, descriptions));
    }
}

public void OnQuestButtonClicked(int questId, Image previewImages, TextMeshProUGUI descriptions)
{
    if (questId >= 0 && questId < questDatabase.Count)
    {
        // Qui puoi fare qualcosa quando il pulsante della quest viene cliccato, ad esempio aprire una finestra con i dettagli della quest
        // Assegna i valori desiderati ai componenti dell'immagine di preview e della descrizione
        previewImages.sprite = questDatabase.Find(q => q.id == questId).Bigicon;
        descriptions.text = questDatabase.Find(q => q.id == questId).Description;
    }
}





// Metodo per iniziare una quest
// id: l'indice della quest nell'array
public void QuestStart(int id)
{
    // Imposta lo stato della quest a true
    quest[id] = true;   
}
// Metodo per completare una quest
// id: l'indice della quest nell'array
public void QuestComplete(int id)
{
    // Imposta lo stato di completamento della quest a true
    _QuestComplete[id] = true;  
}

}






