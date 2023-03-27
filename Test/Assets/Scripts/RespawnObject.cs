using UnityEngine;
using Cinemachine;

public class RespawnObject : MonoBehaviour
{
    public Transform respawnPoint; // il punto di respawn del giocatore
    public string sceneName; // il nome della scena in cui si trova il punto di respawn
    [SerializeField] GameObject Sdeng;
    [SerializeField] GameObject Selectionmenu;
    [SerializeField] public Transform Pos;
    public GameObject Camera;
    //[SerializeField] public GameplayManager gM;
    public GameObject button;
    private CinemachineVirtualCamera virtualCamera; //riferimento alla virtual camera di Cinemachine
    private GameObject player; // Variabile per il player

    private bool _isInTrigger;
    [HideInInspector]public bool isPray = false;

 public static RespawnObject instance;


    private void Awake()
    {
        if (instance == null)
        {    
            instance = this;
        }
       virtualCamera = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>(); //ottieni il riferimento alla virtual camera di Cinemachine
        player = GameObject.FindWithTag("Player");
    }

void Update()
    {

        if (_isInTrigger && Input.GetButtonDown("Talk") && !isPray)
        {
            virtualCamera.Follow = Camera.transform;
            Instantiate(Sdeng, Pos.transform.position, transform.rotation);
            GameplayManager.instance.StopInput();
            Move.instance.StopinputTrue();
            //InventoryManager.Instance.ListItems();
            Move.instance.AnimationRest();
            Move.instance.Stop();
            Selectionmenu.gameObject.SetActive(true);
            UIControllers.instance.SetSelectedGameObjectToSettings();

            //Ripristina gli utilizzi se hai gli slot pieni
            if(UpdateMenuRapido.Instance.idup > 0 || 
            UpdateMenuRapido.Instance.idleft > 0 || 
            UpdateMenuRapido.Instance.idbottom > 0||
            UpdateMenuRapido.Instance.idright > 0 )
            {
            UpdateMenuRapido.Instance.Vleft = SkillMenu.Instance.MXVleft;
            UpdateMenuRapido.Instance.Vup = SkillMenu.Instance.MXVup;
            UpdateMenuRapido.Instance.Vright = SkillMenu.Instance.MXVright;
            UpdateMenuRapido.Instance.Vbottom = SkillMenu.Instance.MXVbottom;
        
            UpdateMenuRapido.Instance.SkillBottom_T.text = UpdateMenuRapido.Instance.Vbottom.ToString();
            UpdateMenuRapido.Instance.SkillUp_T.text = UpdateMenuRapido.Instance.Vup.ToString();
            UpdateMenuRapido.Instance.SkillLeft_T.text = UpdateMenuRapido.Instance.Vleft.ToString();
            UpdateMenuRapido.Instance.SkillRight_T.text = UpdateMenuRapido.Instance.Vright.ToString();
            }
            //Ripristina L'essenza
            PlayerHealth.Instance.currentEssence = PlayerHealth.Instance.maxEssence;
            PlayerHealth.Instance.EssenceImg();

            //Salva la partita
          
          // SaveSystem.SavePlayer(Move.instance);
            Move.instance.isPray = true;
            isPray = true;

        }
        else if (isPray && Input.GetButtonDown("Talk"))
        {
            Move.instance.animationWakeup();
            GameplayManager.instance.StopInputResume();
            Move.instance.StopinputFalse();
            UIControllers.instance.SetSelectedGameObjectToSettings();
            Selectionmenu.gameObject.SetActive(false);   
            isPray = false;
            Move.instance.isPray = false;
            _isInTrigger = false;
            virtualCamera.Follow = player.transform;        

    
        }
        
    }

public void ChooseCharacter()
    {

    }
public void notchoose()
    {
        virtualCamera.Follow = Camera.transform;

    }
    
private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            button.gameObject.SetActive(true); // Initially hide the dialogue text
            _isInTrigger = true;
            
        }
    }
private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            button.gameObject.SetActive(false); // Initially hide the dialogue text
            _isInTrigger = false;
            
        }
    }

}

