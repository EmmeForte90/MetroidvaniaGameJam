using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
     public Transform Follower;
    public Transform Player;

     private Vector3 originalScale;

    void Awake()
    {
        // Salva la scala originale per mantenerla costante
        if (Follower != null)
            originalScale = Follower.localScale;
    }

    void LateUpdate()
    {
        FollowWithoutScaling();
    }

    /// <summary>
    /// Segue il player in posizione, mantenendo inalterata la scala locale.
    /// </summary>
    private void FollowWithoutScaling()
    {
        if (Follower == null || Player == null)
            return;

        // Copia solo la posizione del player
        Follower.position = Player.position;
        // Ripristina la scala originale
        Follower.localScale = originalScale;
    }
}


