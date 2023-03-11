using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Usa l'input system di Unity scaricato dal packet manager


public class PlayerWeaponManager : MonoBehaviour
{
   [Header("Skill")]
    [SerializeField] private GameObject Globo;
    [SerializeField] private GameObject PenetratingSlash;
    [SerializeField] private GameObject SlashSword;
    [SerializeField] private GameObject bomb;
   
   // public KeyCode Kk = KeyCode.F;
//private float cooldown = 0.5f;
//private float lastWeaponChangeTime;
  //  private int currentWeaponIndex;

    public static PlayerWeaponManager instance;

    private void Awake()
    { 
        instance = this;
        //currentWeaponIndex = 1;
    }

    public void SetWeapon(int WeaponID)
{
    switch (WeaponID)
    {
    case 1:
    Move.instance.SetBulletPrefab(Globo);
    break;

    case 2:
    Move.instance.SetBulletPrefab(PenetratingSlash);
    break;

    case 3:
    Move.instance.SetBulletPrefab(SlashSword);
    break;

    case 4:
    Move.instance.SetBulletPrefab(bomb);
    break;
        
    }
    

 }
    /*
void Update()
{
     if (Input.GetKey(Kk) && Time.time - lastWeaponChangeTime > cooldown)
    {
        OnChangeWeapon();
        lastWeaponChangeTime = Time.time;
    }
    
    }

#region ChangeWeapon
void OnChangeWeapon()
{
        int weaponIndex = (currentWeaponIndex + 1) % 4;
        SetWeapon(weaponIndex + 1);
        currentWeaponIndex = weaponIndex;
    
}

#endregion*/


}

