using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Image rotatingIcon;

    [SerializeField]
    public GameObject problemsScreen;

    [Header("Tweakables")]
    [SerializeField]
    float problemScreenDelay = 15;

    [SerializeField]
    Vector3 rotation = new Vector3(0, 0, -720);

    [SerializeField]
    float duration = 2f;

    [SerializeField]
    Ease ease = Ease.InOutBounce;

    [SerializeField]
    Color initialColor = Color.green;

    [SerializeField]
    Color targetColor = Color.yellow;

    [SerializeField]
    int vibrato = 1;

    [SerializeField]
    float elasticity = 1;

    [SerializeField]
    Vector3 punchSize = new Vector3(.5f, .5f, .5f);
   
    Sequence tween;

    private void OnEnable()
    {
        rotatingIcon.color = initialColor;
        rotatingIcon.transform.rotation = Quaternion.Euler(Vector3.zero);
        StartAnim();
        Invoke(nameof(ShowProblemScreen), problemScreenDelay);
        rotatingIcon.transform.localScale =Vector3.one;
    }

    private void OnDisable()
    {
        StopAnim();
        CancelInvoke(nameof(ShowProblemScreen));
    }
    void StartAnim()
    {
        tween = DOTween.Sequence();

        tween.Append(rotatingIcon.transform.DORotate(rotation, duration, RotateMode.FastBeyond360).SetEase(ease))
             .Insert(0, rotatingIcon.DOColor(targetColor, duration / 2).SetEase(ease))
             .Insert(0, rotatingIcon.DOColor(initialColor, duration / 2).SetEase(ease).SetDelay(duration / 2))
             .Insert(0, rotatingIcon.transform.DOPunchScale(punchSize, duration, vibrato, elasticity))
             .PrependInterval(.15f);

        tween.SetLoops(-1);
    }

    void StopAnim()
    {
        tween.Kill();
    }

    public void ShowProblemScreen()
    {
        problemsScreen.SetActive(true);
    }

  
}
