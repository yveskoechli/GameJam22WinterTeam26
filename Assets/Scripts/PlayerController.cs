using System;
using System.Collections;
using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Inspector

    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    
    
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpSpeed = 10f;

    [SerializeField] private float slowDownTime = 2f;
    
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float screenShakeTime = 1f;

    [SerializeField] private LayerMask groundLayer;
    
    #endregion

    private GameInput input;
    private InputAction moveAction;

    private float moveInput;
    private float moveDirection;

    private float slowDownMove = 0f;
    

    private Rigidbody2D rbPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animator;

    private EnemyController enemy;
    
    private GameController gameController;
    private float gameMultiplier = 1f;

    #region Unity Event Functions

    private void Awake()
    {
        input = new GameInput();
        EnableInput();

        moveAction = input.Player.Move;

        rbPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spritePlayer = GetComponent<SpriteRenderer>();
        enemy = FindObjectOfType<EnemyController>();
        gameController = FindObjectOfType<GameController>();

        vCam = FindObjectOfType<CinemachineVirtualCamera>();
        
        input.Player.Jump.performed += Jump;
    }

    private void Update()
    {
        ReadInput();
        Move(moveInput);

        UpdateAnimation();
    }


    private void OnDestroy()
    {
        input.Player.Jump.performed -= Jump;
    }
    
    #endregion
    
    #region Input
    public void EnableInput()
    {
        input.Enable();
    }

    public void DisableInput()
    {
        input.Disable();
    }
    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<float>();
    }


    #endregion

    #region Movement

    private void Move(float moveInput)
    {
        rbPlayer.velocity = new Vector2((moveInput * moveSpeed) - slowDownMove, rbPlayer.velocity.y);
        spritePlayer.flipX = rbPlayer.velocity.x < -2f ? true : false;

    }

    private void Jump(InputAction.CallbackContext _)
    {
        if (!Groundcheck())
        {
            return;
        }
        rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpSpeed);
        enemy.Jump();
    }


    private bool Groundcheck()
    {
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayer);
        
        return isGrounded;
    }
    
    #endregion

    #region Animation

    private void UpdateAnimation()
    {
        Vector2 velocity = rbPlayer.velocity;
        velocity.x = MathF.Abs(velocity.x);
        animator.SetFloat(MovementSpeed, velocity.x);
        animator.SetFloat(AnimationSpeed, (velocity.x + 1f) * gameMultiplier);
        switch (velocity.y)
        {
            case < 0:
                animator.SetBool(IsFalling, true);
                animator.SetBool(IsJumping, false);
                break;
            case > 0:
                animator.SetBool(IsJumping, true);
                animator.SetBool(IsFalling, false);
                break;
            case 0:
                animator.SetBool(IsJumping, false);
                animator.SetBool(IsFalling, false);
                break;
        }
    }

    #endregion

    public void SlowDown()
    {
        StartCoroutine(SlowDown(slowDownTime));
        StartCoroutine(ScreenShake(screenShakeTime));
        animator.SetTrigger(Damaged);
    }
    
    
    private IEnumerator SlowDown(float time)
    {
        slowDownMove = 2f;
        DisableInput();
        yield return new WaitForSeconds(time);
        slowDownMove = 0f;
        EnableInput();
    }
    
    private IEnumerator ScreenShake(float time)
    {
        vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1;
        yield return new WaitForSeconds(time);
        vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GameOver"))
        {
            Debug.Log("The enemy hits the player!");
            //TODO Restart Level + Respawn Player + Enemy
            //Destroy(gameObject);
        }
        
        
    }
}
