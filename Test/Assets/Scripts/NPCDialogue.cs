using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class NPCDialogue : MonoBehaviour
{
   public Transform player; // Reference to the player's position
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI component
    public GameObject Button;
    public GameObject dialogueBox;
    [SerializeField] private string[] dialogue; // array of string to store the dialogues
    public float dialogueDuration; // variable to set the duration of the dialogue
    private int dialogueIndex; // variable to keep track of the dialogue status
    private float elapsedTime; // variable to keep track of the elapsed time
    private bool talk;
    private Animator anim; // componente Animator del personaggio
// Keycode for activating dialogue
    public KeyCode dialogueActivationKey = KeyCode.E;

    public bool isInteragible;

    void Start()
    {
        //player = GameObject.FindWithTag("Player");
        dialogueText.gameObject.SetActive(false); // Initially hide the dialogue text
        dialogueBox.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
        anim = GetComponent<Animator>();

    }
void Update()
    {
        anim.SetBool("talk", talk);
        FacePlayer();
       
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.gameObject.SetActive(true); // Hide dialogue text when player exits the trigger

             if (isInteragible){
         // Check for player input to activate dialogue
        if (Input.GetKeyDown(dialogueActivationKey))
        {
            StartCoroutine(ShowDialogue());
        }
        }else if (!isInteragible)
        {
            
            StartCoroutine(ShowDialogue());
            talk = true;
        }
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Button.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
            StopCoroutine(ShowDialogue());
            dialogueIndex++; // Increment the dialogue index
            if (dialogueIndex >= dialogue.Length)
            {
                dialogueIndex = 0;
            }
            talk = false;
            dialogueBox.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
            dialogueText.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
        }
    }

    
    IEnumerator ShowDialogue()
    {
        elapsedTime = 0; // reset elapsed time
        dialogueBox.gameObject.SetActive(true); // Hide dialogue text when player exits the trigger
        dialogueText.gameObject.SetActive(true); // Show dialogue text when player enters the trigger
        string currentDialogue = dialogue[dialogueIndex]; // Get the current dialogue
        dialogueText.text = ""; // Clear the dialogue text

        
        for (int i = 0; i < currentDialogue.Length; i++)
        {
            dialogueText.text += currentDialogue[i]; // Add one letter at a time
            elapsedTime += Time.deltaTime; // Update the elapsed time
            if(elapsedTime >= dialogueDuration)
            {
                break;
            }
            yield return new WaitForSeconds(0.05f); // Wait before showing the next letter
        }
        //dialogueText.gameObject.SetActive(false); // Hide dialogue text
        elapsedTime = 0; // reset elapsed time
        
    }


    void FacePlayer()
{
    if (player != null)
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
}