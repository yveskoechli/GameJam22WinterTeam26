using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using UnityEngine;

public class FadeUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    #region Unity Event Functions

    

    #endregion
    private void Awake()
    {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }

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
