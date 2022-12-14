using System;
using System.Collections;
using Cinemachine;
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
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    
    
    
    [SerializeField] private float moveSpeed = 0.4f;
    [SerializeField] private float jumpSpeed = 15f;

    [SerializeField] private float slowDownTime = 2f;
    
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float screenShakeTime = 1f;

    [SerializeField] private LayerMask groundLayer;
    
    #endregion

    private GameInput input;
    private InputAction moveAction;

    private float moveInput;
    private float moveDirection;

    [SerializeField] private int jumpCounter = 0;
    [SerializeField] private int maxJumpAmount = 2;
    
    private float slowDownMove = 0f;

    private bool isDying = false;

    private Rigidbody2D rbPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animator;

    private EnemyController enemy;
    
    private GameController gameController;
    private float gameMultiplier = 1f;
    

    //private Background background;

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
        //background = FindObjectOfType<Background>();

        vCam = FindObjectOfType<CinemachineVirtualCamera>();

        gameMultiplier = gameController.GetGameSpeed();
        
        input.Player.Jump.performed += Jump;
        GameController.GameSpeedUp += SpeedUp;
        //GameController.GameFinished += GameOver;

    }

    private void Update()
    {
        //ReadInput();
        if (isDying) { return; }
        Move();
        UpdateAnimation();
        
    }


    private void OnDestroy()
    {
        input.Player.Jump.performed -= Jump;
        GameController.GameSpeedUp -= SpeedUp;
        //GameController.GameFinished -= GameOver;
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

    private void Move()
    {
        rbPlayer.velocity = new Vector2(moveSpeed-slowDownMove, rbPlayer.velocity.y);
    }

    private void SpeedUp() // Only Animation Speed Up (MoveSpeed stays the same)
    {
        gameMultiplier = gameController.GetGameSpeed();
    }
    
    private void Jump(InputAction.CallbackContext _)
    {
        
        
        if (Groundcheck())
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpSpeed);
            enemy.Jump();
            jumpCounter = 1;
        }
        else if (jumpCounter < maxJumpAmount)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpSpeed);
            enemy.Jump();
            jumpCounter++;
        }
        /*else
        {
            jumpCounter = 0;
        }*/
    }


    private bool Groundcheck()
    {
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        
        return isGrounded;
    }
    
    #endregion

    #region Animation

    private void UpdateAnimation()
    {
        Vector2 velocity = rbPlayer.velocity;
        velocity.x = MathF.Abs(velocity.x);
        animator.SetFloat(MovementSpeed, velocity.x);
        //animator.SetFloat(AnimationSpeed, (velocity.x + 1f) * gameMultiplier);
        animator.SetFloat(AnimationSpeed, gameMultiplier);
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
        if (isDying) { return; }
        StartCoroutine(SlowDown(slowDownTime));
        StartCoroutine(ScreenShake(screenShakeTime));
        animator.SetTrigger(Damaged);
    }

    private void GameOver()
    {
        rbPlayer.gravityScale = 0;
        rbPlayer.velocity = new Vector2(gameMultiplier * -46*4, 0);
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
        if (isDying) { return; }
        
        if (other.CompareTag("GameOver"))
        {
            isDying = true;
            animator.SetBool(IsDead, true);
            enemy.GetComponent<Animator>().SetBool(IsDead, true);
            //enemy.slowDownMove = 3.3f;
            //slowDownMove = 3.3f;
            enemy.GameOver();
            GameOver();
            StartCoroutine(GameOverDelayed(1));
        }

    }
    
    private IEnumerator GameOverDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        gameController.GameOver();
        yield return new WaitForSeconds(1f);
        slowDownMove = 0;
        enemy.slowDownMove = 0;

    }
}
