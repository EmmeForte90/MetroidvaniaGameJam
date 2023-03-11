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
   

    public static PlayerWeaponManager instance;

    private void Awake()
    { 
        instance = this;
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
   


}

