using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class test : MonoBehaviour
{
    public Skill Skill;
    public TextMeshProUGUI itemDescriptionText;

    public void Pickup()
    {
        //itemDescriptionText.text = Skill.Description;
        SkillsInventory.Instance.IsGlobe = true;
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

