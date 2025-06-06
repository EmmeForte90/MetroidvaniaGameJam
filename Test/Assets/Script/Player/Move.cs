using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using TMPro;
using Spine.Unity.AttachmentTools;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    public GameObject ContainerPlayer;

    [Header("Sbloccatori")]
    public bool DoubleJump = false;
    public bool WallJump = false;
    public bool Dash = false;
    public bool Hook = false;
    public bool useMagic = false;
    
    [Header("Bloccatori")]
    public bool StopInput = false;
    public bool isDie = false;
    public bool isTrap = false;
    public bool canTrap = true;
    public bool isHeal = false;

    public string rtAxis       = "RT";        // configurato su 6° asse
    public string ltAxis       = "LT";        // configurato su 3° asse
    public string comboAxis    = "Triggers";  // configurato su 9° asse
    float rt;
    float lt;
    float combo;
    [Header("Threshold to consider 'pressed'")]
    [Header("Press Threshold")]
    [Range(0f, 1f)] public float threshold = 0.1f;

    [Header("Velocità")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    [HideInInspector]public float speed;
    [HideInInspector]public Vector3 velocity;
    [Tooltip("Velocità di accelerazione verso corsa")]
    public float accelerationSpeed = 0.1f;

    [Header("Hook Setting")]
    public Transform targetHook;
    public float pullSpeed = 5f;
    private bool pulling = false;
    public bool BlockInput = false;
    public float hookRadius = 5f;
    public LayerMask hookableLayer;
    public Transform playerTransform;

    [Header("Dash Settings")]
    public GameObject DashVFX;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool isHurt = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownLeft = 0f;
    
    [Header("Blast Settings")]
    public GameObject[] Skill;
    public GameObject firePoint;
    public GameObject StartVFX;
    private bool isBlast = false;
    public float blastTimeLeft = 0.5f;
    private float blastLeft = 0f;
    public float blastDuration = 0.2f;
    private float blastCooldownLeft = 0f; 
    public float blastCooldown = 0.5f;

    [Header("Gravity & Jump")]
    public GameObject JumpVFX;
    public GameObject WallDistancePar;
    public float gravity = 12f;
    public float jumpForce = 5f;
    private int jumpsLeft;
    private int maxJumps = 1;
    [HideInInspector]public bool Jumponwall = false;
    private bool wasGroundedLastFrame;

    [Header("Coyote Jump")]
    public float coyoteTime = 0.2f;      // tempo massimo in secondi dopo essere staccati da terra
    private float coyoteTimer = 0f;

    [Header("Wall Jump & Slide")]
    public float Touch = 1f;
    RaycastHit hit;
    public bool  isFalling = false;
    private bool wallJumpAnimTriggered = false;
    private float wallSlideSpeed = 2f;
    public float wallJumpForce = 7f;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool DoubleJ = false;
    private Vector3 wallNormal;
    private float lastWallDistance = 0f;
    public float gizmoDistance;
        
    [Header("KnockBack")]
    private bool isKnockedBack = false; 
    private bool canKnock = true;
    public float durationKnockback = 0.3f; 
    public float ForceKnocback = 10f; 

    [Header("Combo Settings")]
    public string[] comboAttacks = { "CS/attack", "CS/attack_h", "CS/attack_a" };
    public float comboResetTime = 1.5f;
    private int currentComboIndex = 0;
    public bool isAttacking = false;
    private bool nextAttackBuffered = false;
    private float lastAttackTime;
    public GameObject[] VFXAtk;
    public GameObject DamageCaster;
    public GameObject VFXAtkDOWN;
    public GameObject DamageCasterDOWN;
    public GameObject VFXAtkTOP;
    public GameObject DamageCasterTOP;

    [Header("Animazioni")]
    public string currentState = "Stop";
    private string currentAnimationName;

    [Header("Componenti")]
    private CharacterController characterController;

    [Header("Spine Setup")]
    public SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    [HideInInspector]public float horizontalInput, verticalInput, hor;
    [HideInInspector]public float verticalVelocity = 0f;
    private bool isGrounded = false;
    private bool isJumpRequested = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null) Debug.LogError("Spine SkeletonAnimation mancante!");
        StartVFX.SetActive(false);DashVFX.SetActive(false);JumpVFX.SetActive(false);
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.skeleton;
        spineAnimationState.Event += HandleAnimationEvent;
        spineAnimationState.Complete += OnAttackComplete;
        DamageCaster.SetActive(false);VFXAtkDOWN.SetActive(false);DamageCasterDOWN.SetActive(false);VFXAtkTOP.SetActive(false);DamageCasterTOP.SetActive(false);
        foreach (GameObject go in VFXAtk) {go.SetActive(false);}
        maxJumps = DoubleJump ? 2 : 1;
        jumpsLeft = maxJumps;
    }

    private IEnumerator RestoreButton(float timer)
    {
            BlockInput = true;
            yield return new WaitForSeconds(timer);
            BlockInput = false;
            StopCoroutine(RestoreButton(0f));
    }

    void Update()
    {
        if(GameManager.instance.isGamepadConnected) 
        {    
        rt = Input.GetAxisRaw(rtAxis);
        lt = Input.GetAxisRaw(ltAxis);
        combo = Input.GetAxisRaw(comboAxis);
        if (rt < threshold){rt = Mathf.Max(0f, combo);}
        if (lt < threshold){lt = Mathf.Max(0f, -combo);}
        }

        if(!StopInput){
        onPlatform();
        if(GameManager.instance.E_cur <= 0){isDie = true;} 

        if(!isTrap){
        if(!isDie){
        if(!pulling){
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        hor = Input.GetAxisRaw("Horizontal");

        // Rilevamento muro
        isGrounded = characterController.isGrounded;
        // ─── GESTIONE COYOTE ─────────────────────────────
        if (isGrounded){coyoteTimer = coyoteTime;}
        else{coyoteTimer -= Time.deltaTime;}
        // ─────────────────────────────────────────────────
        Vector3[] directions = { Vector3.left, Vector3.right };
        isTouchingWall = false;

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(WallDistancePar.transform.position, dir, out RaycastHit wallHit, Touch) &&
                wallHit.collider.CompareTag("Wall"))
            {
                isTouchingWall = true;
                wallNormal = wallHit.normal;
                hit = wallHit;
                break;
            }
        }

        if (isTouchingWall)
        {
            wallNormal = hit.normal;
            lastWallDistance = hit.distance;
        }
        else if(!isTouchingWall){isWallSliding = false;}

        // Salto
        if (Input.GetButtonDown("Jump") && !isHeal)
        {
            if (isGrounded || coyoteTimer > 0f)
            {
                isJumpRequested = true;
                Jumponwall = true;
                jumpsLeft = maxJumps;
                coyoteTimer = 0f;   // previene multi‐salti durante il coyote
            }
            else if (WallJump && isTouchingWall)
            {
                Jumponwall = true;
                DoWallJump();
            }
            else if (DoubleJump && jumpsLeft > 0 && !isWallSliding)
            {
                Jumponwall = true;
                JumpVFX.SetActive(true);
                currentState = "DoubleJump";
                DoubleJ = true;
                isJumpRequested = true;
            }
        }
       
        //useMagic
        if(useMagic)
        {
            blastCooldownLeft -= Time.deltaTime;
            if (Input.GetButtonDown("Magic") && blastCooldownLeft <= 0f){StartCoroutine(Blast());}
        }

        // Dash
        if(GameManager.instance.isGamepadConnected)
        {
        if (Dash)
        {
            dashSpeed = 20f;
            dashDuration = 0.3f;
            dashCooldown = 0.3f;
            dashCooldownLeft -= Time.deltaTime;   
            if (rt > 0.1f && dashCooldownLeft <= 0f){OnRightTrigger(rt);Debug.Log("CanDash");}
        }
        else if (!Dash)
        {
            dashSpeed = 10f;
            dashDuration = 0.3f;
            dashCooldown = 0.3f;
            dashCooldownLeft -= Time.deltaTime;
            if(isGrounded){if (rt > 0.1f && dashCooldownLeft <= 0f){OnRightTrigger(rt);Debug.Log("CanDash");}}
        }
        }else if(!GameManager.instance.isGamepadConnected)
        {
        if (Dash)
        {
            dashSpeed = 20f;
            dashDuration = 0.3f;
            dashCooldown = 0.3f;
            dashCooldownLeft -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.X) && dashCooldownLeft <= 0f){StartDash();}
        }
        else if (!Dash)
        {
            dashSpeed = 10f;
            dashDuration = 0.3f;
            dashCooldown = 0.3f;
            dashCooldownLeft -= Time.deltaTime;
            if(isGrounded){if (Input.GetKeyDown(KeyCode.X) && dashCooldownLeft <= 0f){StartDash();}}
        }
        }
        
        //Hook
        if(!GameManager.instance.isGamepadConnected)
        {
        if (Input.GetKeyDown(KeyCode.E) && !BlockInput) // Tasto per attivare il gancio
        {
            targetHook = FindClosestHookable();
            StartCoroutine(RestoreButton(1f));
            if (targetHook != null){pulling = true;}
        }
        }else if(GameManager.instance.isGamepadConnected)
        {
        if (lt > 0.1f && !BlockInput){OnLeftTrigger(lt);}
        }

        // Attacco
    if (Input.GetButtonDown("TastoX"))
    {
        if (!isGrounded)
        {
            if (verticalInput > 0){AttackUp();}
            else if (verticalInput < 0){AttackDown();}
            else{TryAttack();}
        }
        else{TryAttack(); }
    }
    if (Time.time - lastAttackTime > comboResetTime && isAttacking){ResetCombo();}
    
    if(GameManager.instance.isGamepadConnected)
        {
    if (Input.GetButton("Heal") && GameManager.instance.M_cur > 0f && GameManager.instance.E_cur < GameManager.instance.E_max)
    {
        RestoreHP();
        isHeal = true;
        currentState = "Heal";
    }
    else
    {
        isHeal = false;
    }
    }
    else if(!GameManager.instance.isGamepadConnected)
    {
    if (Input.GetKey(KeyCode.C) && GameManager.instance.M_cur > 0f && GameManager.instance.E_cur < GameManager.instance.E_max)
    {
        RestoreHP();
        isHeal = true;
        currentState = "Heal";
    }
    else
    {
        isHeal = false;
    }
    }

        if (!isWallSliding){Flip();}
        HandleMovement();
        }
        else if(pulling)
        {
            if (pulling && targetHook != null)
        {
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetHook.position, pullSpeed * Time.deltaTime);

        // Quando arrivi vicino al punto, ferma il movimento
        if (Vector3.Distance(playerTransform.position, targetHook.position) < 0.1f)
        {
            pulling = false;
            isJumpRequested = true;
        }
        }
        }
        }
        else if(isDie)
        {
            PlayAnimationLoop("Gameplay/die");
            GameManager.instance.CheckpointDie();
        }
        }
        else if(isTrap)
        {
            PlayAnimation("Gameplay/die");
            GameManager.instance.CheckpointTrap();
        }
        }
        
    }
    #region Comands
    protected virtual void OnRightTrigger(float value)
    {
        //Debug.Log($"LT: {value:F2}");
        StartDash();
        // la tua logica per LT
    }
    
    protected virtual void OnLeftTrigger(float value)
    {
        targetHook = FindClosestHookable();
        StartCoroutine(RestoreButton(1f));
        if (targetHook != null){pulling = true;}
        //Debug.Log($"LT: {value:F2}");
        // la tua logica per LT
    }
    #endregion

    #region Restore HP
    public void RestoreHP()
    {
        float mpConsumed = GameManager.instance.mpCostPerSecond * Time.deltaTime;
        float hpRecovered = GameManager.instance.hpRegenPerSecond * Time.deltaTime;
        GameManager.instance.N_cur = Mathf.Max(0, GameManager.instance.N_cur - mpConsumed);
        GameManager.instance.HandleRecovery();
        GameManager.instance.E_cur = Mathf.Min(GameManager.instance.E_max, GameManager.instance.E_cur + hpRecovered);
    }
    #endregion

    #region Jump and air system
    Transform FindClosestHookable()
    {
        Collider[] hookables = Physics.OverlapSphere(playerTransform.position, hookRadius, hookableLayer);

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in hookables)
        {
            float distance = Vector3.Distance(playerTransform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = col.transform;
            }
        }
        return closest;
    }

    public void Bump(){isJumpRequested = true;Debug.Log("Rimbalzo");}
   
    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownLeft = dashCooldown;
        DashVFX.SetActive(true);
        if (Dash){PlayAnimationLoop("Gameplay/dash");}
        else if (!Dash){PlayAnimationLoop("Gameplay/dodge");}
        IgnoreCollisionBetweenLayers(7, 3, dashDuration);
        currentState = "Dash";
    }
    
    private IEnumerator Blast()
    {
           if(GameManager.instance.M_cur > 0)
        {
        isBlast = true;
        blastLeft = blastDuration;
        blastCooldownLeft = blastCooldown;
        yield return new WaitForSeconds(0.2f);
        StartVFX.SetActive(true);
        GameObject bullet = Instantiate(Skill[0], firePoint.transform.position, Quaternion.identity);
        bullet.GetComponent<Globo>().Initialize(transform.localScale);
        GameManager.instance.M_cur -= 20;
        currentState = "Blast";
        }   
    }

    private void HandleMovement()
    {
        // Disabilita movimento orizzontale durante l'attacco
        if (isAttacking){horizontalInput = 0f;}
        if (isWallSliding){horizontalInput = 0f; }
        if (isHeal){horizontalInput = 0f;}
        if (isBlast){horizontalInput = 0f;}

        // Se il player NON ha saltato ma è appena sceso da una piattaforma
        if (!isGrounded && wasGroundedLastFrame && verticalVelocity <= 0f && !isJumpRequested)
        {
            verticalVelocity = -1f; // oppure -2f per una caduta più naturale
        }

        // Reset stato atterraggio
        if (isGrounded && verticalVelocity <= 0f)
        {
            isWallSliding = false;
            isFalling = false;
            wallJumpAnimTriggered = false;
            DoubleJ = false;
            gravity = 12f;

            if (currentState == "Jump" || currentState == "WallJump")
                currentState = "Stop";
        }

        // Wall Slide
        if (!isGrounded && isTouchingWall)
        {
            float inputDirection = Mathf.Sign(horizontalInput);           // -1, 0, 1
            float wallDirection = Mathf.Sign(-wallNormal.x);             // direzione del muro

            if (inputDirection != 0 && inputDirection == wallDirection)
            {
                isWallSliding = true;
                verticalVelocity = Mathf.Max(verticalVelocity - gravity * Time.deltaTime, -wallSlideSpeed);
                currentState = "WallSlide";
            }
            
        }
        else if (isGrounded)
        {
            isWallSliding = false;
        }
        // Se in caduta libera
        if (!isGrounded && !isWallSliding && !isBlast && !isFalling && verticalVelocity <= -0.1f)
        {
            currentState = "Fall";
            gravity = 15f; // più naturale
            isFalling = true;
            DoubleJ = false;
        }
        if (isFalling && gravity < 18f)
        {
            gravity += Time.deltaTime * 10f; // aumenta dolcemente
        }

        // Salto
        if (isJumpRequested)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * 2f * gravity);
            isJumpRequested = false;
            jumpsLeft--;
        }

        // Gravità
        verticalVelocity -= gravity * Time.deltaTime;

        if (isBlast)
        {
            blastTimeLeft -= Time.deltaTime;
            if (blastTimeLeft <= 0f){blastTimeLeft = 0.5f;
            isBlast = false;}
        }

        
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            velocity = (transform.localScale.x > 0 ? Vector3.right : Vector3.left) * dashSpeed;
            velocity.y = verticalVelocity;
            if (dashTimeLeft <= 0f)
                isDashing = false;
        }
        else
        {
            Vector3 moveInput = new Vector3(horizontalInput, 0, 0);
            float inputMagnitude = isAttacking ? 0f : moveInput.magnitude;
            Vector3 moveDirection = moveInput.normalized;
            speed = (inputMagnitude >= 0.6f) ? runSpeed : walkSpeed;
            velocity = moveDirection * speed;
            velocity.y = verticalVelocity;
            UpdateAnimationState(inputMagnitude);
        }

        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = isGrounded;
    }

    private void DoWallJump()
{
    // Blocca input per un brevissimo tempo per evitare movimenti innaturali
    StartCoroutine(RestoreButton(0.15f));
    
    // Resetta la velocità verticale e applica spinta combinata
    verticalVelocity = 0f;

    Vector3 wallJumpDirection = (wallNormal + Vector3.up).normalized;
    verticalVelocity = wallJumpDirection.y * wallJumpForce;

    // Impulso orizzontale
    Vector3 impulse = new Vector3(-wallNormal.x * wallJumpForce, 0f, 0f);
    characterController.Move(impulse * Time.deltaTime);

    isWallSliding = false;
    currentState = "WallJump";
    wallJumpAnimTriggered = true;
}
    #endregion
    
    #region Damage
    public void OnTriggerEnter(Collider collision)
    {if (collision.gameObject.CompareTag("EDamage")){DamageSystem();}
    if(canTrap){if (collision.gameObject.CompareTag("Trap")){if (!isDashing){GameManager.instance.E_cur -= 20f;isTrap = true;canTrap = false;}}}
    }

    public void onPlatform()
{
    Ray ray = new Ray(transform.position, Vector3.down);

    if (Physics.Raycast(ray, out RaycastHit hit, 1.1f))
    {
        if (hit.collider.CompareTag("MovingPlatform"))
        {
            Vector3 originalScale = transform.lossyScale; // Salva la scala globale prima del parenting
            transform.parent = hit.collider.transform;
            transform.localScale = Vector3.one; // Resetta la scala locale
            transform.localScale = new Vector3(
                originalScale.x / transform.parent.lossyScale.x,
                originalScale.y / transform.parent.lossyScale.y,
                originalScale.z / transform.parent.lossyScale.z
            ); // Ricalcola per mantenere scala globale invariata
        }
        else{transform.parent = null; transform.parent = ContainerPlayer.transform;}
    }
    else{transform.parent = null; transform.parent = ContainerPlayer.transform;}
}

    public void DamageSystem()
    {
        if (!isDashing && !isHurt)
        {
            // Determina la direzione del knockback basata sulla direzione in cui il player sta guardando
            float knockbackDirectionX = transform.localScale.x > 0 ? -1f : 1f;
            Vector3 knockbackDirection = new Vector3(knockbackDirectionX, 0, 0); 
            Knockback(knockbackDirection * ForceKnocback, durationKnockback);
            TemporaryChangeColor(Color.red);
            currentState = "Hurt";
            GameManager.instance.DamageData();
            if(!isDie){
            IgnoreCollisionBetweenLayers(7, 3, durationKnockback);
            StartCoroutine(RestoreHurt());}            
        }
    }
    public void IgnoreCollisionBetweenLayers(int layer1, int layer2, float duration)
    {
        // Disabilita collisioni tra layer1 e layer2
        Physics.IgnoreLayerCollision(layer1, layer2, true);

        // Dopo 'duration' secondi, riabilita le collisioni
        StartCoroutine(RestoreCollisionAfterDelay(layer1, layer2, duration));
    }

    private IEnumerator RestoreCollisionAfterDelay(int layer1, int layer2, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreLayerCollision(layer1, layer2, false);
    }


    IEnumerator RestoreHurt()
    {
        isHurt = true;
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    public void Knockback(Vector3 force, float duration)
    {
        if (!isKnockedBack)
        {
            Debug.Log("Pet received knockback!");
            StartCoroutine(ApplyKnockbackCoroutine(force, duration));
        }
    }

    private IEnumerator ApplyKnockbackCoroutine(Vector3 force, float duration)
    {
        isKnockedBack = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            characterController.Move(force * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }
    #endregion
    private void Flip()
    {
        if (hor > 0f) transform.localScale = Vector3.one;
        else if (hor < 0f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateAnimationState(float inputMagnitude)
    {
        if (isDashing) return;
        StateAnm();
        if (isWallSliding && !isFalling && !isBlast)
        {
            currentState = "WallSlide";
            return;
        }
        if (!characterController.isGrounded && !isAttacking && !isFalling && !isBlast && !DoubleJ)
        {
            currentState = "Jump";
            return;
        }
        if (!characterController.isGrounded && !isAttacking && !isFalling && !isBlast && DoubleJ)
        {
            currentState = "DoubleJump";
            return;
        }

        if (inputMagnitude > 0.01f && inputMagnitude < 0.6f && !isAttacking && !isFalling && !isBlast)
        {
            currentState = "Walk";
        }
        else if (inputMagnitude >= 0.6f && !isAttacking && !isFalling && !isBlast)
        {
            currentState = "Run";
        } 
        else if (!isAttacking && !isFalling && !isBlast)
        {
            currentState = "Stop";
        }
        else if (isBlast)
        {
            currentState = "Blast";
        }
    }
    public void StateAnm()
    {
        switch(currentState)
        {
            case "Stop":PlayAnimationLoop("Gameplay/idle");break;
            case "Blast":PlayAnimationLoop("Gameplay/blast");break;
            case "Run":PlayAnimationLoop("Gameplay/run");break;
            case "Walk":PlayAnimationLoop("Gameplay/walk");break;
            case "Jump":PlayAnimationLoop("Gameplay/jump_start");break;
            case "DoubleJump":PlayAnimationLoop("Gameplay/doublejump");break;
            case "Hurt":PlayAnimationLoop("Gameplay/hurt");break;
            case "Fall":PlayAnimationLoop("Gameplay/fall");break;
            case "Dash":PlayAnimationLoop("Gameplay/dash");break;
            case "Heal":PlayAnimationLoop("Gameplay/heal");break;
            case "WallJump":
            if(!Jumponwall){PlayAnimationLoop("Gameplay/jump");}else if(Jumponwall){PlayAnimationLoop("Gameplay/wallslideJump");StartCoroutine(RestoreJumpWallANM());}      
            break;
            case "WallSlide":
            if(!Jumponwall){PlayAnimationLoop("Gameplay/wallslide");}else if(Jumponwall){PlayAnimationLoop("Gameplay/wallslideJump");StartCoroutine(RestoreJumpWallANM());}      
            break;
        }
    }
    IEnumerator RestoreJumpWallANM()
    {
        Jumponwall = true;
        yield return new WaitForSeconds(0.2f);
        Jumponwall = false;
        StopCoroutine(RestoreJumpWallANM());
    }
    #region Gizmo
    #if UNITY_EDITOR
    // Gizmo del raycast muro in rosso e distanza muro
    private void OnDrawGizmos()
    {
        // Direzione del raycast basata su forward della faccia del personaggio
        Vector3 dir = transform.right * (transform.localScale.x > 0 ? 1f : -1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(WallDistancePar.transform.position, dir * gizmoDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerTransform.position, hookRadius);
    }
  
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !isTouchingWall) return;
        Vector3 start = transform.position;
        Vector3 end = start + transform.right * hor * lastWallDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);
        Handles.color = Color.green;
        Handles.Label((start + end) * 0.5f, $"Dist: {lastWallDistance:F2}");
    }
    #endif
    #endregion
    #region Attack
    private void TryAttack() { if (!isAttacking) {PlayAttackAnimation(currentComboIndex); currentState = "Attack";}else if (currentComboIndex < comboAttacks.Length - 1) nextAttackBuffered = true; }
    void AttackUp()
    {
        Debug.Log("Attacco verso l'alto");
        if (!isAttacking) {PlayAttackAnimationAir("CS/jump_atk_up"); currentState = "Attack";} 
    }

    void AttackDown()
    {
        Debug.Log("Attacco verso l'alto");
        if (!isAttacking) {PlayAttackAnimationAir("CS/jump_atk_down"); currentState = "Attack";} 
    }
    private void PlayAttackAnimation(int index) { isAttacking = true; lastAttackTime = Time.time; spineAnimationState.SetAnimation(0, comboAttacks[index], false); }
    private void PlayAttackAnimationAir(string NameAnm) { isAttacking = true; lastAttackTime = Time.time; spineAnimationState.SetAnimation(0, NameAnm, false); }
    private void OnAttackComplete(TrackEntry trackEntry) { if (trackEntry.Animation.Name == comboAttacks[currentComboIndex])
     { if (nextAttackBuffered && currentComboIndex < comboAttacks.Length - 1) { nextAttackBuffered = false; currentComboIndex++; PlayAttackAnimation(currentComboIndex);} 
     else ResetCombo(); }}
    private void ResetCombo() { currentComboIndex = 0; isAttacking = false; lastAttackTime = 0f; nextAttackBuffered = false; currentState = "Stop";}
    public void TemporaryChangeColor(Color color){SetSkeletonColor(color);Invoke(nameof(ResetColor), 0.5f);}
    private void SetSkeletonColor(Color color) => skeletonAnimation.Skeleton.SetColor(color);
    public void ResetColor() => SetSkeletonColor(Color.white);
    #endregion
    #region Animations
    public void PlayAnimationLoop(string animName) { if (currentAnimationName == animName) return; spineAnimationState.SetAnimation(0, animName, true); currentAnimationName = animName; }
    public void PlayAnimation(string animName) { if (currentAnimationName == animName) return; spineAnimationState.SetAnimation(0, animName, false); currentAnimationName = animName; }
    public void StopMovement() { horizontalInput = verticalInput = 0f; currentState = "Stop"; }
    private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e) 
    { 
        if (e.Data.Name == "atk") { VFXAtk[currentComboIndex].SetActive(true); DamageCaster.SetActive(true);} 
        if (e.Data.Name == "atk_down") { VFXAtkDOWN.SetActive(true); DamageCasterDOWN.SetActive(true);} 
        if (e.Data.Name == "atk_top") { VFXAtkTOP.SetActive(true); DamageCasterTOP.SetActive(true);} 
        if (e.Data.Name == "atk_dir") { VFXAtk[currentComboIndex].SetActive(true); DamageCaster.SetActive(true);} 
    }
    #endregion
}