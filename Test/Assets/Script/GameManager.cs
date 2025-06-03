using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool Test = false;
    public int Money = 0;
    public TextMeshProUGUI MoneyText;
    public bool isPause = false;
    public GameObject Pause;
    public GameObject Player;
    public Move BrainPlayer;
    public CameraShake CameraShakeS;
    public CameraFollow CameraFollowS;
    public string Area;

    [Header("CheckPoint")]
    public GameObject SavedCheck;
    public GameObject CheckPoint;

    [Header("Fade")]
    public GameObject FadeOpen;
    public GameObject FadeClosed;
    public bool canPause = false;

    [Header("Sprite")]
    public Sprite[] Key;
    public Image KeyUI;

    [Header("Some states")]
    public float Atk = 20;
    public float skillDamage = 20;
    public Scrollbar EnergyBar;
    public Scrollbar MPBar;
    public Image manaImageUI; 
    public float smoothSpeed = 5f; // Puoi regolarla da Inspector

    public float E_cur; 
    public float E_max;
    public float M_cur; 
    public float M_max;
    public float N_cur; 
    public float N_max;
    public float mpCostPerSecond = 10f;
    public float hpRegenPerSecond = 5f;
    public bool canRestore = false;
    public float EnemyAtk = 0;
    public int IconHP = 0;
    public int IconMP = 0;
    public RectTransform targetImageHP; // Assegna qui l'Image nella UI
    public RectTransform targetImageMP; // Assegna qui l'Image nella UI
    private float widthIncrease = 50f; // Quanto aumentare la larghezza
    private float posXIncrease = 50f;  // Quanto spostare in X

    [Header("Joypad")]
    public bool isGamepadConnected = false; 
    public delegate void InputDeviceChanged(bool isGamepad);
    public static event InputDeviceChanged OnInputDeviceChanged;
    public GameObject[] ButtonXboxJoypad;
    public GameObject[] ButtonKeyboard;
    
    [Header("Audio Settings")]
    [Tooltip("Audio Mixer per la musica")]
    public AudioMixer musicMixer;

    [Tooltip("Audio Mixer per gli effetti sonori")]
    public AudioMixer sfxMixer;

    [Header("Music Clips")]
    [Tooltip("Lista dei brani musicali disponibili")]
    public AudioClip[] musicClips;

    [Header("Sound Effects")]
    [Tooltip("Lista degli effetti sonori disponibili")]
    public AudioClip[] soundEffects;

    [Header("Voice Clips")]
    [Tooltip("Lista delle voci")]
    public AudioClip[] voiceClips;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource voiceSource;
    private bool isMusicPlaying = false;
    private bool isSFXPlaying = false;
    private bool isVoicePlaying = false;
    
    [Header("Treasure")]
    public bool[] ChestId;
    public bool[] HaveKey;



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        FadeOpen.SetActive(true);
        FadeClosed.SetActive(false);
        if (instance == null){instance = this;}
        E_cur = E_max; M_cur = M_max;N_cur = N_max;
        StartCoroutine(StartPauseInput());
        InitializeAudioSources();
    }
    private IEnumerator StartPauseInput()
    {
        yield return new WaitForSeconds(4f);
        canPause = true;
        StopCoroutine(StartPauseInput());
        //foreach (GameObject ItemOBJS in Menu){ItemOBJS.SetActive(false);}
    }


    private void InitializeAudioSources()
    {
        // Music AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Master")[0];
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // SFX AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixer.FindMatchingGroups("Master")[0];
        sfxSource.playOnAwake = false;

        // Voice AudioSource
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.outputAudioMixerGroup = sfxMixer.FindMatchingGroups("Master")[0];
        voiceSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGamepadConnection();
    // Aggiornamento UI in base alla connessione del gamepad
    bool isUsingGamepad = isGamepadConnected;
    SetActiveForUI(ButtonXboxJoypad, isUsingGamepad);
    SetActiveForUI(ButtonKeyboard, !isUsingGamepad);

        HP();
        MoneyText.text = Money.ToString();
        HandleRecovery();
        if (Input.GetButtonDown("Pause") && canPause)
    {
        if (!isPause){Time.timeScale = 0f; isPause = true;Pause.SetActive(true);}
        else{Time.timeScale = 1f; isPause = false;Pause.SetActive(false);}
    
    }
    }
    public void HandleRecovery()
{
    float targetScaleValue = Mathf.Clamp(N_cur / N_max, 0f, 0.9f);
    Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, targetScaleValue);

    // Interpola in modo fluido verso la scala target
    manaImageUI.transform.localScale = Vector3.Lerp(
        manaImageUI.transform.localScale,
        targetScale,
        Time.deltaTime * smoothSpeed
    );
}

    

    public void QuitGame(){Application.Quit();Debug.Log("Quitting Game");}

    public void StartRun()
    {
        //StartCoroutine(StartGameF());
        Debug.Log("Start Game");
    }

    
    #region Settings
    //public void SetResolution(int resolutionIndex){Resolution resolution = resolutions[resolutionIndex];Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);}
    public void SetQuality(int qualityIndex){QualitySettings.SetQualityLevel(qualityIndex);}
    public void SetFullscreen(bool isFullScreen){Screen.fullScreen = isFullScreen;}
    #endregion

    #region JoypadFunction
    void SetActiveForUI(GameObject[] uiElements, bool isActive){foreach (GameObject item in uiElements){item.SetActive(isActive);}}
    
    void CheckGamepadConnection()
    {
    string[] connectedJoysticks = Input.GetJoystickNames();
    bool gamepadCurrentlyConnected = connectedJoysticks.Length > 0 && !string.IsNullOrEmpty(connectedJoysticks[0]);
    if (gamepadCurrentlyConnected != isGamepadConnected)
    {
        isGamepadConnected = gamepadCurrentlyConnected;
        Debug.Log(isGamepadConnected ? "Joypad collegato!" : "Joypad scollegato!");
        // Notifica ai listener il cambiamento
        OnInputDeviceChanged?.Invoke(isGamepadConnected);
    }
    }
    #endregion
    #region Data for damage
    public void DamageData()
    {
        E_cur -= EnemyAtk;
        //Pet_Toy.DamageCount.SetActive(true);
        //Pet_Toy.DC_T.text = EnemyAtk.ToString();
    }
    #endregion 
    public void ResizeAndMoveHP()
    {
        if (targetImageHP == null) return;

        // Aumenta la larghezza
        Vector2 newSize = targetImageHP.sizeDelta;
        newSize.x += widthIncrease;
        targetImageHP.sizeDelta = newSize;

        // Sposta in X
        Vector2 newPos = targetImageHP.anchoredPosition;
        newPos.x += posXIncrease;
        targetImageHP.anchoredPosition = newPos;
    }
    public void ResizeAndMoveMP()
    {
        if (targetImageMP == null) return;

        // Aumenta la larghezza
        Vector2 newSize = targetImageMP.sizeDelta;
        newSize.x += widthIncrease;
        targetImageMP.sizeDelta = newSize;

        // Sposta in X
        Vector2 newPos = targetImageMP.anchoredPosition;
        newPos.x += posXIncrease;
        targetImageMP.anchoredPosition = newPos;
    }

    public void HP()
    {  
    EnergyBar.size = Mathf.RoundToInt(E_cur) / (float)Mathf.RoundToInt(E_max);
    EnergyBar.size = Mathf.Clamp(EnergyBar.size, 0.01f, 1f);
    MPBar.size = Mathf.RoundToInt(M_cur) / (float)Mathf.RoundToInt(M_max);
    MPBar.size = Mathf.Clamp(MPBar.size, 0.01f, 1f);
    if(IconHP >= 4){E_max += 50;ResizeAndMoveHP();IconHP = 0;}
    if(IconMP >= 4){M_max += 50;ResizeAndMoveMP();IconMP = 0;}
    if(E_max >= 500){E_max = 500;}if(M_max >= 500){M_max = 500;}
    //HPC_T.text = string.Format("{0:00}/{1:00}", Mathf.FloorToInt(E_cur), Mathf.FloorToInt(E_max));
    //MPC_T.text = string.Format("{0:00}/{1:00}", Mathf.FloorToInt(M_cur), Mathf.FloorToInt(MP_max));
    //MPCM_T.text = string.Format("{0:00}/{1:00}", Mathf.FloorToInt(M_cur), Mathf.FloorToInt(MP_max));
    if(E_cur <= 0){E_cur = 0;} 
    if(M_cur <= 0){M_cur = 0;}
    if(E_cur > E_max){E_cur = E_max;} 
    if(M_cur > M_max){M_cur = M_max;} 
    }
    #region ReturnCheckpoint
    public void CheckpointDie(){StartCoroutine(RestartCheck());}
    public void CheckpointTrap(){StartCoroutine(RestartCheckTrap());BrainPlayer.isTrap = false;}

    IEnumerator RestartCheck()
    {
        BrainPlayer.StopInput = true;
        BrainPlayer.isTrap = false;
        yield return new WaitForSeconds(4f);
        FadeClosed.SetActive(true);
        yield return new WaitForSeconds(2f);
        E_cur = E_max;M_cur = M_max;BrainPlayer.isDie = false;
        BrainPlayer.PlayAnimationLoop("CheckpointAnimation/respawn_rest");
        Player.transform.position = SavedCheck.transform.position;
        yield return new WaitForSeconds(1f);
        FadeOpen.SetActive(true);
        yield return new WaitForSeconds(3f);
        BrainPlayer.PlayAnimationLoop("CheckpointAnimation/respawn");
        yield return new WaitForSeconds(1f);
        BrainPlayer.PlayAnimationLoop("Gameplay/idle");
        BrainPlayer.canTrap = true;
        BrainPlayer.StopInput = false;
    }
    IEnumerator RestartCheckTrap()
    {
        BrainPlayer.StopInput = true;
        BrainPlayer.isTrap = false;
        yield return new WaitForSeconds(1f);
        FadeClosed.SetActive(true);
        yield return new WaitForSeconds(1f);
        BrainPlayer.PlayAnimationLoop("CheckpointAnimation/respawn");
        Player.transform.position = CheckPoint.transform.position;
        yield return new WaitForSeconds(1f);
        FadeOpen.SetActive(true);
        BrainPlayer.PlayAnimationLoop("CheckpointAnimation/respawn");
        yield return new WaitForSeconds(1f);
        BrainPlayer.PlayAnimationLoop("Gameplay/idle");
        BrainPlayer.canTrap = true;
        BrainPlayer.StopInput = false;
    }
    public void testDie(){E_cur = 0;}
    public void testCamera(){CameraShakeS.Shake(0.2f, 0.15f);}


    #endregion

    #region Music Methods
    public void PlayMusic(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= musicClips.Length)
        {
            Debug.LogWarning("Indice del brano musicale non valido.");
            return;
        }

        musicSource.clip = musicClips[clipIndex];
        musicSource.Play();
        isMusicPlaying = true;
    }

    public void StopMusic(int whatMusic)
    {
    if (isMusicPlaying && musicSource.clip == musicClips[whatMusic])
    {
        musicSource.Stop();
        isMusicPlaying = false;
    }
    }

    public void CrossFadeMusic(int newClipIndex, float fadeDuration = 1f)
    {
        if (newClipIndex < 0 || newClipIndex >= musicClips.Length)
        {
            Debug.LogWarning("Indice del brano musicale non valido.");
            return;
        }

        StartCoroutine(CrossFadeCoroutine(newClipIndex, fadeDuration));
    }

    private IEnumerator CrossFadeCoroutine(int newClipIndex, float duration)
    {
        float startVolume = musicSource.volume;

        // Fade out current music
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        // Switch to new clip
        musicSource.Stop();
        musicSource.clip = musicClips[newClipIndex];
        musicSource.Play();

        // Fade in new music
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= soundEffects.Length)
        {
            Debug.LogWarning("Indice dell'effetto sonoro non valido.");
            return;
        }

        sfxSource.PlayOneShot(soundEffects[clipIndex]);
        isSFXPlaying = true;
        StartCoroutine(ResetSFXState(soundEffects[clipIndex].length));
    }

    private IEnumerator ResetSFXState(float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.2f); // Aspetta un breve intervallo
        isSFXPlaying = false;
    }
    #endregion

    #region Voice Methods
    public void PlayVoice(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= voiceClips.Length)
        {
            Debug.LogWarning("Indice della voce non valido.");
            return;
        }

        // Riproduci la voce
        voiceSource.PlayOneShot(voiceClips[clipIndex]);
        isVoicePlaying = true;

        // Usa la lunghezza del clip per il coroutine
        StartCoroutine(ResetVoiceState(voiceClips[clipIndex].length));
    }

    private IEnumerator ResetVoiceState(float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.2f); // Aspetta la fine del clip
        isVoicePlaying = false;
    }
    #endregion

    #region Volume Controls
    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("Volume", Mathf.Log10(volume) * 20); // Converte in dB
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.SetFloat("Volume", Mathf.Log10(volume) * 20); // Converte in dB
    }
    #endregion

}