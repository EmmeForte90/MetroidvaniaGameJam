using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using Spine.Unity.AttachmentTools;
using Spine.Unity;
using Spine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{


    public static bool playerExists;

    private GameObject player; // Variabile per il player
    private CinemachineVirtualCamera virtualCamera;
    [HideInInspector]
    public bool gameplayOff = false;
    public bool StopDefaultSkill = false;

    [Header("Money")]
    [SerializeField] int money = 0;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI moneyTextM;
    [SerializeField] GameObject moneyObject;
    [SerializeField] GameObject moneyObjectM;
    [HideInInspector]
    public bool PauseStop = false;
    //Variabile del testo dei money

   

    [Header("Fade")]
    [SerializeField] GameObject callFadeIn;
    [SerializeField] GameObject callFadeOut;
   private bool isStartGame;

    [Header("Pause")]
    [SerializeField] public GameObject PauseMenu;
    private GameObject Scenary;

    [Header("Personaggio")]
    public bool Ainard = false;
    public bool Milner = false;
    public bool Galliard = false;

[Header("Stato inizio gioco")]
    public Skill slash;
    public Skill globo;
    public Skill gladio;
    private Skill selectedSkill;

    [HideInInspector]
    public int selectedId = -1; // Id dell'abilità selezionata
    public int idup= -1; // Id dell'abilità selezionata
    public int MXVup; // Id dell'abilità selezionata
       
    [SerializeField] TextMeshProUGUI SkillUp_T;
    [SerializeField] Image SkillUp;


    [Header("Abilitazioni")]
    public bool unlockWalljump = false;
    public bool unlockDoubleJump = false;
    public bool unlockDash = false;
    public bool unlockCrash = false;
    public bool startGame = false;


    private GameObject[] Ordalia;
    public bool[] OrdaliaActive;

    private GameObject[] Door;
    public bool[] DoorActive;
    public static GameplayManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Application.targetFrameRate = 60;
          // Verifica se un'istanza del GameObject esiste già e distruggila se necessario
        if (playerExists) //&& gameplayOff) 
        {
            Destroy(gameObject);
        }
        else 
        {
            playerExists = true;
            DontDestroyOnLoad(gameObject);
            
        }

        Scenary = GameObject.FindWithTag("Scenary");
        virtualCamera = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>(); //ottieni il riferimento alla virtual camera di Cinemachine
        player = GameObject.FindWithTag("Player");
        // Cerca tutti i GameObjects con il tag "Timeline" all'inizio dello script
        //Ordalia = GameObject.FindGameObjectsWithTag("Ordalia");
        StartCoroutine(StartFadeInSTART());

        if(!startGame)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }else
        {
        AudioManager.instance.CrossFadeINAudio(1);
        unlockWalljump = false;   
        unlockDoubleJump = false; 
        unlockDash = false;  
        unlockCrash = false; 
        }

      if(!StopDefaultSkill)
        {
            if(Ainard)   
        {
            AssignId(globo);
            SkillInventoryHUD.Instance.IsGlobe = true;
        }else if(Milner)   
        {           
            AssignId(gladio);
            SkillInventoryHUD.Instance.IsGladio = true;

        }else if(Galliard)   
        {
            AssignId(slash);
            SkillInventoryHUD.Instance.IsSlash = true;

        }
       // print("assegnato");

        AssignButtonUp();
        }


        if(!moneyObject.gameObject)
        {
            moneyObject.gameObject.SetActive(true);
        }
        
        moneyText.text = money.ToString();
        moneyTextM.text = money.ToString();    
        //Il testo assume il valore dello money
    }

public void AssignId(Skill id)
    {
        // Recupera la skill corrispondente all'id selezionato
        selectedSkill = id; 
        if(selectedId == null)
        {
        selectedId = selectedSkill.id;
        }
    }

    public void AssignButtonUp()
{
    if (selectedId > 0)
    {
        SkillUp_T.text = selectedSkill.value.ToString();
        SkillUp.sprite = selectedSkill.icon;
        idup = selectedId;
        UpdateMenuRapido.Instance.idup = selectedId;
        UpdateMenuRapido.Instance.SkillUp_T.text = selectedSkill.value.ToString();
        UpdateMenuRapido.Instance.Vup = selectedSkill.value;
        UpdateMenuRapido.Instance.SkillUp.sprite = selectedSkill.icon;
        MXVup = selectedSkill.value;


        
    }
}


public void TakeCamera()
    {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;    
    }
    

public void StartPlay()
    {
        StartCoroutine(StartStage());
    }

#region  Score
//Funziona
    public void AddTomoney(int pointsToAdd)
    {
        money += pointsToAdd;
        //Lo money aumenta
        moneyText.text = money.ToString(); 
        moneyTextM.text = money.ToString();    
        //il testo dello money viene aggiornato
    }


