using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;
    private Vector3 target;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Assign both pointA and pointB in the inspector!");
            return;
        }

        target = pointB.position;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;


        // Switch direction if reached the target
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
    }
}