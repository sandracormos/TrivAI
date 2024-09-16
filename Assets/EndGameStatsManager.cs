using PlayFab.ClientModels;
using Singleton.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EndGameStatsManager : SingletonBehaviour<EndGameStatsManager>
{
    [Header("References")]
    public TextMeshProUGUI numberOfCorrectAnsweredQuestions;
    public TextMeshProUGUI numberOfWrongAnsweredQuestions;
    public TextMeshProUGUI score;

    [SerializeField]
    public TextMeshProUGUI currentUserUsername;
    [SerializeField]
    public TextMeshProUGUI currentUserScore;
    [SerializeField]
    public TextMeshProUGUI currentUserRank;

    [SerializeField]
    public Ui_ScoreEntry scoreEntryPrefab;

    [SerializeField]
    public RectTransform scoreEntryContent;

    [Header("Properties")]
    [SerializeField]
    List<Ui_ScoreEntry> spawnedUiEntries = new();

    protected override void Awake()
    {
        base.Awake();

        GlobalEvents.OnEndGame += GlobalEvents_OnEndGame;
        GlobalEvents.OnBackToSetupMenu += GlobalEvents_OnBackToSetupMenu;
        GlobalEvents.OnLeaderboardReceived += GlobalEvents_OnLeaderboardReceived;
        GlobalEvents.OnBackToSetupMenu += GlobalEvents_OnBackToSetupMenu;
    }

  
    private void OnDestroy()
    {
        GlobalEvents.OnEndGame -= GlobalEvents_OnEndGame;
        GlobalEvents.OnBackToSetupMenu -= GlobalEvents_OnBackToSetupMenu;
        GlobalEvents.OnLeaderboardReceived -= GlobalEvents_OnLeaderboardReceived;

        GlobalEvents.OnBackToSetupMenu -= GlobalEvents_OnBackToSetupMenu;
    }

    private void GlobalEvents_OnBackToSetupMenu(string username)
    {
        gameObject.SetActive(false);
    }

    private void GlobalEvents_OnEndGame(string endGame)
    {
        gameObject.SetActive(true);
        numberOfCorrectAnsweredQuestions.text = References.Instance.numberOfCorrectAnsweredQuestions.ToString();
        numberOfWrongAnsweredQuestions.text = (References.Instance.numberOfAnsweredQuestions - int.Parse(numberOfCorrectAnsweredQuestions.text)).ToString();
        score.text = (int.Parse(numberOfCorrectAnsweredQuestions.text) * 10).ToString();
    }

    private void GlobalEvents_OnLeaderboardReceived(List<PlayerLeaderboardEntry> entries)
    {
        Debug.Log($"Needed Id : {References.Instance.UserId} in : {string.Join("-", entries.Select(e => e.PlayFabId))}");
        var currentPlayerEntry = entries.FirstOrDefault(e => e.PlayFabId == References.Instance.UserId);
        if (currentPlayerEntry is not null)
        {
            References.Instance.rank = currentPlayerEntry.Position;
            currentPlayerEntry.StatValue = ((ScoreManager.Instance.CurrentLevel * 100) + ScoreManager.Instance.CurrentScore);
        }
        else
        {
            var playerEntry = new PlayerLeaderboardEntry()
            {
                DisplayName = References.Instance.Username,
                StatValue = ((ScoreManager.Instance.CurrentLevel * 100) + ScoreManager.Instance.CurrentScore)
            };
            entries.Add(playerEntry);
            entries.OrderBy(e => e.StatValue);
        }

        ClearAllEntries();
        SpawnNewLeaderboardEntries(entries);       

        HighlightUserRank();
    }


    public void HighlightUserRank()
    {
        int level = ScoreManager.Instance.CurrentLevel;
        int xp = ScoreManager.Instance.CurrentScore;
        
        currentUserRank.text = (References.Instance.rank).ToString();
        currentUserScore.text =(( level *100) + xp).ToString();
        currentUserUsername.text = References.Instance.Username.ToString();
    }

    private void SpawnNewLeaderboardEntries(List<PlayerLeaderboardEntry> entries)
    {
        int i = 1;
        foreach (var entry in entries)
        {
            var spawnedEntry = Instantiate(scoreEntryPrefab, scoreEntryContent);
            spawnedEntry.Initialize(i++, entry.DisplayName, entry.StatValue);
            spawnedUiEntries.Add(spawnedEntry);
        }
    }

    public void ClearAllEntries()
    {
        spawnedUiEntries.ForEach(x => Destroy(x.gameObject));
        spawnedUiEntries.Clear();
    }

    public void pressedButtonBackToMenu()
    {
        gameObject.SetActive(false);
        GlobalEvents.InvokeOnBackToSetupMenu("");

    }
}
