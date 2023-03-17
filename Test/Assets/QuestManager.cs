using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{    
public static QuestManager Instance;

public bool Quest1 = false;
[SerializeField] GameObject  Quest_1;
public bool Quest2 = false;
[SerializeField] GameObject  Quest_2;
public bool Quest3 = false;
[SerializeField] GameObject  Quest_3;


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

}

}






