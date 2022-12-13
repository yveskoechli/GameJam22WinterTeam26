
using UnityEngine;

public class MagicLightController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float moveSpeedMultiplier = 2f;

    #endregion
    
    private GameController gameController;

    private Rigidbody2D rbLight;
    
    private float moveSpeed;

    private void Awake()
    {
        rbLight = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        moveSpeed = gameController.GetGameSpeed() * moveSpeedMultiplier;

        rbLight.velocity = new Vector2(moveSpeed*-1, 0);
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().SlowDown();
            Debug.Log(other.tag);
        }
    }
}
