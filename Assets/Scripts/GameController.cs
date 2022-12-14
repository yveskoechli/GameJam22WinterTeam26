using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static event Action GameFinished;
    public static event Action GameRestart;
    public static event Action GameSpeedUp;


    #region Inspector

    [SerializeField] private float timerCounter = 0;

    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private GameOverMenu gameOverScreen;
    
    [Tooltip("Set the initial overall gamespeed of the game")]
    [SerializeField] private float gameSpeedSet = 0.1f;
    
    [Tooltip("Shows the actual gamespeed of the game")]
    [SerializeField] private float gameSpeed;

    [SerializeField] private Transform playerStart;
    [SerializeField] private GameObject player;
    
    [SerializeField] private Transform enemyStart;
    [SerializeField] private GameObject enemy;

    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private ObstacleSpawner collectibleSpawner;
    [SerializeField] private ObstacleSpawner foregroundSpawner;
    
    
    
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

    private bool retryOnce = false;
    
    

    #region Unity Event Functions

    private void Awake()
    {
        gameSpeed = gameSpeedSet;
        
        enemyClone = Instantiate(enemy, enemyStart.position, enemyStart.rotation);
        playerClone = Instantiate(player, playerStart.position, playerStart.rotation);
        
        

        background = FindObjectOfType<Background>();

        background.additionalScrollSpeed = gameSpeed;
        
        //gameOverScreen = FindObjectOfType<GameOverMenu>();
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
        gameSpeed += 0.02f;
        //background.targetScrollSpeed = gameSpeed;
        GameSpeedUp?.Invoke();
        obstacleSpawner.IncreaseSpawnRate(0.2f);
    }

    public float GetTimeCounter()
    {
        return timerCounter;
    }

    public void GameOver()
    {
        Debug.Log("Game-Over triggered");
        retryOnce = false;
        GameFinished?.Invoke();
        fadeUI.DOShow();
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.DOShow();
        //StartCoroutine(DOHideDelayed(2));
        //List<GameObject> magicLights = new List<GameObject>();
        //magicLights = gameFindObjectsOfType<MagicLightController>();

        gameSpeed = gameSpeedSet;
        gameOverScreen.SetHighscore(timerText.text);
        timerCounter = 0;
        
        obstacleSpawner.gameObject.SetActive(false);
        collectibleSpawner.gameObject.SetActive(false);
        foregroundSpawner.gameObject.SetActive(false);
        
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
        if (retryOnce)
        {
            return;
        }
        retryOnce = true;
        StartCoroutine(Respawn(1));
        StartCoroutine(WaitForSpawn(1));
        
        gameSpeed = gameSpeedSet;
        GameRestart?.Invoke();

        
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
        
        enemyClone = Instantiate(enemy, enemyStart.position, enemyStart.rotation);
        playerClone = Instantiate(player, playerStart.position, playerStart.rotation);
        
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
