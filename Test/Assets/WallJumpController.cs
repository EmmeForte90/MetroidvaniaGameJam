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
    public GameObject WallDistancePar;
    public float gizmoDistance;

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

        Vector3 origin = WallDistancePar.transform.position;
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
            else if (!Physics.Raycast(origin, dir, out hit, wallCheckDistance, wallLayerMask))
            {
                isTouchingWall = false;
                break;
            }
        }
    }

    /// <summary>
    /// Se il giocatore sta in aria, tocca un muro, sta premendo verso il muro e cade,
    /// allora applica scivolamento controllato. Altrimenti, esce dal wall‐slide e
    /// applica subito una spinta orizzontale verso l’esterno del muro.
    /// </summary>
    private void HandleWallSlide()
    {
        // 1) Input orizzontale
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2) Verifico se sto premendo verso il muro
        bool pressingIntoWall = false;
        if (isTouchingWall && horizontalInput != 0f)
        {
            // Se il muro è a destra (wallNormal.x > 0), devo premere Right (+1).
            // Se il muro è a sinistra (wallNormal.x < 0), devo premere Left (−1).
            pressingIntoWall = (Mathf.Sign(horizontalInput) == -Mathf.Sign(wallNormal.x));
        }

        // 3) Condizioni per entrare in wall‐slide:
        //    – non sono a terra
        //    – tocco il muro
        //    – sto cadendo (verticalVelocity < 0)
        //    – sto premendo verso il muro
        if (!characterController.isGrounded 
            && isTouchingWall 
            && moveScript.verticalVelocity < 0f 
            && pressingIntoWall)
        {
            if (wallStickTimer <= 0f)
            {
                isWallSliding = true;
                moveScript.verticalVelocity = -slideSpeed;
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
            // Se ero in wall‐slide e ora devo uscirne:
            if (isWallSliding)
            {
                // Calcolo la direzione orizzontale "fuori dal muro":
                // wallNormal.x indica verso dove il muro "punta": se wallNormal.x > 0 → muro a destra, 
                // quindi voglio spingermi a sinistra (−1). Viceversa se wallNormal.x < 0.
                float awayDir = -Mathf.Sign(wallNormal.x);

                // Imposto subito la velocità orizzontale in Move.cs per far "ricominciare" il movimento:
                moveScript.velocity.x = awayDir * moveScript.speed;
            }

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

    private void OnDrawGizmos()
    {
        // Direzione del raycast basata su forward della faccia del personaggio
        Vector3 dir = transform.right * (transform.localScale.x > 0 ? 1f : -1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(WallDistancePar.transform.position, dir * gizmoDistance);
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
