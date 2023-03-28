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

public class QuestCharacters : MonoBehaviour
{
    public Quests Quest;
    public int IDQuest;
    public int IDCharacter;

    private GameObject player; // Reference to the player's position
    public TextMeshProUGUI dialogueText; // Reference to the TextMeshProUGUI component
    public GameObject button;
    public GameObject dialogueBox;
    public GameObject QuestStart;
    public GameObject QuestEnd;

    public GameObject Reward;
    public Transform RewardPoint;

    private string[] dialogue; // array of string to store the dialogues
    public float dialogueDuration; // variable to set the duration of the dialogue
    private int dialogueIndex; // variable to keep track of the dialogue status
    private float elapsedTime; // variable to keep track of the elapsed time
    private Animator anim; // componente Animator del personaggio
    private bool notGo = false;
    public bool isInteragible;
    public bool heFlip;
    public bool FirstD = true;
    private bool StopButton = false; // o la variabile che deve attivare la sostituzione

    private bool _isInTrigger;
    private bool _isDialogueActive;
[Header("Audio")]
[SerializeField] AudioSource talk;
[SerializeField] AudioSource Clang;


public static QuestCharacters Instance;


void Awake()
{
        Instance = this;   
        player = GameObject.FindWithTag("Player");
        IDQuest = Quest.id;
}


    void Start()
    {        
        button.gameObject.SetActive(false); // Initially hide the dialogue text
        dialogueText.gameObject.SetActive(false); // Initially hide the dialogue text
        dialogueBox.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
        anim = GetComponent<Animator>();
        

    }

    void Update()
    {
        if (FirstD)
{
    dialogue = Quest.Startdialogue;
}
else if (Quest.isActive)
{
    dialogue = Quest.Middledialogue;
}
else if (Quest.isComplete)//EndDia
{
    dialogue = Quest.Endingdialogue;
}
else if (Quest.AfterQuest)
{
    dialogue = Quest.Afterdialogue;
}

        anim.SetBool("talk", _isInTrigger);
        if(heFlip)
        {
        FacePlayer();
        }
if(!notGo)
{
        if (_isInTrigger && Input.GetButtonDown("Talk") && !_isDialogueActive)
        {
            Move.instance.stopInput = true;
            Move.instance.Stop();
            Move.instance.Stooping();
            dialogueIndex = 0;
            StartCoroutine(ShowDialogue());
        }
        else if (_isDialogueActive && Input.GetButtonDown("Talk") && StopButton)
        {
            NextDialogue();
             StopButton = false;
        }
    }
}
    

public void clang()
{
Clang.Play();
}

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        button.gameObject.SetActive(true);
        _isInTrigger = true;
        if (!isInteragible)
        {
            dialogueIndex = 0; // Reset the dialogue index to start from the beginning
            StartCoroutine(ShowDialogue());
        }
    }
}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            button.gameObject.SetActive(false); // Initially hide the dialogue text
            _isInTrigger = false;
            StopCoroutine(ShowDialogue());
            dialogueIndex++; // Increment the dialogue index
            if (dialogueIndex >= dialogue.Length)
            {
                dialogueIndex = 0;
                _isDialogueActive = false;
                dialogueBox.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
                dialogueText.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
                talk.Stop();

            }
        }
    }

    IEnumerator ShowDialogue()
{
    talk.Play();
    _isDialogueActive = true;
    elapsedTime = 0; // reset elapsed time
    dialogueBox.gameObject.SetActive(true); // Show dialogue box
    dialogueText.gameObject.SetActive(true); // Show dialogue text
    string currentDialogue = dialogue[dialogueIndex]; // Get the current dialogue
    dialogueText.text = ""; // Clear the dialogue text
    for (int i = 0; i < currentDialogue.Length; i++)
    {
        dialogueText.text += currentDialogue[i]; // Add one letter at a time
        elapsedTime += Time.deltaTime; // Update the elapsed time
        if (elapsedTime >= dialogueDuration)
        {
            break;
        }
        yield return new WaitForSeconds(0); // Wait before showing the next letter
    }
            dialogueText.text = currentDialogue; // Set the dialogue text to the full current dialogue
            StopButton = true;
}



    void NextDialogue()
    {

        elapsedTime = 0; // reset elapsed time
        dialogueIndex++; // Increment the dialogue index
        if (dialogueIndex >= dialogue.Length)
        {
            talk.Stop();
            dialogueIndex = 0;
            _isDialogueActive = false;
            dialogueBox.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
            dialogueText.gameObject.SetActive(false); // Hide dialogue text when player exits the trigger
            if(FirstD)
            {
            StartCoroutine(StartQuest());
            }
            else if(Quest.isComplete)
            {
            StartCoroutine(EndQuest());
            }
            else
            {
            Move.instance.stopInput = false;
            }

        }
        else
        {
            StartCoroutine(ShowDialogue());
        }
    }



 IEnumerator EndQuest()
{
        notGo = true;
        QuestEnd.gameObject.SetActive(true); 
        yield return new WaitForSeconds(3f); 
        Instantiate(Reward, RewardPoint.position, transform.rotation);
        QuestEnd.gameObject.SetActive(false); 
        yield return new WaitForSeconds(1f); 
        QuestManager.Instance.QuestComplete(IDQuest);
        Quest.isActive = false;
        Quest.isComplete = false;
        Quest.AfterQuest = true;
        notGo = false;
        Move.instance.stopInput = false;
        

    }
            
 IEnumerator StartQuest()
{            
        notGo = true;
        Quest.isActive = true;
        QuestStart.gameObject.SetActive(true); 
        QuestManager.Instance.AddQuest(Quest);
        QuestManager.Instance.ListQuest(IDQuest);
        QuestManager.Instance.QuestStart(IDQuest);
        yield return new WaitForSeconds(3f); 
        QuestManager.Instance.QuestActive(IDQuest);
        QuestStart.gameObject.SetActive(false); 
        Move.instance.stopInput = false;
        notGo = false;
        FirstD = false;

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
