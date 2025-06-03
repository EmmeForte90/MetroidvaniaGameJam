using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColoredCollider : MonoBehaviour
{
        public Color gizmoColor = Color.blue;

     void OnDrawGizmos()
    {
        
        // Cambia il colore dei Gizmo
        Gizmos.color = gizmoColor;

        // Prendi il collider dell'oggetto
        Collider collider = GetComponent<Collider>();

        // Disegna i gizmos in base al tipo di collider
        if (collider is BoxCollider boxCollider)
        {
            // Disegna un cubo per il BoxCollider
            Gizmos.matrix = transform.localToWorldMatrix; // Applica la trasformazione locale dell'oggetto
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (collider is SphereCollider sphereCollider)
        {
            // Disegna una sfera per il SphereCollider
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            // Disegna una capsula per il CapsuleCollider
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius), capsuleCollider.radius); // Sfera superiore
            Gizmos.DrawWireSphere(capsuleCollider.center + Vector3.down * (capsuleCollider.height / 2 - capsuleCollider.radius), capsuleCollider.radius); // Sfera inferiore
            Gizmos.DrawLine(capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius), capsuleCollider.center + Vector3.down * (capsuleCollider.height / 2 - capsuleCollider.radius)); // Linea centrale
        }
    }
}
