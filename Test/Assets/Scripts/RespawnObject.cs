using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    public Transform respawnPoint; // il punto di respawn del giocatore
    public string sceneName; // il nome della scena in cui si trova il punto di respawn
    [SerializeField] GameObject Sdeng;
    [SerializeField] GameObject Selectionmenu;
    [SerializeField] public Transform Pos;
    //[SerializeField] public GameplayManager gM;
    public GameObject button;
    private Move player;
    private GameplayManager gM;

    private bool _isInTrigger;
    private bool isPray = false;

void Awake()
    {
        GameObject playerObject = GameObject.Find("Knight");
         if (player == null)
        {
        player = playerObject.GetComponent<Move>();
        }

        GameObject GmObject = GameObject.Find("Gameplay");
         if (gM == null)
        {
        gM = GmObject.GetComponent<GameplayManager>();
        }

    }

void Update()
    {

        if (_isInTrigger && Input.GetButtonDown("Talk") && !isPray)
        {
            Instantiate(Sdeng, Pos.transform.position, transform.rotation);
            gM.StopInput();
            player.StopinputTrue();
            //InventoryManager.Instance.ListItems();
            player.AnimationRest();
            player.Stop();
            Selectionmenu.gameObject.SetActive(true);

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

            isPray = true;

        }
        else if (isPray && Input.GetButtonDown("Talk"))
        {
            player.animationWakeup();
            gM.StopInputResume();
            player.StopinputFalse();
            Selectionmenu.gameObject.SetActive(false);   
                            isPray = false;
                            _isInTrigger = false;
    
        }
        
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

