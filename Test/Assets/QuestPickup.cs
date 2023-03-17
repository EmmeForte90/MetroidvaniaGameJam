using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPickup : MonoBehaviour
{
    public Quests quest;
    //public Transform QuestContent;
    //public GameObject InventoryQuest;
//public static QuestPickup instance;



    public void Pickup()
    {
        //GameObject obj = Instantiate(InventoryQuest, QuestContent);
        QuestManager.Instance.Add(quest);
        QuestManager.Instance.ListQuest();
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
        }
    }
}