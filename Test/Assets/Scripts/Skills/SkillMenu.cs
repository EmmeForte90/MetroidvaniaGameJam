using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{
   
    [SerializeField] TextMeshProUGUI SkillLeft_T;
    [SerializeField] TextMeshProUGUI SkillRight_T;
    [SerializeField] TextMeshProUGUI SkillUp_T;
    [SerializeField] TextMeshProUGUI SkillBottom_T;

    [SerializeField] GameObject SkillLeft;
    [SerializeField] GameObject SkillRight;
    [SerializeField] GameObject SkillUp;
    [SerializeField] GameObject SkillBottom;

    private void Awake()
    {
                
    }

    void Update()
    {     
        /*if (Input.GetButtonDown("UseItem") || (!rtButtonFree && Input.GetAxisRaw("UseItem") > 0))
        {
            rtButtonFree = true;
            //activeEffect();
        }

        if (Input.GetAxisRaw("UseItem") == 0)
            rtButtonFree = false;

        if (Input.GetButtonDown("ChangeItem") || (!ltButtonFree && Input.GetAxisRaw("ChangeItem") > 0))
        {
            ltButtonFree = true;
            moveSlot();
        }

        if (Input.GetAxisRaw("ChangeItem") == 0)
            ltButtonFree = false;*/
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