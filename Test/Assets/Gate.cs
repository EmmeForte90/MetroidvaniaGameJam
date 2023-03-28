using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Gate : MonoBehaviour
{

public GameObject GateS;
public GameObject Box;
public GameObject Button;
public GameObject[] inter;
public Item TypesKey;
private bool _isInTrigger;
private bool StopButton = false; // o la variabile che deve attivare la sostituzione
private bool _isDialogueActive;
public string request;
public string NotK;
public TextMeshProUGUI descriptions;

void Awake()
    {
        descriptions.text = request;
    }

void Update()
    {

        if (_isInTrigger && Input.GetButtonDown("Talk") && !_isDialogueActive)
        {
            Move.instance.stopInput = true;
            Move.instance.Stop();
            Move.instance.Stooping();
            Box.gameObject.SetActive(true);

        }
        else if (_isDialogueActive && Input.GetButtonDown("Talk") && StopButton)
        {

            StopButton = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
         Button.gameObject.SetActive(true);
        _isInTrigger = true;
        
        }
    }

public void Accepted()
    {
            Move.instance.stopInput = false;
            if(InventoryManager.Instance.itemDatabase.Find(q => q.id == TypesKey.id))
            {
                print("Hai aperto");
            InventoryManager.Instance.RemoveItem(TypesKey);
            Box.gameObject.SetActive(false);
            Button.gameObject.SetActive(false); 

            }else
            {
                    print("niente da fare");
                 StartCoroutine(CloseBox());
            }
            
    }

IEnumerator CloseBox()
    {
    descriptions.text = NotK;
    foreach (GameObject But in inter)
        {
            But.SetActive(false);
            
        }
    yield return new WaitForSeconds(2);
    Move.instance.stopInput = false;
    Box.gameObject.SetActive(false);
    Button.gameObject.SetActive(false); // Initially hide the dialogue text   
    descriptions.text = request;
    foreach (GameObject But in inter)
        {
            But.SetActive(true);
            
        }
    }

public void Denied()
    {
            Move.instance.stopInput = false;
            Box.gameObject.SetActive(false);
            Button.gameObject.SetActive(false); // Initially hide the dialogue text   
            descriptions.text = request;
    
    }


 private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.gameObject.SetActive(false); // Initially hide the dialogue text
            _isInTrigger = false;
            Box.gameObject.SetActive(false);
            descriptions.text = request;

        }
    }











}

