using System;
using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    
    #region Inspector

    [SerializeField] public float additionalScrollSpeed;

    [SerializeField] private GameObject[] backgrounds;

    [SerializeField] private float[] scrollSpeed;
    

    #endregion

    public bool backgroundStop = false;

    public float targetScrollSpeed;


    #region Unity Event Functions

    private void FixedUpdate()
    {
        if (backgroundStop) { return; }
        
        for (int background = 0; background < backgrounds.Length; background++)
        {
            Renderer rend = backgrounds[background].GetComponent<Renderer>();

            float offset = Time.time * (scrollSpeed[background] + additionalScrollSpeed);

            if (additionalScrollSpeed + 0.001f <= targetScrollSpeed)
            {
                additionalScrollSpeed = Mathf.Lerp(additionalScrollSpeed, targetScrollSpeed, Time.deltaTime);
            }
            /*else{
                additionalScrollSpeed = targetScrollSpeed;
            }*/
            rend.material.SetTextureOffset(MainTex, new Vector2(offset, 0));
        }
    }

    #endregion

}
