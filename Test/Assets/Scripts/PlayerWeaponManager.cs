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


    private void Awake()
    { 
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
        currentWeaponIndex = weaponIndex;
    
}

#endregion

public void SetWeapon(int WeaponID)
{
    switch (WeaponID)
    {
        case 1:
    Move.instance.SetBulletPrefab(normal);
    
    break;

    case 2:
    Move.instance.SetBulletPrefab(rapid);
    break;

    case 3:
    Move.instance.SetBulletPrefab(shotgun);
    break;

    case 4:
    Move.instance.SetBulletPrefab(bomb);
    break;
        
    }
    

 }
}

