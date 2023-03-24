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
    private int IDQuest;
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

    public bool isInteragible;
    public bool heFlip;
    public bool FirstD = true;
    //public bool EndDia = false;
    //public bool EndQue = false;

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
else if (Quest.isActive || Quest.isComplete)//EndDia
{
    dialogue = Quest.Endingdialogue;
}
else
{
    dialogue = Quest.Middledialogue;
}
        anim.SetBool("talk", _isInTrigger);
        if(heFlip)
        {
        FacePlayer();
        }

        if (_isInTrigger && Input.GetButtonDown("Talk") && !_isDialogueActive)
        {
            Move.instance.stopInput = true;
            Move.instance.Stop();
            Move.instance.Stooping();
            dialogueIndex = 0;
            StartCoroutine(ShowDialogue());
        }
        else if (_isDialogueActive && Input.GetButtonDown("Talk"))
        {
            NextDialogue();
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
            else if(Quest.isActive)
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

        QuestEnd.gameObject.SetActive(true); 
        //QuestManager.Instance.Quest3 = true;
        yield return new WaitForSeconds(3f); 
        Instantiate(Reward, RewardPoint.position, transform.rotation);
        QuestEnd.gameObject.SetActive(false); 
        yield return new WaitForSeconds(1f); 
        QuestManager.Instance.QuestComplete(IDQuest);
        Quest.isActive = false;
        Quest.isComplete = true;
        //EndDia = false;
        //EndQue = true;
        Move.instance.stopInput = false;
        

    }
            
 IEnumerator StartQuest()
{
        QuestStart.gameObject.SetActive(true); 
        QuestManager.Instance.AddQuest(Quest);
        QuestManager.Instance.ListQuest(IDQuest);
        QuestManager.Instance.QuestStart(IDQuest);
        yield return new WaitForSeconds(3f); 
                 
        QuestStart.gameObject.SetActive(false); 
        Move.instance.stopInput = false;
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
