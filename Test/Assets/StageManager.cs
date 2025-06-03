using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] AreaBKG;

    // Update is called once per fra
    void Update()
    {
        switch(GameManager.instance.Area)
        {
            case "ABoss": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[0].SetActive(true); break;
            //case "A0": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[0].SetActive(true); break;
            case "A1": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[1].SetActive(true); break;
            case "A2": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[2].SetActive(true); break;
            case "A3": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[3].SetActive(true); break;
            case "A4": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[4].SetActive(true); break;
            case "A5": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[5].SetActive(true); break;
            case "A6": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[6].SetActive(true); break;
            case "A7": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[7].SetActive(true); break;
            case "A8": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[8].SetActive(true); break;
            case "A9": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[9].SetActive(true); break;
            case "A10": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[10].SetActive(true); break;
            case "A11": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[11].SetActive(true); break;
            case "A12": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[12].SetActive(true); break;
            case "A13": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[13].SetActive(true); break;
            case "A14": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[14].SetActive(true); break;
            case "A15": foreach (GameObject col in AreaBKG){col.SetActive(false);}AreaBKG[15].SetActive(true); break;

        }
    }
}
