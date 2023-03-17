using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainMenuCam : MonoBehaviour
{
    public GameObject mainmenu;
    public GameObject menu;
    private GameObject player; // Variabile per il player
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {        
        player = GameObject.FindWithTag("Player");
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
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
    
}
