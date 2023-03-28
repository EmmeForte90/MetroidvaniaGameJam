using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGameSetting : MonoBehaviour
{

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
    
    public Transform respawnPoint; // il punto di respawn del giocatore
    public string sceneName; // il nome della scena in cui si trova il punto di respawn

 public static StartGameSetting instance;


    private void Awake()
    {
        if (instance == null)
        {    
            instance = this;
        }

    }

private void Update()
    {
        
       if(!GameplayManager.instance.startGame)
       {
            Destroy(gameObject);
       }
        
    }

   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        GameplayManager.instance.unlockWalljump = false;   
        GameplayManager.instance.unlockDoubleJump = false; 
        GameplayManager.instance.unlockDash = false;  
        GameplayManager.instance.unlockCrash = false; 
        //Assegna abilità iniziale in base al personaggio
     //   print("ha ricevuto");
        if(GameplayManager.instance.Ainard)   
        {
            AssignId(globo);
            
        }else if(GameplayManager.instance.Milner)   
        {           
            AssignId(gladio);

        }else if(GameplayManager.instance.Galliard)   
        {
            AssignId(slash);
        }
       // print("assegnato");

        AssignButtonUp();
 
    }
    }

public void AssignId(Skill id)
    {
        // Recupera la skill corrispondente all'id selezionato
        selectedSkill = id; 
        selectedId = selectedSkill.id;
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
        Destroy(gameObject);
    }
}

}
