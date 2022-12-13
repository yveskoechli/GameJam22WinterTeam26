
using System;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{

    #region Inspector

    [SerializeField] private float bgSpeedMultiplier = 0.1f;
    [SerializeField] private Renderer bgRenderer;

    #endregion

    private GameController gameController;
    private float gameMultiplier = 1f;

    private float bgSpeed;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        bgSpeed = gameController.GetGameSpeed() * bgSpeedMultiplier;
    }

    private void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(bgSpeed * Time.deltaTime * gameController.GetGameSpeed(), 0f);
    }
}
