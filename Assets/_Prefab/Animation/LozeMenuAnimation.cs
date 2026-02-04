using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LozeMenuAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _restartButton, _exitButton;
    [SerializeField] private Image _panelImage;

    private void OnEnable()
    {
        _panelImage.DOFade(100 , 0.8f).SetEase(Ease.OutCubic); 
        _restartButton.transform.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            _restartButton.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutBack);
        });
        _exitButton.transform.DOLocalMoveY(-130, 0.5f).SetDelay(0.9f).SetEase(Ease.OutQuint);
    }
}

