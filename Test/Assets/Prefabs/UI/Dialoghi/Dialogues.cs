using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue/Create New Dialogue")]
public class Dialogues : ScriptableObject
{
public int id;
public string CharacterName;
[SerializeField][TextArea(3, 10)]public string[] dialogue;
public int value;
}
