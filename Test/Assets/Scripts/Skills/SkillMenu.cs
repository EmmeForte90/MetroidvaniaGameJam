using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{

//C'è da stabilere COME  deve recuperare la skill in automatico senza assegnarla pubblicamente
    private Skill Skill;
    public int id;

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

     


    void Update()
    {     
        horDir = Input.GetAxisRaw("Horizontal");
        vertDir = Input.GetAxisRaw("Vertical");
    }

public void assignId()
{
    Skill = descriptionData.Instance.Skill;
    id = descriptionData.Instance.Skill.id;
    
}


public void AssignButtonUp()
{
    assignId(); // Aggiorniamo il valore di id

    if(id > 0)
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
        assignId(); // Aggiorniamo il valore di id

    if(id > 0)
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
        assignId(); // Aggiorniamo il valore di id

    if(id > 0)
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

    assignId(); // Aggiorniamo il valore di id

if(id > 0)
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

public class InventorySlot
{
    public Item type;
    public int quantity;

    public InventorySlot(Item t, int q)
    {
        type = t;
        quantity = q;
    }
}