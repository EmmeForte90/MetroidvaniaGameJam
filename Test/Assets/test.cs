using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class test : MonoBehaviour
{
    public Skill Skill;
    public TextMeshProUGUI itemDescriptionText;
    public bool IsGlobe = false;

    public bool IsDashAtk = false;

    public bool IsSlashSword = false;


    public void Pickup()
    {
        if(IsGlobe)
        {SkillsInventory.Instance.IsGlobe = true;
        SkillInventoryHUD.Instance.IsGlobe = true;}
        else if(IsDashAtk)
        {SkillsInventory.Instance.IsDashAtk = true;
        SkillInventoryHUD.Instance.IsDashAtk = true;}
        else if(IsSlashSword)
        {SkillsInventory.Instance.IsSlashSword = true;
        SkillInventoryHUD.Instance.IsSlashSword = true;}
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
        }
    }
}

