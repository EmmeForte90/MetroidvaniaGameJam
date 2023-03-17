using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
   public static QuestManager Instance;
   public List<Quests> Quest = new List<Quests>();

   public Transform QuestContent;
   public GameObject InventoryQuest;

   private void Awake()
   {
    Instance = this;
   }


   public void Add(Quests quest)
   {
    Quest.Add(quest);
   }

   public void Remove(Quests quest)
   {
     Quest.Remove(quest);
   }

   public void ListQuest()
   {
      foreach (Transform child in QuestContent)
      {
         Destroy(child.gameObject);
      }

      foreach (var quest in Quest)
      {
         GameObject obj = Instantiate(InventoryQuest, QuestContent);
         //var questIcon = obj.transform.Find("Icon_quest").GetComponent<Image>();

          var questName = obj.transform.Find("Title_quest").GetComponent<TextMeshProUGUI>();

       //  questIcon.sprite = quest.icon;
       questName.text = quest.questName;


      }
   }
}

