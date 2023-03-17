using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="New Item", menuName = "Item/Create New Item")]

public class Item : ScriptableObject
{
public int id;
public string itemName;
public string Description;
public int value;
public Sprite icon;
  
}

