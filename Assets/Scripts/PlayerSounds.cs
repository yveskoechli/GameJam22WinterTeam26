
using FMODUnity;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    #region Inspector

    [SerializeField] private StudioEventEmitter stepSound;
    [SerializeField] private StudioEventEmitter jumpSound;

    [SerializeField] private PlayerController playerController;

    #endregion

    #region Animaiton Events


    public void PlaySound(AnimationEvent animationEvent)
    {
        switch (animationEvent.stringParameter)
        {
            case "Step":
                stepSound.Play();
                break;
            case "Jump":
                jumpSound.Play();
                break;
            default:
                Debug.LogWarning($"Unknown sound parameter {animationEvent.stringParameter}");
                break;
        }
    }

    public StudioEventEmitter GetJumpSound()
    {
        return jumpSound;
    }

    #endregion
}
