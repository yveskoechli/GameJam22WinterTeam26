using FMODUnity;
using UnityEngine;

public class LandSound : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter landSound;

    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floor"))
        {
            landSound.Play();
        }
    }
}
