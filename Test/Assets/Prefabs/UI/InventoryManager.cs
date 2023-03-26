using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    
    public bool[] itemActive;

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
        //var questName = obj.transform.Find("Name_Item").GetComponent<TextMeshProUGUI>();
        var Itemimg = obj.transform.Find("Icon_item").GetComponent<Image>();

        // Assegna l'id univoco al game object istanziato
        obj.name = "ItemButton_" + item.id;

        // Assegna il nome della item al componente del titolo
        //questName.text = item.itemName;
        Itemimg.sprite =  item.icon;
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
    if (itemId >= 0)
    {
        // Qui puoi fare qualcosa quando il pulsante della item viene cliccato, ad esempio aprire una finestra con i dettagli della item
        // Assegna i valori desiderati ai componenti dell'immagine di preview e della descrizione
        previewImages.sprite = itemDatabase.Find(q => q.id == itemId).icon;
        descriptions.text = itemDatabase.Find(q => q.id == itemId).Description;
    }
}


private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}
public void ItemActive(int id)
{
    // Imposta lo stato di completamento della quest a true
    itemActive[id] = true;  
}
// Questo metodo viene chiamato quando una nuova scena viene caricata
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // Cerca tutti gli oggetti con il tag "Item" nella scena appena caricata
    GameObject[] collectibleItem = GameObject.FindGameObjectsWithTag("Item");

    // Cicla su tutti gli oggetti trovati
    foreach (GameObject Item in collectibleItem)
    {
        // Cerca il componente "ItemPickup" collegato all'oggetto
        ItemPickup ItemTake = Item.GetComponent<ItemPickup>();

        // Se l'oggetto ha il componente "ItemPickup"
        if (ItemTake != null)
        {
            // Recupera l'identificatore dell'oggetto
            int ItemId = ItemTake.Item.id;

            // Verifica se l'oggetto è già stato raccolto
            for (int i = 0; i < itemActive.Length; i++)
            {
                if (itemActive[i] && i == ItemId)
                {
                    // Se l'oggetto è stato raccolto, imposta il suo stato "isCollected" su true
                    ItemTake.isCollected = true;
                    break;
                }
            }
        }
    }
}




}

