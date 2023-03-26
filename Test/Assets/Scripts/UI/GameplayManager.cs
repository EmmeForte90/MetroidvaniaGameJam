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

public class GameplayManager : MonoBehaviour
{


    public static bool playerExists;

    private GameObject player; // Variabile per il player
    private CinemachineVirtualCamera virtualCamera;
    [HideInInspector]
    public bool gameplayOff = false;

    [Header("Money")]
    [SerializeField] int money = 0;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI moneyTextM;
    [SerializeField] GameObject moneyObject;
    [SerializeField] GameObject moneyObjectM;
    [HideInInspector]
    public bool PauseStop = false;
    //Variabile del testo dei money

    [Header("Music")]
    [SerializeField] bool isStartGame;
    [SerializeField] bool isTImeline;
    [SerializeField] AudioSource City;
    public AudioMixer MSX;
    public AudioMixer SFX;
    
    [Header("Fade")]
    [SerializeField] GameObject callFadeIn;
    [SerializeField] GameObject callFadeOut;
    [SerializeField] GameObject centerCanvas;

    [Header("Pause")]
    [SerializeField] public GameObject PauseMenu;
    //public Transform QuestContent;
    private GameObject Scenary;

    [Header("Abilitazioni")]
    public bool unlockWalljump = false;
    public bool unlockDoubleJump = false;
    public bool unlockDash = false;
    public bool unlockCrash = false;

    public static GameplayManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            Application.targetFrameRate = 60;
            instance = this;
        }
        //QuestManager.Instance.QuestContent = QuestContent;
        player = GameObject.FindWithTag("Player");
        Scenary = GameObject.FindWithTag("Scenary");
virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
virtualCamera.Follow = player.transform;
virtualCamera.LookAt = player.transform;
StartCoroutine(StartFadeInSTART());


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


        if(!moneyObject.gameObject)
        {
            moneyObject.gameObject.SetActive(true);
        }
        
        moneyText.text = money.ToString();
        moneyTextM.text = money.ToString();    
        //Il testo assume il valore dello money
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

//Boh

    void TakeLife()
    {
        //Il player perde 1 vita
        StartCoroutine(Restart());
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

    public void SetVolume(float volume)
    {
        MSX.SetFloat("Volume", volume);

    }


     public void SetSFX(float volume)
    {
        SFX.SetFloat("Volume", volume);

    }

#region Processo vita e morte

public void StartDie()
    {
        StartCoroutine(CallGameSession());
        //AudioManager.instance.DieMusic();
    }

    IEnumerator CallGameSession()
    {
        yield return new WaitForSeconds(2f);
        ProcessPlayerDeath();
        //Richiama i componenti dello script gamesessione e 
        //ne attiva la funzione di processo di morte 

    }
public void ProcessPlayerDeath()
    {
            StartCoroutine(AfterDie());
            //ResetGameSession();
            //Richiama la funzione di reset
    }
    

//Funziona
    public void DeactiveGameOver()
    {
        //gameOver.gameObject.SetActive(false);
        Time.timeScale = 1;
        //playMusic();
        StartCoroutine(Restart());
    }

//Funziona
    IEnumerator AfterDie()
    {
        //gameOver.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
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
            City.Play();
        callFadeOut.gameObject.SetActive(true);
        //Instantiate(callFadeOut, centerCanvas.transform.position, centerCanvas.transform.rotation);
        }
        //FindObjectOfType<PlayerMovement>().ReactivatePlayer();
        yield return new WaitForSeconds(5f);
        callFadeOut.gameObject.SetActive(false);

    }

#endregion

}