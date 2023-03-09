using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomButton : MonoBehaviour
{
    public Sprite regular;
    public Sprite mouseOver;
    public Sprite mouseClicked;
    public TextMeshPro buttonText;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    private void OnMouseDown()
    {
Debug.Log("premuto");

    }

    private void OnMouseEnter()
    {
Debug.Log("entrato");

    }

    private void OnMouseExit()
    {
Debug.Log("uscito");

    }

    private void OnMouseUpAsButton()
    {
Debug.Log("dopra credo");

    }
}