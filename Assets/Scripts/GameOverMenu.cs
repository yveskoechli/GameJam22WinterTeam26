using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    //public static event Action GameRestart;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private CanvasGroup canvasGroup;

    #region Unity Event Functions
    private void Awake()
    {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }

    public void SetHighscore(string highscore)
    {
        highscoreText.text = highscore;
    }

    #endregion
    
    #region Animations

    public Tween DOShow()
    {
        this.DOKill();
        Sequence sequence = DOTween.Sequence(this)
            .Append(DOFade(1).From(0));
        return sequence;    
    }
    
    public Tween DOHide()
    {
        this.DOKill();
        Sequence sequence = DOTween.Sequence(this)
            .Append(DOFade(0).From(1));
        return sequence;    
    }
    

    private TweenerCore<float, float, FloatOptions> DOFade(float targetAlpha)
    {
        return canvasGroup.DOFade(targetAlpha, 0.75f).SetEase(Ease.InOutSine);
    }

    #endregion
}
