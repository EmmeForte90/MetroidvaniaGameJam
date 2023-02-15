using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine.Unity;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
     #region variable_Movement
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private float horDir;
    
    #endregion

    #region variable_Jump
    [Header("Jump")]
    [SerializeField] private float jumpForce;

    //COYOTE TIME: can jump for a short time after leave ground
    [SerializeField] private float coyoteTime;
    private float lastTimeGround;
    
    //JUMP DELAY: avoid jump only when touch ground
    [SerializeField] private float jumpDelay;
    private float lastTimeJump;

    [SerializeField] private float gravityOnJump;
    [SerializeField] private float gravityOnFall;
    
    private readonly Vector3 raycastColliderOffset = new (0.25f, 0, 0);
    private const float distanceFromGroundRaycast = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    #endregion

    #region variable_Attack
    [Header("Attack")]
    [SerializeField] private float comboWindow;
    [SerializeField] private float comboTimeResetCombo;

    private bool canCombo = true;
    private int comboCount;
    private Coroutine comboTimerCor;
    #endregion
    
    #region Animations
    [Header("Animations")]
    [SpineAnimation][SerializeField] private string idleAnimationName;
    [SpineAnimation][SerializeField] private string runAnimationName;
    [SpineAnimation][SerializeField] private string jumpAnimationName;
    [SpineAnimation][SerializeField] private string jumpDownAnimationName;
    //[SpineAnimation][SerializeField] private string attackAnimationName;

    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;

    private string currentAnimationName;

    #endregion

    private Rigidbody2D rb;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        horDir = Input.GetAxisRaw("Horizontal");

        if (isGrounded())
        {
            lastTimeGround = coyoteTime;   
            rb.gravityScale = 1;
        }
        else
        {
            lastTimeGround -= Time.deltaTime;
            modifyPhysics();
        }

        if (Input.GetButtonDown("Jump"))
            lastTimeJump = Time.time + jumpDelay;

        //Pre-interrupt jump if button released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            lastTimeGround = 0; //Avoid spam button
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);   
        }

        if (canCombo && Input.GetButtonDown("Fire1"))
            attack();
        
        checkFlip();

        selectAnimation();
    }

    private void FixedUpdate()
    {
        float playerSpeed = horDir * speed;
        float accelRate = Mathf.Abs(playerSpeed) > 0.01f? acceleration : deceleration;
        rb.AddForce((playerSpeed - rb.velocity.x) * accelRate * Vector2.right);
        rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, speed).x, rb.velocity.y); //Limit velocity
        
        if (lastTimeJump > Time.time && lastTimeGround > 0)
            jump();
    }

    private void jump()
    {
        lastTimeJump = 0;
        lastTimeGround = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce), ForceMode2D.Impulse);
    }

    private void modifyPhysics()
    {
        if (rb.velocity.y > 0)
            rb.gravityScale = gravityOnJump;
        else if (rb.velocity.y < 0)
            rb.gravityScale = gravityOnFall;
    }

    private void attack()
    {
        comboCount++;

        switch (comboCount)
        {
            case 2:
                Debug.Log("ATTACCO 2");
                if (comboTimerCor != null)
                    StopCoroutine(comboTimerCor);
                comboTimerCor = StartCoroutine(comboTimer());
                break;
            case 3:
                Debug.Log("ATTACCO 3");
                if (comboTimerCor != null)
                    StopCoroutine(comboTimerCor);
                comboTimerCor = StartCoroutine(comboTimer());
                break;
            default:
                Debug.Log("ATTACCO 1");
                if (comboTimerCor != null)
                    StopCoroutine(comboTimerCor);
                comboTimerCor = StartCoroutine(comboTimer());
                break;
        }
    }

    IEnumerator comboTimer()
    {
        canCombo = false;
        
        yield return new WaitForSeconds(comboWindow);
        canCombo = true;
        
        yield return new WaitForSeconds(comboTimeResetCombo - comboWindow);
        comboCount = 0;
    }

    private bool isGrounded()
    {
        //DOUBLE RAYCAST FOR GROUND: check if you touch the ground even with just one leg 
        return (
                Physics2D.Raycast(transform.position + raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
                ||
                Physics2D.Raycast(transform.position - raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
            );
    }
    
    private void checkFlip()
    {
        if (horDir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horDir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void selectAnimation()
    {
        switch (rb.velocity.y)
        {
            case 0:
                if (currentAnimationName != "idle" && horDir == 0)
                {
                    _spineAnimationState.SetAnimation(0, idleAnimationName, true);
                    currentAnimationName = "idle"; 
                }
                else if (currentAnimationName != "run" && horDir != 0)
                {
                    _spineAnimationState.SetAnimation(0, runAnimationName, true);
                    currentAnimationName = "walk"; 
                }
                break;
            case > 0:
                if (currentAnimationName != "jump")
                {
                    _spineAnimationState.SetAnimation(0, jumpAnimationName, false);
                    currentAnimationName = "jump"; 
                }
                break;
            case < 0:
                if (currentAnimationName != "jump_down")
                {
                    _spineAnimationState.SetAnimation(0, jumpDownAnimationName, false);
                    currentAnimationName = "jump_down"; 
                }
                break;
        }
    }
    
#if(UNITY_EDITOR)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + raycastColliderOffset, transform.position + raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
        Gizmos.DrawLine(transform.position - raycastColliderOffset, transform.position - raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
    }
#endif
}
