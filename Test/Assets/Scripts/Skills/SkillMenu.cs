using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{

//C'è da stabilere COME  deve recuperare la skill in automatico senza assegnarla pubblicamente
    public Skill Skill;
    public  int selectedId = -1; // Id dell'abilità selezionata
    private float horDir;
    private float vertDir;
    [SerializeField] private TextMeshProUGUI[] skillValueTexts; // Array di TextMeshProUGUI per i valori delle abilità
    [SerializeField] private Image[] skillIconImages; // Array di Image per le icone delle abilità

    [SerializeField] TextMeshProUGUI SkillLeft_T;
    [SerializeField] TextMeshProUGUI SkillRight_T;
    [SerializeField] TextMeshProUGUI SkillUp_T;
    [SerializeField] TextMeshProUGUI SkillBottom_T;

    [SerializeField] Image SkillLeft;
    [SerializeField] Image SkillRight;
    [SerializeField] Image SkillUp;
    [SerializeField] Image SkillBottom;

     


    void Update()
    {     
        horDir = Input.GetAxisRaw("Horizontal");
        vertDir = Input.GetAxisRaw("Vertical");
    }
 
public void AssignId(int id)
    {
        selectedId = id; // Assegna l'id dell'abilità selezionata
        descriptionData.Instance.Skill.id = selectedId;

    }


public void AssignButtonUp()
{

    if(selectedId > 0)
{
    if (Skill == null)
{
    Skill = descriptionData.Instance.Skill;
}

   // descriptionData.Instance.Skill.id = id;
    
    SkillUp_T.text = Skill.value.ToString();
    SkillUp.sprite = Skill.icon;
}
}

public void AssignButtonBottom()
{

    if(selectedId > 0)
{
    if (Skill == null)
{
    Skill = descriptionData.Instance.Skill;

}

   // descriptionData.Instance.Skill.id = id;
    
    SkillBottom_T.text = Skill.value.ToString();
    SkillBottom.sprite = Skill.icon;
}
}

public void AssignButtonLeft()
{

    if(selectedId > 0)
{
    if (Skill == null)
{
    Skill = descriptionData.Instance.Skill;

}

   // descriptionData.Instance.Skill.id = id;
    
    SkillLeft_T.text = Skill.value.ToString();
    SkillLeft.sprite = Skill.icon;
}
}

public void AssignButtonRight()
{


if(selectedId > 0)
{
    if (Skill == null)
{
    Skill = descriptionData.Instance.Skill;

}
   // descriptionData.Instance.Skill.id = id;
    SkillRight_T.text = Skill.value.ToString();
    SkillRight.sprite = Skill.icon;
}
}


}
