using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
La classe SkillMenu contiene una mappa di abilità (skillMap), 
in cui gli id delle skill sono mappati ai loro valori, e tre icone di abilità 
(icon1, icon2, icon3). Viene anche dichiarato un valore intero "selectedId" 
per tenere traccia dell'abilità selezionata, e due variabili float "horDir" 
e "vertDir" per tenere traccia dell'input dell'utente.

Nel metodo Start(), le tre abilità definite precedentemente vengono aggiunte 
alla mappa skillMap.

Nel metodo Update(), l'input dell'utente viene registrato e memorizzato nelle 
variabili horDir e vertDir.

Ci sono quattro metodi pubblici AssignButtonUp(), AssignButtonLeft(), AssignButtonRight(), 
e AssignButtonBottom(), ognuno dei quali viene chiamato quando l'utente preme il pulsante 
corrispondente sulla tastiera o sul controller. Ognuno di questi metodi recupera l'abilità 
corrispondente all'id selezionato, se l'id è maggiore di zero (cioè se un'abilità è stata selezionata), 
quindi aggiorna l'etichetta e l'icona dell'abilità corrispondente. L'etichetta e l'icona dell'abilità 
vengono visualizzate in quattro posizioni differenti, corrispondenti ai pulsanti su, sinistra, 
destra e giù dell'interfaccia grafica del menu.

Infine, c'è un metodo pubblico AssignId(), che assegna l'id dell'abilità selezionata alla 
variabile selectedId. Questo metodo viene chiamato quando l'utente seleziona un'abilità dal menu.*/

public class SkillMenu : MonoBehaviour
{

// Mappa che mappa gli id delle skill ai loro valori
    Dictionary<int, Skill> skillMap = new Dictionary<int, Skill>();
[SerializeField] private Sprite icon0; // Define icon1 as an Image variable
[SerializeField] private Sprite icon1; // Define icon1 as an Image variable
[SerializeField] private Sprite icon2; // Define icon1 as an Image variable
[SerializeField] private Sprite icon3; // Define icon1 as an Image variable

    public int selectedId = -1; // Id dell'abilità selezionata
    private float horDir;
    private float vertDir;


    [SerializeField] TextMeshProUGUI SkillLeft_T;
    [SerializeField] TextMeshProUGUI SkillRight_T;
    [SerializeField] TextMeshProUGUI SkillUp_T;
    [SerializeField] TextMeshProUGUI SkillBottom_T;

    [SerializeField] Image SkillLeft;
    [SerializeField] Image SkillRight;
    [SerializeField] Image SkillUp;
    [SerializeField] Image SkillBottom;

public static SkillMenu Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Aggiungi le tue skill alla mappa
        skillMap.Add(-1, new Skill("noSkill", 0, icon0));//NoSkill
        skillMap.Add(1, new Skill("Skill 1", 5, icon1));//Globo
        skillMap.Add(2, new Skill("Skill 2", 20, icon2));//SwordSlash
        skillMap.Add(3, new Skill("Skill 3", 10, icon3));//PenetratingSlash
    }

    void Update()
    {
        horDir = Input.GetAxisRaw("Horizontal");
        vertDir = Input.GetAxisRaw("Vertical");
    }

    public void AssignId(int id)
    {
        selectedId = id; // Assegna l'id dell'abilità selezionata
    }

   public void AssignButtonUp()
{
    // Recupera la skill corrispondente all'id selezionato
    Skill selectedSkill = skillMap[selectedId];

    if (selectedId > 0)
    {
        SkillUp_T.text = selectedSkill.value.ToString();
        SkillUp.sprite = selectedSkill.icon;

         UpdateMenuRapido.Instance.SkillUp_T.text = selectedSkill.value.ToString();
         UpdateMenuRapido.Instance.SkillUp.sprite = selectedSkill.icon;
    }
}
  public void AssignButtonleft()
{
    // Recupera la skill corrispondente all'id selezionato
    Skill selectedSkill = skillMap[selectedId];

    if (selectedId > 0)
    {
        SkillLeft_T.text = selectedSkill.value.ToString();
        SkillLeft.sprite = selectedSkill.icon;
         UpdateMenuRapido.Instance.SkillLeft_T.text = selectedSkill.value.ToString();
        UpdateMenuRapido.Instance.SkillLeft.sprite = selectedSkill.icon;
    }
}  
public void AssignButtonright()
{
    // Recupera la skill corrispondente all'id selezionato
    Skill selectedSkill = skillMap[selectedId];

    if (selectedId > 0)
    {
        SkillRight_T.text = selectedSkill.value.ToString();
        SkillRight.sprite = selectedSkill.icon;

        UpdateMenuRapido.Instance.SkillRight_T.text = selectedSkill.value.ToString();
        UpdateMenuRapido.Instance.SkillRight.sprite = selectedSkill.icon;
    }
}  public void AssignButtonbottom()
{
    // Recupera la skill corrispondente all'id selezionato
    Skill selectedSkill = skillMap[selectedId];

    if (selectedId > 0)
    {
        SkillBottom_T.text = selectedSkill.value.ToString();
        SkillBottom.sprite = selectedSkill.icon;

        UpdateMenuRapido.Instance.SkillBottom_T.text = selectedSkill.value.ToString();
        UpdateMenuRapido.Instance.SkillBottom.sprite = selectedSkill.icon;
    }
}
}
