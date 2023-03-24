using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
   public static InventoryManager Instance;
   //public List<Item> Items = new List<Item>();

   // Riferimento al contenitore dei pulsanti delle item
    public Transform ItemContent;
   public GameObject InventoryItem;
   [HideInInspector] public int qID;
    // Scriptable Object delle item
   public List<Item> itemDatabase;

    // Riferimenti ai componenti delle immagini di preview e delle descrizioni
    public Image previewImages;
    public TextMeshProUGUI descriptions;
    public TextMeshProUGUI Num;
    public TextMeshProUGUI NameItems;



     // Id unico del bottone della item selezionata
    private static int uniqueId;
//public TextMeshProUGUI Title;
//public TextMeshProUGUI QuestsDescriptionText;
//public Image icon;



private void Awake()
{
    Instance = this;
}


   

     // Metodo per aggiungere una nuova item al database
    public void AddItem(Item newItem)
{
    itemDatabase.Add(newItem);
}

public void ListItem(int itemId)
{
    // Cerca la item con l'id specificato
    Item item = itemDatabase.Find(q => q.id == itemId);

    if (item != null)
    {
        // Istanzia il prefab del bottone della item nella lista UI
        GameObject obj = Instantiate(InventoryItem, ItemContent);

        // Recupera il riferimento al componente del titolo della item e del bottone
        var questName = obj.transform.Find("Title_quest").GetComponent<TextMeshProUGUI>();

        // Assegna l'id univoco al game object istanziato
        obj.name = "QuestButton_" + item.id;

        // Assegna il nome della item al componente del titolo
        questName.text = item.itemName;

        // Assegna i valori desiderati ai componenti dell'immagine di preview e della descrizione del pulsante della item
        previewImages.sprite = item.icon;
        descriptions.text = item.Description;
        Num.text = item.value.ToString(); 
        NameItems.text = item.itemName;
        // Aggiungi un listener per il click del bottone
        var button = obj.GetComponent<Button>();
        button.onClick.AddListener(() => OnQuestButtonClicked(item.id, previewImages, descriptions));
    }
}

public void OnQuestButtonClicked(int itemId, Image previewImages, TextMeshProUGUI descriptions)
{
    if (itemId >= 0 && itemId < itemDatabase.Count)
    {
        // Qui puoi fare qualcosa quando il pulsante della item viene cliccato, ad esempio aprire una finestra con i dettagli della item
        // Assegna i valori desiderati ai componenti dell'immagine di preview e della descrizione
        previewImages.sprite = itemDatabase.Find(q => q.id == itemId).icon;
        descriptions.text = itemDatabase.Find(q => q.id == itemId).Description;
    }
}
}

