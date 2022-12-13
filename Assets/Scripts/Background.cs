using System;
using UnityEngine;

public class Background : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    
    #region Inspector

    [SerializeField] private float additionalScrollSpeed;

    [SerializeField] private GameObject[] backgrounds;

    [SerializeField] private float[] scrollSpeed;
    

    #endregion


    #region Unity Event Functions

    private void FixedUpdate()
    {
        for (int background = 0; background < backgrounds.Length; background++)
        {
            Renderer rend = backgrounds[background].GetComponent<Renderer>();

            float offset = Time.time * (scrollSpeed[background] + additionalScrollSpeed);
            
            rend.material.SetTextureOffset(MainTex, new Vector2(offset, 0));
        }
    }

    #endregion

}
