using TMPro;
using UnityEngine;

public class View_CurrentLevel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;


    private void OnEnable()
    {
        ScoreManager.OnLevelUpdated += ScoreManager_OnXpRequiredUpdated;
        ScoreManager_OnXpRequiredUpdated(ScoreManager.Instance.CurrentLevel);
    }

    private void OnDisable()
    {
        ScoreManager.OnLevelUpdated -= ScoreManager_OnXpRequiredUpdated;
    }
    private void ScoreManager_OnXpRequiredUpdated(int currentLevel)
    {
        label?.SetText(currentLevel.ToString());
    }
}
