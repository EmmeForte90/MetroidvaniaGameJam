using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unlockSkill : MonoBehaviour
{
    public bool Isdoublejump = false;
    public bool IsDash = false;
    public bool Iswalljump = false;
    [SerializeField] GameObject VFX;


    public void Pickup()
    {
        if(Isdoublejump)
        {Move.instance.unlockDoubleJump = true;
        }
        else if(IsDash)
        {Move.instance.unlockDash = true;
        }
        else if(Iswalljump)
        {Move.instance.unlockWalljump = true;
        }
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Pickup();
                    Instantiate(VFX, transform.position, transform.rotation);

        }
    }
}
