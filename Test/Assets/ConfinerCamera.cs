using UnityEngine;

public class ConfinerCamera : MonoBehaviour
{
    [Header("Camera Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public bool alwaysShowGizmos = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TakeCamera();
        }
    }

    private void TakeCamera()
    {
        if (GameManager.instance != null && GameManager.instance.CameraFollowS != null)
        {
            GameManager.instance.CameraFollowS.minBounds = minBounds;
            GameManager.instance.CameraFollowS.maxBounds = maxBounds;
        }
        else
        {
            Debug.LogWarning("GameManager or CameraFollowS is not assigned.");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!alwaysShowGizmos) return;

        Gizmos.color = Color.green;

        float z = Application.isPlaying && GameManager.instance?.CameraFollowS?.transform != null
            ? GameManager.instance.CameraFollowS.transform.position.z
            : 0f;

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            z
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
