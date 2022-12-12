using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private Transform cameraTarget;
    
    #endregion

    private GameInput input;

    private InputAction moveAction;

    private Vector2 moveInput;

    private Rigidbody2D rbPlayer;
        
    #region Unity Event Functions

    private void Awake()
    {
        input = new GameInput();
        
        moveAction = input.Player.Move;

        rbPlayer = GetComponent<Rigidbody2D>();
    }

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
        moveInput = moveAction.ReadValue<Vector2>();
    }


    #endregion

    #region Movement

    private void Move(Vector2 moveInput)
    {
        
    }

    #endregion
}
