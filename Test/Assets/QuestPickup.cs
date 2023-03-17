using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPickup : MonoBehaviour
{
    public Quests quest;

    public void Pickup()
    {
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
