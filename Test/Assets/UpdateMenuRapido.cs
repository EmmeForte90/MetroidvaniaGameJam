using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateMenuRapido : MonoBehaviour
{
    // Mappa che mappa gli id delle skill ai loro valori
    Dictionary<int, Skill> skillMap = new Dictionary<int, Skill>();
[SerializeField] public Sprite icon0; // Define icon1 as an Image variable
[SerializeField] public Sprite icon1; // Define icon1 as an Image variable
[SerializeField] public Sprite icon2; // Define icon1 as an Image variable
[SerializeField] public Sprite icon3; // Define icon1 as an Image variable


    [SerializeField] public TextMeshProUGUI SkillLeft_T;
    [SerializeField] public TextMeshProUGUI SkillRight_T;
    [SerializeField] public TextMeshProUGUI SkillUp_T;
    [SerializeField] public TextMeshProUGUI SkillBottom_T;

    [SerializeField] public Image SkillLeft;
    [SerializeField] public Image SkillRight;
    [SerializeField] public Image SkillUp;
    [SerializeField] public Image SkillBottom;

    [SerializeField] public Image SkillLeftsel;
    [SerializeField] public Image SkillRightsel;
    [SerializeField] public Image SkillUpsel;
    [SerializeField] public Image SkillBottomsel;
    public float timeSelection = 0.1f; // ritardo tra la spawn di ogni moneta


public static UpdateMenuRapido Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



public void Selup()
    {
        SkillUpsel.gameObject.SetActive(true);
        SkillLeftsel.gameObject.SetActive(false);        
        SkillRightsel.gameObject.SetActive(false);
        SkillBottomsel.gameObject.SetActive(false);
        StartCoroutine(closeSel());

    }

public void Selbottom()
    {
        SkillBottomsel.gameObject.SetActive(true);
        SkillLeftsel.gameObject.SetActive(false);
        SkillUpsel.gameObject.SetActive(false);
        SkillRightsel.gameObject.SetActive(false);
        StartCoroutine(closeSel());

    }

public void Selleft()
    {
        SkillLeftsel.gameObject.SetActive(true);
        SkillUpsel.gameObject.SetActive(false);
        SkillRightsel.gameObject.SetActive(false);
        SkillBottomsel.gameObject.SetActive(false);
        StartCoroutine(closeSel());

    }

    
public void Selright()
    {
        SkillRightsel.gameObject.SetActive(true);
        SkillLeftsel.gameObject.SetActive(false);
        SkillUpsel.gameObject.SetActive(false);
        SkillBottomsel.gameObject.SetActive(false);
        StartCoroutine(closeSel());

    }
IEnumerator closeSel()
{

    
        yield return new WaitForSeconds(timeSelection);
                SkillLeftsel.gameObject.SetActive(false);
                SkillUpsel.gameObject.SetActive(false);
                SkillRightsel.gameObject.SetActive(false);
                SkillBottomsel.gameObject.SetActive(false);


}
}
