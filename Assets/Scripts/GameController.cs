
using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private float timerCounter = 0;

    [SerializeField] private TextMeshProUGUI timerText;
    
    [SerializeField] private float gameSpeed = 1f;

    #endregion

    #region Unity Event Functions



    private void Update()
    {
        timerCounter += Time.deltaTime;
        UpdateTimerUI(timerCounter);
        
    }

    private void UpdateTimerUI(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        //mintues = time / 60;
        float seconds = Mathf.FloorToInt(time % 60);

        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
    public float GetGameSpeed()
    {
        return gameSpeed;
    }

    public void SpeedUp()
    {
        gameSpeed++;
    }

    public float GetTimeCounter()
    {
        return timerCounter;
    }
    
    

    #endregion


}
