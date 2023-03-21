using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndQuest : MonoBehaviour
{
     public void Pickup()
    {
        QuestCharacters.Instance.EndDia = true;
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
            //Instantiate(VFX, transform.position, transform.rotation);

        }
    }
}
