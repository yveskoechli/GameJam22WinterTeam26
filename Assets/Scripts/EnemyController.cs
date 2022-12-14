using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
#region Inspector

    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    
    
    [SerializeField] private float moveSpeed = 0.6f;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] public float slowDownMove = 0f;
    

    [SerializeField] private float delayToPlayer = 0.5f;
    
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private LayerMask groundLayer;
    
    #endregion

    //private GameInput input;
    //private InputAction moveAction;

    //private float moveInput;
    private float moveDirection;

    public bool isKilling = false;

   
    

    private Rigidbody2D rbPlayer;
    private SpriteRenderer spritePlayer;
    private Animator animator;

    private GameController gameController;
    private float gameMultiplier = 1f;



    #region Unity Event Functions

    private void Awake()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        gameController = FindObjectOfType<GameController>();
        GameController.GameSpeedUp += SpeedUp;
        //GameController.GameFinished += GameOver;
    }

    private void Update()
    {
        //ReadInput();
        if (!isKilling)
        {
            Move();
        }
        

        UpdateAnimation();

        gameMultiplier = gameController.GetGameSpeed();
    }


    private void OnDestroy()
    {
        GameController.GameSpeedUp -= SpeedUp;
        //GameController.GameFinished -= GameOver;
    }
    
    #endregion
    

    #region Movement

    private void Move()
    {
        rbPlayer.velocity = new Vector2((moveSpeed) - slowDownMove, rbPlayer.velocity.y);
    }

    private void SpeedUp() // Only Animation Speed Up (MoveSpeed stays the same)
    {
        gameMultiplier = gameController.GetGameSpeed();
    }
    
    public void Jump()
    {
        StartCoroutine(JumpDelayed(delayToPlayer));
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
        StartCoroutine(SlowDown(1f));
        animator.SetTrigger(Damaged);
    }

    public void GameOver()
    {
        rbPlayer.gravityScale = 0;
        rbPlayer.velocity = new Vector2(gameMultiplier * -46*4, 0);
    }
    
    private IEnumerator JumpDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        //if (Groundcheck())
        //{
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpSpeed);
        //}
       
    }
    
    private IEnumerator SlowDown(float time)
    {
        slowDownMove = 2f;
        //DisableInput();
        yield return new WaitForSeconds(time);
        slowDownMove = 0f;
        //EnableInput();
    }
}