#endregion

#region Pausa
        public void Pause()
        //Funzione pausa
        {
            PauseMenu.gameObject.SetActive(true);
           // Scenary.gameObject.SetActive(false);
            UIControllers.instance.SetSelectedGameObjectToSettings();
            //Move.instance.Player.gameObject.SetActive(false);
            PauseStop = true;
            //Time.timeScale = 0f;
        }

        public void Resume()
        {
            //Time.timeScale = 1;
            PauseStop = false;
            PauseMenu.gameObject.SetActive(false);
           // Scenary.gameObject.SetActive(true);

            //Move.instance.Player.gameObject.SetActive(true);

        }
public void StopInput()
        //Funzione pausa
        {
            PauseStop = true;
            //Time.timeScale = 0f;
        }

        public void StopInputResume()
        {
            //Time.timeScale = 1;
            PauseStop = false;
        }
#endregion

   
private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}


private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // Cerca tutti i GameObjects con il tag "Ch_Quest"
    GameObject[] Ordalia = GameObject.FindGameObjectsWithTag("Ordalia");
    GameObject[] Door = GameObject.FindGameObjectsWithTag("Door");

    // Itera attraverso tutti gli oggetti trovati
    foreach (GameObject Character in Ordalia)
    {
        // Ottiene il componente QuestCharacters
        TriggerOrdalia ordaliT = Character.GetComponent<TriggerOrdalia>();

        // Verifica se il componente esiste
        if (ordaliT != null)
        {
            // Verifica se l'id della quest corrisponde all'id di un gameobject in OrdaliaActive
            int Id = ordaliT.id;
            for (int i = 0; i < OrdaliaActive.Length; i++)
            {
                if (OrdaliaActive[i] && i == Id)
                {
                    // Imposta ordaliT.FirstD a false
                    ordaliT.OrdaliaDosentExist();
                    break;
                }
            }
        }
    }


    foreach (GameObject Character in Door)
    {
        // Ottiene il componente QuestCharacters
        Gate DoorT = Character.GetComponent<Gate>();

        // Verifica se il componente esiste
        if (DoorT != null)
        {
            // Verifica se l'id della quest corrisponde all'id di un gameobject in OrdaliaActive
            int Id = DoorT.id;
            for (int i = 0; i <  DoorActive.Length; i++)
            {
                if ( DoorActive[i] && i == Id)
                {
                    // Imposta ordaliT.FirstD a false
                    DoorT.DoorOpen();
                    break;
                }
            }
        }
    }
}

public void OrdaliaEnd(int id)
{
    // Imposta lo stato della quest a true
    OrdaliaActive[id] = true;   
}

public void DoorAct(int id)
{
    // Imposta lo stato della quest a true
    DoorActive[id] = true;   
}


    IEnumerator Restart()
    {
        callFadeIn.gameObject.SetActive(true);
        //Instantiate(callFadeIn, centerCanvas.transform.position, centerCanvas.transform.rotation);
        yield return new WaitForSeconds(5f);
        //Le vite del player vengono aggiornate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Lo scenario assume il valore della build
        SceneManager.LoadScene(currentSceneIndex);
        //Lo scenario viene ricaricato
    }




public void StopFade()
    {
        StartCoroutine(EndFede());
    }
// Coroutine per attendere il caricamento della scena
IEnumerator EndFede()
{   
    yield return new WaitForSeconds(1f);
    callFadeOut.gameObject.SetActive(false);
    callFadeIn.gameObject.SetActive(false);
    
}
public void FadeIn()
    {

StartCoroutine(StartFadeIn());

    }

    public void FadeOut()
    {

StartCoroutine(StartFadeOut());

    }
IEnumerator StartFadeInSTART()
    {
        callFadeIn.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        callFadeIn.gameObject.SetActive(false);
        //callFadeIn.gameObject.SetActive(false);


    }


IEnumerator StartFadeIn()
    {
        callFadeOut.gameObject.SetActive(false);
        callFadeIn.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        //callFadeOut.gameObject.SetActive(false);
        //callFadeIn.gameObject.SetActive(false);


    }

IEnumerator StartFadeOut()
    {        
        callFadeIn.gameObject.SetActive(false);
        callFadeOut.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
       // callFadeIn.gameObject.SetActive(false);
       // callFadeOut.gameObject.SetActive(false);


    }



    IEnumerator StartStage()
    {
        if(!isStartGame)
        {
        callFadeOut.gameObject.SetActive(true);
        //Instantiate(callFadeOut, centerCanvas.transform.position, centerCanvas.transform.rotation);
        }
        //FindObjectOfType<PlayerMovement>().ReactivatePlayer();
        yield return new WaitForSeconds(5f);
        callFadeOut.gameObject.SetActive(false);

    }


}