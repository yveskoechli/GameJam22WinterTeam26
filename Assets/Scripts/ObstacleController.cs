using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float moveSpeedMultiplier = 2f;

    #endregion
    
    private GameController gameController;

    private Rigidbody2D rbObstacle;
    
    private float moveSpeed;
    
    

    private void Awake()
    {
        rbObstacle = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        moveSpeed = gameController.GetGameSpeed() * moveSpeedMultiplier;

        rbObstacle.velocity = new Vector2(moveSpeed*-1, 0);

        GameController.GameSpeedUp += SpeedUp;

    }

    private void OnDestroy()
    {
        GameController.GameSpeedUp -= SpeedUp;
    }


    private void SpeedUp()
    {
        moveSpeed = gameController.GetGameSpeed() * moveSpeedMultiplier;
        rbObstacle.velocity = new Vector2(moveSpeed*-1, 0);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
            
        }
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SlowDown();
        }
    }
}
