using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class test : MonoBehaviour
{     
    [Header("Skill")]
    public Skill Skill;

     [Header("Audio")]
    public float basePitch = 1f;
    public float randomPitchOffset = 0.1f;
    [SerializeField] AudioSource take;
    
    [Header("VFX")]
    [SerializeField] GameObject VFX;
    //public TextMeshProUGUI itemDescriptionText;
public bool IsUpper = false;
public bool IsSwordRain = false;
public bool IsTornado = false;
public bool IsSpinner = false;
public bool IsDash = false;
public bool IsMultilunge = false;
public bool IsSlash = false;

////////////////////////////////////////
public bool IsPenetratig = false;
public bool IsGlobe = false;
public bool IsShotgun = false;
public bool IsSawDash = false;
public bool IsWall = false;
public bool IsBomb = false;
public bool IsBoomerang = false;

////////////////////////////////////////
public bool IsGladio = false;
public bool IsLumen = false;
public bool IsTurris = false;
public bool IsShield = false;
public bool IsFlame = false;
public bool IsAura = false;
public bool IsHeal = false;


    public void Pickup()
    {
        if(IsUpper)
        {SkillsInventory.Instance.IsUpper = true;
        SkillInventoryHUD.Instance.IsUpper = true;}
        else if(IsSwordRain)
        {SkillsInventory.Instance.IsSwordRain = true;
        SkillInventoryHUD.Instance.IsSwordRain = true;}
        else if(IsTornado)
        {SkillsInventory.Instance.IsTornado = true;
        SkillInventoryHUD.Instance.IsTornado = true;}
        else if(IsSpinner)
        {SkillsInventory.Instance.IsSpinner = true;
        SkillInventoryHUD.Instance.IsSpinner = true;}
        else if(IsDash)
        {SkillsInventory.Instance.IsDash = true;
        SkillInventoryHUD.Instance.IsDash = true;}
        else if(IsMultilunge)
        {SkillsInventory.Instance.IsMultilunge = true;
        SkillInventoryHUD.Instance.IsMultilunge = true;}
        else if(IsSlash)
        {SkillsInventory.Instance.IsSlash = true;
        SkillInventoryHUD.Instance.IsSlash = true;}

        else if(IsPenetratig)
        {SkillsInventory.Instance.IsPenetratig = true;
        SkillInventoryHUD.Instance.IsPenetratig = true;}
        else if(IsGlobe)
        {SkillsInventory.Instance.IsGlobe = true;
        SkillInventoryHUD.Instance.IsGlobe = true;}
        else if(IsShotgun)
        {SkillsInventory.Instance.IsShotgun = true;
        SkillInventoryHUD.Instance.IsShotgun = true;}
        else if(IsSawDash)
        {SkillsInventory.Instance.IsSawDash = true;
        SkillInventoryHUD.Instance.IsSawDash = true;}
        else if(IsWall)
        {SkillsInventory.Instance.IsWall = true;
        SkillInventoryHUD.Instance.IsWall = true;}
        else if(IsBomb)
        {SkillsInventory.Instance.IsBomb = true;
        SkillInventoryHUD.Instance.IsBomb = true;}
        else if(IsBoomerang)
        {SkillsInventory.Instance.IsBoomerang = true;
        SkillInventoryHUD.Instance.IsBoomerang = true;}

         else if(IsGladio)
        {SkillsInventory.Instance.IsGladio = true;
        SkillInventoryHUD.Instance.IsGladio = true;}
        else if(IsLumen)
        {SkillsInventory.Instance.IsLumen = true;
        SkillInventoryHUD.Instance.IsLumen = true;}
        else if(IsTurris)
        {SkillsInventory.Instance.IsTurris = true;
        SkillInventoryHUD.Instance.IsTurris = true;}
        else if(IsShield)
        {SkillsInventory.Instance.IsShield = true;
        SkillInventoryHUD.Instance.IsShield = true;}
        else if(IsAura)
        {SkillsInventory.Instance.IsAura = true;
        SkillInventoryHUD.Instance.IsAura = true;}
        else if(IsFlame)
        {SkillsInventory.Instance.IsFlame = true;
        SkillInventoryHUD.Instance.IsFlame = true;}
        else if(IsHeal)
        {SkillsInventory.Instance.IsHeal = true;
        SkillInventoryHUD.Instance.IsHeal = true;}
        
 
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
        Pickup();
        Instantiate(VFX, transform.position, transform.rotation);
       // take.Play();

        }
    }
}

