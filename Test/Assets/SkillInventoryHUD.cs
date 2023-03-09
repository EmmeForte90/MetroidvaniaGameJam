using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventoryHUD : MonoBehaviour
{
    public static SkillInventoryHUD Instance;
    public List<Skill> abil = new List<Skill>(); // Lista delle abilità

    public Transform SkillContent; // Contenitore delle abilità
    public GameObject InventorySkill; // Prefab dell'abilità nell'inventario

    private void Awake()
    {
        Instance = this; // Imposta l'istanza dell'inventario come quella corrente
    }

    public void Add(Skill ab) // Aggiunge un'abilità all'inventario
    {
        abil.Add(ab);
    }

    public void Remove(Skill ab) // Rimuove un'abilità dall'inventario
    {
        abil.Remove(ab);
    }

    public void Listabil() // Mostra le abilità nell'inventario
    {
        // Elimina le abilità precedenti nell'inventario
        foreach (Transform child in SkillContent)
        {
            Destroy(child.gameObject);
        }

        // Aggiunge le nuove abilità all'inventario
        foreach (var ab in abil)
        {
            // Istanzia un nuovo oggetto dell'abilità nell'inventario
            GameObject obj = Instantiate(InventorySkill, SkillContent);

            // Imposta l'icona dell'abilità
            var abIcon = obj.transform.Find("skill_icon").GetComponent<Image>();
            abIcon.sprite = ab.icon;

            // Imposta il testo dell'abilità
            // Nota: Questa parte è commentata poiché il testo dell'abilità non viene utilizzato
            //var abText = obj.transform.Find("skill_text").GetComponent<Text>();
            //abText = ab.Description;
        }
    }
}



