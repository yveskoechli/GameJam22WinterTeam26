using System;
using System.Collections;
using UnityEngine;

public class Background : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    
    #region Inspector

    [SerializeField] public float additionalScrollSpeed;

    [SerializeField] private GameObject[] backgrounds;

    [SerializeField] private float[] scrollSpeed;

    [SerializeField] private float speedUpRate = 0.1f;
    

    #endregion

    public bool backgroundStop = false;

    [SerializeField] private float targetScrollSpeed;

    private GameController gameController;

    private bool hasSpeedUp = false;

    private float lerp = 0f;

    private float offset = 0f;
    private float currentOffset = 0f;

    private float additionalScrollSpeedOld;
    
    #region Unity Event Functions

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        
        GameController.GameSpeedUp += SpeedUp;
        GameController.GameRestart += Restart;
        GameController.GameFinished += GameOver;
    }

    private void Start()
    {
        additionalScrollSpeed = gameController.GetGameSpeed();
    }

    private void OnDestroy()
    {
        GameController.GameSpeedUp -= SpeedUp;
        GameController.GameRestart -= Restart;
        GameController.GameFinished -= GameOver;
    }

    private void FixedUpdate()
    {
        if (backgroundStop) { return; }
        
        additionalScrollSpeed = gameController.GetGameSpeed();
        Debug.Log("Scrollspeed: " + additionalScrollSpeed);
        for (int background = 0; background < backgrounds.Length; background++)
        {
            Renderer rend = backgrounds[background].GetComponent<Renderer>();
            offset = (Time.time) * (scrollSpeed[background] * additionalScrollSpeed);
            //currentOffset = Mathf.Lerp(currentOffset, offset, 3);
            //Debug.Log("Current Offset: " +currentOffset + " of Background No: "+ background);
            rend.material.SetTextureOffset(MainTex, new Vector2(offset, 0));
            //offset += (scrollSpeed[background] * additionalScrollSpeed) * Time.deltaTime * speedUpRate;
            /*if (hasSpeedUp)
            {
                offset = Time.time * (scrollSpeed[background] * additionalScrollSpeed) + 
                         (additionalScrollSpeedOld-additionalScrollSpeed)*scrollSpeed[background]*0.01f*Time.deltaTime;
                rend.material.SetTextureOffset(MainTex, new Vector2(offset, 0));
            }
            else
            {
                offset = Time.time * (scrollSpeed[background] * additionalScrollSpeed);
                rend.material.SetTextureOffset(MainTex, new Vector2(offset, 0));
            }
            
            */
            /*
            if (additionalScrollSpeed + 0.001f <= targetScrollSpeed)
            {
                additionalScrollSpeed = Mathf.Lerp(additionalScrollSpeed, targetScrollSpeed, 0.0001f);
            }
            else if (additionalScrollSpeed + 0.0005f <= targetScrollSpeed)
            {
                additionalScrollSpeed = targetScrollSpeed;
            }
            else{
                additionalScrollSpeed = targetScrollSpeed;
            }*/
            
        }
        //if (!hasSpeedUp)
        //{
          //  return;
        //}
        
        /*if (additionalScrollSpeed <= targetScrollSpeed)
        {
            additionalScrollSpeed += 0.01f * Time.deltaTime;
            //additionalScrollSpeed = Mathf.Lerp(additionalScrollSpeed, targetScrollSpeed, 0.015f*Time.deltaTime);
            //lerp += 1f * Time.deltaTime;
        }
        else
        {
            additionalScrollSpeed = targetScrollSpeed;
            hasSpeedUp = false;
            
        }
        Debug.Log("Additional: "+ additionalScrollSpeed + "   Target:" + targetScrollSpeed);*/
    }

    private void SpeedUp()
    {
        //targetScrollSpeed = gameController.GetGameSpeed();
        //additionalScrollSpeedOld = additionalScrollSpeed;
        hasSpeedUp = true;
        //lerp = 0;
        //additionalScrollSpeed = gameController.GetGameSpeed();
    }

    private void GameOver()
    {
        StartCoroutine(StopBackgroundDelayed(0.1f));
    }

    private void Restart()
    {
        backgroundStop = false;
        targetScrollSpeed = gameController.GetGameSpeed();
        additionalScrollSpeed = targetScrollSpeed;
    }
    
    private IEnumerator StopBackgroundDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        backgroundStop = true;
    }
    
    #endregion

}
