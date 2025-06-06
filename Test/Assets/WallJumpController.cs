using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class WallJumpController : MonoBehaviour
{
    [Header("Wall Detection")]
    [Tooltip("Lunghezza del raycast per rilevare il muro")]
    public float wallCheckDistance = 0.5f;
    [Tooltip("Layer dei muri su cui eseguire il wall‐jump")]
    public LayerMask wallLayerMask;

    [Header("Wall Slide")]
    [Tooltip("Velocità (discendente) durante il wall‐slide")]
    public float slideSpeed = 2f;
    [Tooltip("Forza minima di attrito per bloccare il giocatore al muro")]
    public float wallStickTime = 0.25f;

    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private float wallStickTimer;

    private Vector3 wallNormal = Vector3.zero;
    private CharacterController characterController;
    private Move moveScript; // riferimento allo script Move.cs

    [Header("Wall Jump")]
    [Tooltip("Forza totale del wall‐jump")]
    public float wallJumpForce = 8f;
    [Tooltip("Coef. orizzontale per spinta laterale")]
    public float horizontalMultiplier = 1.2f;
    [Tooltip("Tempo di blocco input durante il wall‐jump")]
    public float inputBlockTime = 0.15f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        moveScript = GetComponent<Move>();
    }

    private void Update()
    {
        CheckForWall();
        HandleWallSlide();
        HandleWallJumpInput();
    }

    /// <summary>
    /// Esegue due raycast (a sinistra e destra) per rilevare se il giocatore è vicino a un muro.
    /// </summary>
    private void CheckForWall()
    {
        isTouchingWall = false;
        wallNormal = Vector3.zero;

        Vector3 origin = transform.position;
        Vector3[] dirs = { Vector3.left, Vector3.right };

        foreach (Vector3 dir in dirs)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, wallCheckDistance, wallLayerMask))
            {
                isTouchingWall = true;
                wallNormal = hit.normal;
                break;
            }
        }
    }

    /// <summary>
    /// Se il giocatore sta in aria e tocca un muro, applica scivolamento controllato.
    /// </summary>
    private void HandleWallSlide()
    {
        // Se tocchiamo il muro ed il CharacterController non è in grounded e stiamo scendendo
        if (!characterController.isGrounded && isTouchingWall && moveScript.verticalVelocity < 0f)
        {
            if (wallStickTimer <= 0f)
            {
                isWallSliding = true;
                moveScript.verticalVelocity = -slideSpeed; 
                // Si resta “appiccicati” per un breve periodo, poi si scivola
                wallStickTimer = wallStickTime;
            }
            else
            {
                wallStickTimer -= Time.deltaTime;
                isWallSliding = true;
                moveScript.verticalVelocity = -slideSpeed;
            }
        }
        else
        {
            isWallSliding = false;
            wallStickTimer = 0f;
        }
    }

    /// <summary>
    /// Controlla se l'utente preme il tasto Jump mentre è in wall‐slide/tocca un muro:
    /// in tal caso innesca il wall‐jump.
    /// </summary>
    private void HandleWallJumpInput()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DoWallJump());
        }
    }

    private IEnumerator DoWallJump()
    {
        // Blocca momentaneamente l’input di movimento principale
        moveScript.BlockInput = true;
        yield return new WaitForSeconds(inputBlockTime);
        moveScript.BlockInput = false;

        // Calcola direzione di salto: verso l’alto + leggermente lontano dal muro
        Vector3 dirUp = Vector3.up;
        Vector3 dirAway = wallNormal * horizontalMultiplier;
        Vector3 finalDir = (dirUp + dirAway).normalized;

        // Imposta velocità verticale/orizzontale direttamente sul Move.cs
        moveScript.verticalVelocity = finalDir.y * wallJumpForce;

        // Per l’input orizzontale, impostiamo la “spinta” orizzontale
        moveScript.velocity.x = finalDir.x * wallJumpForce;

        // Uscita dallo stato di wall‐sliding
        isWallSliding = false;
    }

    /// <summary>
    /// Espone agli altri script (ad es. Move.cs) se stiamo wall‐sliding.
    /// </summary>
    public bool IsWallSliding()
    {
        return isWallSliding;
    }
}
