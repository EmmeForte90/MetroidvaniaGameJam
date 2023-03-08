using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Skill", menuName = "Skill/Create New Skill")]

public class Skill : ScriptableObject
{
public int id;
public string SkillName;
public string Description;
public int value;
public Sprite icon;
  
}

