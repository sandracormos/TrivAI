using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ui_ScoreEntry : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI rankLabel;

    [SerializeField]
    TextMeshProUGUI usernameLabel;

    [SerializeField]
    TextMeshProUGUI scoreLabel;

    public void Initialize(int rank, string username, int score)
    {
        rankLabel?.SetText(rank.ToString());
        usernameLabel?.SetText(username);
        scoreLabel?.SetText(score.ToString());
    }
}
