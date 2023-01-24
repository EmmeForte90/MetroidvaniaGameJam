using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
   [SerializeField] AudioClip coinPickupSFX;
    //Variabile per il suono
    [SerializeField] public int pointsForCoinPickup;
    //Valore della moneta quando raccolta
    [SerializeField] public float loadDelay = 0.5f;
    //Temo di recupero
    [SerializeField] public Animator myAnimator;
    //Animatore
    bool wasCollected = false;
    [SerializeField]  GameObject light;
    //Bool per evitare che la moneta sia raccolta più volte
    [SerializeField] public bool isHeal;
    [SerializeField] public bool isBullet;
    [SerializeField] public bool isLifeUp;


void Start()
{
    myAnimator = GetComponent<Animator>();
    //Recupera i componenti dell'animator
}


    void OnTriggerEnter2D(Collider2D other) 
    {

        #region CollCoin
        if (other.tag == "Player" && !wasCollected && !isHeal && !isBullet && !isLifeUp)
        //Se il player tocca la moneta e non è stato collezionata
        {
            wasCollected = true;
            //La moneta è collezionata
            FindObjectOfType<GameplayManager>().AddTomoney(pointsForCoinPickup);
            //Richiama la funzione dello script GameSessione e aumenta lo score
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            //Avvia l'audio
            myAnimator.SetTrigger("take");
            //Attiva il suono
            Invoke("takeCoin", loadDelay);
            
        }

        #endregion
        
        #region  CollHP
        else if (other.tag == "Player" && !wasCollected && isHeal && !isBullet && !isLifeUp)
        //Se il player tocca la moneta e non è stato collezionata
        {
            //Se gli HP non sono al massimo la raccoglio altrimenti no
            if(PlayerHealth.instance.heartsRemain != PlayerHealth.instance.heartsNumber.Count)
            {
            wasCollected = true;
            //La moneta è collezionata
            FindObjectOfType<PlayerHealth>().restoreOneHeart();
            //Richiama la funzione dello script GameSessione e aumenta lo score
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            //Avvia l'audio
            myAnimator.SetTrigger("take");
            //Attiva il suono
            Invoke("takeCoin", loadDelay);
            }
            
        }
        #endregion

        #region CollBullet
        /*else if (other.tag == "Player" && !wasCollected && !isHeal && isBullet && !isLifeUp)
        //Se il player tocca la moneta e non è stato collezionata
        {
            //Se i proiettili non sono al massimo la raccoglio altrimenti no
            if(PlayerBulletCount.instance.bulletRemain != PlayerBulletCount.instance.bulletNumber.Count)
            {
            wasCollected = true;
            //La moneta è collezionata
            FindObjectOfType<PlayerBulletCount>().restoreOneBullet();
            //Richiama la funzione dello script GameSessione e aumenta lo score
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            //Avvia l'audio
            myAnimator.SetTrigger("take");
            //Attiva il suono
            Invoke("takeCoin", loadDelay);
            }
            
            
        }*/
#endregion

        #region 1UP
        else if (other.tag == "Player" && !wasCollected && !isHeal && !isBullet && isLifeUp)
        //Se il player tocca la moneta e non è stato collezionata
        {
            
            wasCollected = true;
            //La moneta è collezionata
            //FindObjectOfType<GameSession>().AddLife();
            //Richiama la funzione dello script GameSessione e aumenta lo score
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            //Avvia l'audio
            myAnimator.SetTrigger("take");
            light.gameObject.SetActive(false);
            //Attiva il suono
            Invoke("takeCoin", loadDelay);
            
        }
        #endregion
    }

#region Funzione per cancellare l'item
    void takeCoin()
    {

        gameObject.SetActive(false);
        //Disattiva l'oggetto
        Destroy(gameObject);
        //Lo distrugge
    }
    #endregion
}
