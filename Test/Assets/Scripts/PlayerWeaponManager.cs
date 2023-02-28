using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Usa l'input system di Unity scaricato dal packet manager


public class PlayerWeaponManager : MonoBehaviour
{
    public static PlayerWeaponManager instance;
   [Header("Proiettili")]
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject rapid;
    [SerializeField] private GameObject shotgun;
    [SerializeField] private GameObject bomb;
    public KeyCode Kk = KeyCode.F;
private float cooldown = 0.5f;
private float lastWeaponChangeTime;
    private int currentWeaponIndex;

    CharacterController2D playerShootScript;

    private void Awake()
    { 
        playerShootScript = GetComponent<CharacterController2D>();
        instance = this;
        currentWeaponIndex = 1;
    }
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
        FindObjectOfType<ChangeWeaponAnimation>().ChangeWeapon(weaponIndex + 1);
        currentWeaponIndex = weaponIndex;
    
}

#endregion

public void SetWeapon(int WeaponID)
{
    switch (WeaponID)
    {
        case 1:
    playerShootScript.SetBulletPrefab(normal);
    break;

    case 2:
    playerShootScript.SetBulletPrefab(rapid);
    break;

    case 3:
    playerShootScript.SetBulletPrefab(shotgun);
    break;

    case 4:
    playerShootScript.SetBulletPrefab(bomb);
    break;
        
    }
    

 }
}

