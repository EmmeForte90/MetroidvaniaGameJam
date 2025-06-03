using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Camera;
    public Transform Player;

    [Header("Camera Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    public bool alwaysShowGizmos = true;

    [Header("Only for test")]
    public bool isTest = false;
    public float yOffset = 12f; // Altezza extra sopra al player

    private Vector3 originalScale;

    void Awake()
    {
        if (Camera != null){originalScale = Camera.localScale;}
        if(isTest) {yOffset = 2f;}
    }

    void LateUpdate()
    {
        FollowWithoutScaling();
    }

    private void FollowWithoutScaling()
    {
        if(!isTest){
        if (Camera == null || Player == null)
            return;

        // Posizione desiderata dal player
        Vector3 desiredPosition = Player.position;

        // Mantieni la Z originale della camera
        desiredPosition.z = Camera.position.z;

        // Applica limiti
        
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        Camera.position = new Vector3(clampedX, clampedY, desiredPosition.z);

        // Mantieni la scala originale
        Camera.localScale = originalScale;
        }
        else if(isTest)
        {
        yOffset = 2f;
        Camera.position = new Vector3(
        Player.position.x,
        Player.position.y + yOffset,
        Camera.position.z
    );

    Camera.localScale = originalScale;

        }
    }

#if UNITY_EDITOR
void OnDrawGizmos()
{
    if (!alwaysShowGizmos || Camera == null) return;

    Gizmos.color = Color.green;

    Vector3 center = new Vector3(
        (minBounds.x + maxBounds.x) / 2f,
        (minBounds.y + maxBounds.y) / 2f,
        Camera.position.z
    );

    Vector3 size = new Vector3(
        Mathf.Abs(maxBounds.x - minBounds.x),
        Mathf.Abs(maxBounds.y - minBounds.y),
        0f
    );

    Gizmos.DrawWireCube(center, size);
}
#endif

}

