using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName ="New Quest", menuName = "Quest/Create New Quest")]
public class Quests : ScriptableObject
{
public int id;
public string questName;
public string Description;
public int value;
//public Sprite icon;
public Sprite Bigicon;


}
