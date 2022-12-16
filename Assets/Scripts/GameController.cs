using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
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

    [SerializeField] public BoxCollider2D levelBoundLeft;
    
    [SerializeField] private StudioEventEmitter musicClose;
    [SerializeField] private StudioEventEmitter musicFar;
    [SerializeField] private StudioEventEmitter musicGameOver;
    
    [SerializeField] private StudioEventEmitter atmoForest;
    
    [SerializeField] private StudioEventEmitter sfxTicTac;

    [SerializeField] private Animator vCamAnimator;

    [SerializeField] private GameObject jumpButton;
    
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
    private bool isGameOver = false;

    private String difficulty;
    
    #region Unity Event Functions
    

    

    private void Awake()
    {
        //PlayerPrefs.SetString("difficulty", "easy");
        //PlayerPrefs.Save();
        
        difficulty = PlayerPrefs.GetString("difficulty");
        SetDifficultyLevel(difficulty);
        
        //Debug.Log("Difficultylevel is "+ DifficultyLevel.difficulty);
        PlayMusic(musicFar);
        atmoForest.Play();
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

    private void Start()
    {
        
    }
    
    private void Update()
    {
        timerCounter += Time.deltaTime;
        UpdateTimerUI(timerCounter);
        
        // macht alle paar sekunden einen Speedup (Anhand speedUpTime)
        speedUpSteps = ((int)timerCounter - (int)timerCounter % speedUpTime)/ speedUpTime;
        
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
        if(!isGameOver){ SoundChanger(); }
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

    public Animator GetVCamAnimator()
    {
        return vCamAnimator;
    }

    public void SpeedUp()
    {
        float newGameSpeed = gameSpeed + 0.01f;
        DOTween.To(() => gameSpeed, x => gameSpeed = x, newGameSpeed, 1).OnComplete(() =>
        {
            Debug.Log("Speed-Up finished");
        });
        sfxTicTac.Play();
        //gameSpeed += 0.02f;
        GameSpeedUp?.Invoke();
        obstacleSpawner.IncreaseSpawnRate(0.2f);
        collectibleSpawner.DecreaseSpawnRate(0.2f);
    }

    public float GetTimeCounter()
    {
        return timerCounter;
    }

    public void GameOver()
    {
        isGameOver = true;
        PlayMusic(musicGameOver);
        //levelBoundLeft.enabled = false;
        jumpButton.gameObject.SetActive(false);
        
        retryOnce = false;
        GameFinished?.Invoke();
        fadeUI.DOShow();
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.DOShow();

        gameSpeed = gameSpeedSet;
        gameOverScreen.SetHighscore(timerText.text, difficulty);
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

        jumpButton.gameObject.SetActive(true);
        speedUpSteps = 0;
        speedUpStepOld = speedUpSteps;
        
        Time.timeScale = 1f;
        background.backgroundStop = false;
        retryOnce = true;
        levelBoundLeft.enabled = true;
        StartCoroutine(Respawn(1));
        StartCoroutine(WaitForSpawn(1));
        
        gameSpeed = gameSpeedSet;
        GameRestart?.Invoke();
        
    }
    
    public void QuitGame()
    {
        StopAllMusic();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerJump()
    {
        playerClone.GetComponent<PlayerController>().Jump();
    }

    private void SoundChanger()
    {
        var enemyPosition = enemyClone.transform.position.x;
        var playerPosition = playerClone.transform.position.x;

        var distance = playerPosition - enemyPosition;
        
        if (distance < 4f && !musicClose.IsPlaying())
        {
            PlayMusic(musicClose);
        }
        else if (distance > 6 && !musicFar.IsPlaying())
        {
            PlayMusic(musicFar);
        }
    }

    private void PlayMusic(StudioEventEmitter music) // Play a choice of Music and Stop the other music if they are actually playing
    {
        if (music.IsPlaying())
        {
            return;
        }
        
        if (music == musicClose)
        {
            if (musicFar.IsPlaying()) { musicFar.Stop(); }
            if (musicGameOver.IsPlaying()) { musicGameOver.Stop(); }
            if (!musicClose.IsPlaying()) { musicClose.Play(); }
            return;
        }
        if (music == musicFar)
        {
            if (musicClose.IsPlaying()) { musicClose.Stop(); }
            if (musicGameOver.IsPlaying()) { musicGameOver.Stop(); }
            if (!musicFar.IsPlaying()) { musicFar.Play(); }
            return;
        }
        if (music == musicGameOver)
        {
            if (musicFar.IsPlaying()) { musicFar.Stop(); }
            if (musicClose.IsPlaying()) { musicClose.Stop(); }
            if (!musicGameOver.IsPlaying()) { musicGameOver.Play(); }
            return;
        }
    }

    private void StopAllMusic()
    {
        if (musicFar.IsPlaying()) { musicFar.Stop(); }
        if (musicGameOver.IsPlaying()) { musicGameOver.Stop(); }
        if (musicClose.IsPlaying()) { musicClose.Stop(); }
        if (atmoForest.IsPlaying()) { atmoForest.Stop(); }
        
    }

    private void SetDifficultyLevel(string difficulty)
    {
        if (String.IsNullOrEmpty(difficulty))
        {
            Debug.LogWarning("Difficulty-Level not transfered!");
            return;
        }
        switch (difficulty)
        {
            case "Easy":
                gameSpeedSet = 0.1f;
                speedUpTime = 20;
                obstacleSpawner.SetSpawnRates(1f, 5f);
                collectibleSpawner.SetSpawnRates(1f, 4f);
                break;
            case "Normal":
                gameSpeedSet = 0.15f;
                speedUpTime = 15;
                obstacleSpawner.SetSpawnRates(0.8f, 4f);
                collectibleSpawner.SetSpawnRates(2f, 6f);
                break;
            case "Hard":
                gameSpeedSet = 0.2f;
                speedUpTime = 10;
                obstacleSpawner.SetSpawnRates(0.7f, 2.5f);
                collectibleSpawner.SetSpawnRates(2f, 8f);
                break;
            default:
                Debug.LogWarning("Mismatch Difficulty-Level string: " + difficulty);
                break;
        }
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
        PlayMusic(musicFar);
        isGameOver = false;
    }
    private IEnumerator WaitForSpawn(float duration)
    {
        yield return new WaitForSeconds(duration);
        obstacleSpawner.gameObject.SetActive(true);
        collectibleSpawner.gameObject.SetActive(true);
        foregroundSpawner.gameObject.SetActive(true);
    }


}
