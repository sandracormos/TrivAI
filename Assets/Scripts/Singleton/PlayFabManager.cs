using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Singleton.Example;


public class PlayFabManager : SingletonBehaviour<PlayFabManager>
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [SerializeField]
    public TextMeshProUGUI initialScore;

    [SerializeField]
    public TextMeshProUGUI currentLevel;

    [SerializeField]
    public GameObject loginCanvas;

    [Header("UI")]
    public TextMeshProUGUI messageText;


    protected override void Awake()
    {
        base.Awake();
        GlobalEvents.OnEndGame += GlobalEvents_OnEndGame;
    }   

    private void OnDestroy()
    {
        GlobalEvents.OnEndGame -= GlobalEvents_OnEndGame;
    }
    private void GlobalEvents_OnEndGame(string username)
    {
        SendLeaderboard ( (ScoreManager.Instance.CurrentLevel *100 )+ ScoreManager.Instance.CurrentScore) ;
        SaveUsersGameDataOnEndGame(ScoreManager.Instance.CurrentLevel, ScoreManager.Instance.CurrentScore);
    }


    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password to short";
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }


    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registered and logged in!";
        loginCanvas.SetActive(false);

        References.Instance.UserId = result.PlayFabId;
        GlobalEvents.InvokeUserRegistered(string.Empty);
        GetUsersOpenAiId();
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }
    public void LoginButton(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged in";
        Debug.Log("[PlayFabManager] Successful login/account create!");

        References.Instance.UserId = result.PlayFabId;
        Debug.Log($"Logged with PlayFabID: {result.PlayFabId}");
        GetStatistics();
        loginCanvas.SetActive(false);

        GlobalEvents.InvokeUserLoggedIn(string.Empty);
        GetUsersOpenAiId();
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "D541C"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent";
    }


    void OnError(PlayFabError error) 
    {       
        messageText.text = string.Empty;
        string invalidEmail = "Email address is not valid";
        string invalidPassword = "Password must be between 6 and 100 characters";
        string wrongCredentials = "Invalid email address or password";
        string unavailableEmail = "Email address already exists";

        string result = string.Empty;
        int colonIndex= error.GenerateErrorReport().IndexOf(":");
        if (colonIndex != -1)
        {
            result = error.GenerateErrorReport().Substring(colonIndex+1).Trim();
        }
        if(result.Contains(invalidEmail))
        {
            messageText.text += invalidEmail + "\n";
        }
        if (result.Contains(invalidPassword))
        {
            messageText.text += invalidPassword + "\n";
        }
        if (result.Contains(wrongCredentials))
        {
            messageText.text+= wrongCredentials + "\n";
        }
        if (result.Contains(unavailableEmail))
        {
            messageText.text += unavailableEmail + "\n";
        }
        if (result.Contains("User not found"))
        {
            messageText.text = "User not found";
        }
        Debug.Log("error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlatformScore", // Specify the name of the statistic used for the leaderboard
            StartPosition = 0
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("Received leaderboard data:");
        GlobalEvents.InvokeOnLeaderboardReceived(result.Leaderboard);
    }

    void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get leaderboard data: " + error.GenerateErrorReport());
    }


    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "PlatformScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnLeaderboardUpdateFailed);
    }

    
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("[PlayFabManager] Successfull leaderboard sent");
        GetLeaderboard();
    }
    void OnLeaderboardUpdateFailed(PlayFabError error)
    {
        Debug.Log("[PlayFabManager] Failed to load updated leaderboard");
        GetLeaderboard();
    }

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);

            initialScore.SetText(eachStat.Value.ToString());
        }
    }

    public void GetUsersOpenAiId()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    //Get Users ThreadId and username 
    void OnDataReceived(GetUserDataResult result)
    {
        if(result.Data is not null && result.Data.ContainsKey("ThreadId") && result.Data.ContainsKey("AssistantId") && !string.IsNullOrEmpty(result.Data["ThreadId"].Value) && !string.IsNullOrEmpty(result.Data["AssistantId"].Value) )
        {
            Debug.Log("[PlayFabManager] Received user data");
            Debug.Log("ThreadId: " + result.Data["ThreadId"].Value);
            Debug.Log("AssistantId: " + result.Data["AssistantId"].Value);
            //References.Instance.OpenAi.ThreadId = result.Data["ThreadId"].Value;
            //References.Instance.OpenAi.AssistantId = result.Data["AssistantId"].Value;
            GlobalEvents.InvokeUserDataReceived(References.Instance.OpenAi.ThreadId);

            if(int.TryParse(result.Data["Level"].Value, out int currentLevel))
            {
                ScoreManager.Instance.CurrentLevel = currentLevel;
            }
            else
            {
                ScoreManager.Instance.CurrentLevel = 0;
            }

            if (int.TryParse(result.Data["Score"].Value, out int currentScore))
            {
                ScoreManager.Instance.CurrentScore = currentScore;
            }
            else
            {
                ScoreManager.Instance.CurrentScore = 0;
            }

            ScoreManager.Instance.SetXpNeededLabel();
        }
        else
        {
            GlobalEvents.InvokeFailedToRetrieveUserData();
            Debug.Log("[PlayFabManager] data not found");
        }
    }

    public void SaveUsersOpenAiId()
    {
        Debug.Log("[PlayFabManager] Saving user data...");
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"ThreadId", References.Instance.OpenAi.ThreadId },
                {"AssistantId", References.Instance.OpenAi.AssistantId },
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void ChangeDisplayName()
    {
        Debug.Log($"[PlayFabManager] Updating displayName to : {References.Instance.Username}");
        var request = new UpdateUserTitleDisplayNameRequest
        {
           
            DisplayName = References.Instance.Username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdateSuccess, OnDisplayNameUpdateFailure);
    }
    private void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully to: " + result.DisplayName);
    }

    private void OnDisplayNameUpdateFailure(PlayFabError error)
    {
        Debug.LogError("Display name update failed: " + error.GenerateErrorReport());
    }
    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("[PlayFabManager] Succesful user data send");
    }


    public void SaveUsersGameDataOnEndGame(int level, int score)
    {
        Debug.Log("data sent to playfab");
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Level", level.ToString() },
                {"Score", score.ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

}
