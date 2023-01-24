using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


    public class PlayerHealth : MonoBehaviour
    {
        public List<GameObject> heartsNumber;
        //Lista degli HP

        public static PlayerHealth instance;
        //Instanza per interagire con altri script


        [HideInInspector]
        public int heartsRemain;
        //Hp rimanenti

        private void Awake()
        {
            heartsRemain = heartsNumber.Count;
            //HP totali
            instance = this;
        }

#region  HP al massimo

        public bool healthIsFull()
        {
            if (heartsRemain == heartsNumber.Count)
                return true;
            else
                return false;
        }
        #endregion

#region  Recupera 1 HP

        public void restoreOneHeart()
        {
            /*if (!healthIsFull())
            {
                heartsNumber[heartsRemain].GetComponent<HPAnimation>().restoreHP();
                heartsRemain += 1;
            }
            */
        }

#endregion

#region  Rimuovi 1 HP

        public void removeOneHeart()
        {
            if (heartsRemain == 1)
                playerDeath();
            else
            {
                //heartsNumber[heartsRemain - 1].GetComponent<HPAnimation>().removeHP();
                //heartsRemain -= 1;
            }
        }

#endregion

#region  Se gli HP arrivato a 0 il player muore

        public void playerDeath()
        {
            while(heartsRemain != heartsNumber.Count)
            {
                restoreOneHeart();
            }
            FindObjectOfType<GameplayManager>().ProcessPlayerDeath();
            //transform.position = Data.checkpointPos;
            
        }

#endregion
}