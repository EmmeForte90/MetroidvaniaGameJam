using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using Spine.Unity.AttachmentTools;
using Spine.Unity;
using Spine;

public class MainMenu : MonoBehaviour
{

    public GameObject continueButton;
    public GameObject mainmenu;
    public GameObject menu;
    public string pGar;
    public string pAst;
    public string pMil;
    public bool isAstra;
    public bool isGarland;
    public bool isPheresord;

    public string startScene;
    public float Timelife;
    public GameObject opzioni;
    public AudioMixer MSX;
    public AudioMixer SFX;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject player; // Variabile per il player

    Resolution[] resolutions;
    //public Dropdown resolutionDropdown;
    //public PlayerAbilityTracker player;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("ContinueLevel"))
        {
            continueButton.SetActive(true);
        }

        resolutions = Screen.resolutions;

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
         string option = resolutions[i].width + "x" + resolutions[i].height;
        options.Add(option);

        if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
        {
            currentResolutionIndex = i;
        }

        }
        player = GameObject.FindWithTag("Player");
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        //virtualCamera.Follow = menu.transform;
        //virtualCamera.LookAt = player.transform;

//        resolutionDropdown.ClearOptions();
//        resolutionDropdown.value = currentResolutionIndex;
        //resolutionDropdown.RefreshValue();
//        resolutionDropdown.AddOptions(options);



        if(PlayerPrefs.HasKey(startScene + "_unlocked"))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }

    }

    public void Continue()
    {
        //player.gameObject.SetActive(true);
        //PlayerMovement.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));
        StartCoroutine(fadeCont());

    }

public void SetResolution(int resolutionIndex)
{
    Resolution resolution = resolutions[resolutionIndex];

    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
}
public void ChooseCharacter()
    {
        virtualCamera.Follow = player.transform;
        //virtualCamera.LookAt = player.transform;
        mainmenu.gameObject.SetActive(false);

    }
public void notchoose()
    {
        virtualCamera.Follow = menu.transform;
        //virtualCamera.LookAt = player.transform;
        mainmenu.gameObject.SetActive(true);

    }

    public void StartGame()
    {
        StartCoroutine(fade());

    }
    
    public void Phere()
    {
        isPheresord = true;
        isAstra = false;
        isGarland = false;
    } 

    public void Gar()
    {
        isPheresord = false;
        isAstra = false;
        isGarland = true;
    } 
    public void Astr()
    {
        isPheresord = false;
        isAstra = true;
        isGarland = false;

    }


    public void Options()
    {
        opzioni.gameObject.SetActive(true);
    }
    public void ExitOptions()
    {
        opzioni.gameObject.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        MSX.SetFloat("Volume", volume);

    }


     public void SetSFX(float volume)
    {
        SFX.SetFloat("Volume", volume);

    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

IEnumerator fade()
    {
        
        PlayerPrefs.DeleteAll();
        yield return new WaitForSeconds(Timelife);
//        GameplayManager.instance.gameplayOff = false;
if (isGarland)
{
        SceneManager.LoadScene(pGar);

} else if (isPheresord)
{
        SceneManager.LoadScene(pMil);

} else if (isAstra)
{
        SceneManager.LoadScene(pAst);

}

            
    }


    IEnumerator fadeCont()
    {
        
        yield return new WaitForSeconds(Timelife);
        SceneManager.LoadScene(PlayerPrefs.GetString("ContinueLevel"));
        //AudioManager.instance.PlayMFX(1);



            
    }
 

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");

    }

}
