using TMPro;
using UnityEngine;

public class View_XpRequired : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    private void OnEnable()
    {
        ScoreManager.OnXpRequiredUpdated += ScoreManager_OnXpRequiredUpdated;
        ScoreManager_OnXpRequiredUpdated(ScoreManager.Instance.NeededXpForLevelUp);
    }

    private void OnDisable()
    {
        ScoreManager.OnXpRequiredUpdated -= ScoreManager_OnXpRequiredUpdated;
    }
    private void ScoreManager_OnXpRequiredUpdated(int xpRequired)
    {
        label?.SetText(xpRequired.ToString()); 
    }
}
