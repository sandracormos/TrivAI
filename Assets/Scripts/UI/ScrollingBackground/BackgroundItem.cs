using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackgroundItem : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI label;

    [SerializeField]
    float minDuration = 10;
    [SerializeField]
    float maxDuration = 30;

    [SerializeField]
    float minAlpha = 0.05f;

    [SerializeField]
    float maxAlpha = 0.5f;

    [SerializeField]
    float minFontSize = 36;

    [SerializeField]
    float maxFontSize = 100;

    [SerializeField]
    List<TMP_FontAsset> fonts;

    public void Initialize(string s)
    {
        label?.SetText(s);
        label.font = fonts[Random.Range(0, fonts.Count)];   
        label.alpha = Random.Range(minAlpha, maxAlpha);
        label.fontSize = Random.Range(minFontSize, maxFontSize);

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        transform.position = new Vector3(Random.Range(0, Screen.width), -300, 0);

        transform.DOMoveY(Screen.height + 300, Random.Range(minDuration, maxDuration)).SetEase(Ease.Linear).OnComplete(Reset);
    }

    private void Reset()
    {
        Initialize(label.text);
    }
}
