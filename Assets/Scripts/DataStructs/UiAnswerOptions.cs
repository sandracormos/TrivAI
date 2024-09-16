using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]


public class UiAnswerOptions : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI AnswerLabel;
    public Toggle Toggle;
    public Image correctImage;
    public Image wrongImage;

    [Header("Properties")]
    public Answer loadedAnswer;

    public void LoadAnswer(Answer answer)
    {
        Toggle.isOn = false;
        correctImage.enabled = wrongImage.enabled = false;

        loadedAnswer = answer;

        AnswerLabel.SetText(loadedAnswer.text);
    }

    public void CheckAnswer()
    {
        if (loadedAnswer.isCorrect)
        {
            correctImage.enabled = true;
        }
        else if(Toggle.isOn)
        {
            wrongImage.enabled = true;
        }
    }   

    public void deactivateOption()
    {
        Toggle.interactable = false;
    }
    public void activateOption()
    {
        Toggle.interactable = true;
    }
}
