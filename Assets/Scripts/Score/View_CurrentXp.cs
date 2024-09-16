using TMPro;
using UnityEngine;

public class View_CurrentXp : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    private void OnEnable()
    {
        ScoreManager.OnScoreUpdated += ScoreManager_OnScoreUpdated;

        ScoreManager_OnScoreUpdated(ScoreManager.Instance.CurrentScore);
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreUpdated -= ScoreManager_OnScoreUpdated;
    }
    private void ScoreManager_OnScoreUpdated(int currentScore)
    {
        label?.SetText(currentScore.ToString());
    }
}
