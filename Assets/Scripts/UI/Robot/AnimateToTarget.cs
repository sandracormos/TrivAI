using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateToTarget : MonoBehaviour
{
    [SerializeField]
    RectTransform defaultPosition;

    [SerializeField]
    RectTransform targetPosition;

    [SerializeField]
    RectTransform movingElement;

    [Header("Tweakables")]
    [SerializeField]
    float duration = 0.1f;

    [SerializeField]
    Ease ease = Ease.Linear;


    public void MoveToTarget()
    {
        Move(targetPosition);
    }

    public void MoveToDefault()
    {
        Move(defaultPosition);
    }

    private void Move(RectTransform target)
    {
        movingElement.DOAnchorPos(target.anchoredPosition, duration).SetEase(ease);
    }
}
