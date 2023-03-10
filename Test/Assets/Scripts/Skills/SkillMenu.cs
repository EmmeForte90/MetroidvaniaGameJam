using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{

//C'è da stabilere COME  deve recuperare la skill in automatico senza assegnarla pubblicamente
    private Skill Skill;
    private int id;

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
    if(descriptionData.Instance.DataId == 1)
    {
        id = 1;
    }else if(descriptionData.Instance.DataId == 2)
    {
        id = 2;
    }if(descriptionData.Instance.DataId == 3)
    {
        id = 3;
    }
    

}


public void AssignButtonUp()
{

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