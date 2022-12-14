using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //public static event Action GameFinished;


    #region Inspector

    [SerializeField] private float timerCounter = 0;

    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private GameOverMenu gameOverScreen;
    
    [SerializeField] private float gameSpeed = 0.1f;

    [SerializeField] private Transform playerStart;
    [SerializeField] private GameObject player;
    
    [SerializeField] private Transform enemyStart;
    [SerializeField] private GameObject enemy;

    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private ObstacleSpawner collectibleSpawner;
    
    
    [Tooltip("Gibt alle ?s einen Speedschub für den Gamespeed")]
    [SerializeField] private int speedUpTime = 10;

    #endregion

    private FadeUI fadeUI;

    private Background background;
    
    private GameObject playerClone;
    private GameObject enemyClone;

    private bool canSpeedUp = false;
    private int speedUpSteps = 0;
    private int speedUpStepOld = 0;

    #region Unity Event Functions

    private void Awake()
    {
        enemyClone = Instantiate(enemy, enemyStart.position, enemyStart.rotation);
        playerClone = Instantiate(player, playerStart.position, playerStart.rotation);


        background = FindObjectOfType<Background>();
        //playerClone = FindObjectOfType<PlayerController>().gameObject;
        //enemyClone = FindObjectOfType<EnemyController>().gameObject;

        background.additionalScrollSpeed = gameSpeed;
        
        gameOverScreen = FindObjectOfType<GameOverMenu>();
        gameOverScreen.gameObject.SetActive(false);
        
        fadeUI = FindObjectOfType<FadeUI>();
        fadeUI.DOHide();
        
        
    }


    private void Update()
    {
        timerCounter += Time.deltaTime;
        UpdateTimerUI(timerCounter);
        
        // macht alle paar sekunden einen Speedup (Anhand speedUpTime)
        speedUpSteps = ((int)timerCounter - (int)timerCounter % 10)/ speedUpTime;
        
        if (speedUpSteps > speedUpStepOld)
        {
            canSpeedUp = true;
            speedUpStepOld = speedUpSteps;
        }
        
        if (canSpeedUp)
        {
            SpeedUp();
            canSpeedUp = false;
        }
        
    }

    private void UpdateTimerUI(float time)
    {
        int seconds = (int)time % 60;
        int minutes = ((int)time - seconds) / 60;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
    public float GetGameSpeed()
    {
        return gameSpeed;
    }

    public void SpeedUp()
    {
        gameSpeed += 0.01f;
        background.targetScrollSpeed = gameSpeed;
    }

    public float GetTimeCounter()
    {
        return timerCounter;
    }

    public void GameOver()
    {
        //GameFinished?.Invoke();
        fadeUI.DOShow();
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.DOShow();
        //StartCoroutine(DOHideDelayed(2));
        //List<GameObject> magicLights = new List<GameObject>();
        //magicLights = gameFindObjectsOfType<MagicLightController>();

        gameSpeed = 1;
        gameOverScreen.SetHighscore(timerText.text);
        timerCounter = 0;
        
        obstacleSpawner.gameObject.SetActive(false);
        collectibleSpawner.gameObject.SetActive(false);
        
        var magicLights = FindObjectsOfType<MagicLightController>();

        foreach (MagicLightController light in magicLights)
        {
            Destroy(light.gameObject);
        }
        
        var obstacles = FindObjectsOfType<ObstacleController>();

        foreach (ObstacleController obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        
        


    }

    public void RestartGame()
    {
        StartCoroutine(Respawn(1));
        StartCoroutine(WaitForSpawn(1));
        //gameOverScreen.DOHide();
    }
    
    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
    
    private IEnumerator Respawn(float duration)
    {
        Destroy(playerClone);
        Destroy(enemyClone);
        yield return new WaitForSeconds(duration);
        //playerClone.transform.SetPositionAndRotation(playerStart.position, playerStart.rotation);
        //enemyClone.transform.SetPositionAndRotation(enemyStart.position, enemyStart.rotation);
        enemyClone = Instantiate(enemy, enemyStart.position, enemyStart.rotation);
        playerClone = Instantiate(player, playerStart.position, playerStart.rotation);
        
        //Instantiate(player, playerStart.position, playerStart.rotation);
        fadeUI.DOHide();
        gameOverScreen.DOHide();
    }
    
    private IEnumerator WaitForSpawn(float duration)
    {
        yield return new WaitForSeconds(duration);
        obstacleSpawner.gameObject.SetActive(true);
        collectibleSpawner.gameObject.SetActive(true);
    }


}
