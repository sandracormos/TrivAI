using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents
{
    public delegate void StringDelegate(string username);
    public delegate void QuestionDelegate (Question question);
    public delegate void LeaderboardList(List<PlayerLeaderboardEntry> entries);


    public static event StringDelegate OnUserLoggedIn;
    public static event StringDelegate OnUserFailedLogin;

    public static event StringDelegate OnUserRegistered;
    public static event StringDelegate OnRegistrationFailed;

    public static event StringDelegate OnUserDataReceived;
    public static event StringDelegate OnFailedToRetrieveUserData;

    public static event QuestionDelegate OnQuesitonFetchSuccess;
    public static event QuestionDelegate OnQuestionFetchFailed;

    public static event StringDelegate OnCategoriesUpdated;

    public static event StringDelegate OnDifficultySelected;

    public static event StringDelegate OnStartGame;

    public static event StringDelegate OnEndGame;

    public static event StringDelegate OnBackToSetupMenu;

    public static event LeaderboardList OnLeaderboardReceived;

    public static event StringDelegate OnSignOut;




    public static void InvokeUserLoggedIn(string username)
    {
        Debug.Log($"[GlobalEvents] Invoking OnUserLoggedIn.");
        OnUserLoggedIn?.Invoke(username);
    }

    public static void InvokeUserFailedLogin(string username)
    {
        Debug.Log($"[GlobalEvents] Invoking OnUserFailedLogin.");
        OnUserFailedLogin?.Invoke(username);
    }

    public static  void InvokeUserRegistered(string username)
    {
        Debug.Log($"[GlobalEvents] Invoking OnUserRegistered.");
        OnUserRegistered?.Invoke(username);
    }

    public static void InvokeUserDataReceived(string username)
    {
        Debug.Log($"[GlobalEvents] Invoking OnUserDataReceived.");
        OnUserDataReceived?.Invoke(username);
    }
    public static void InvokeFailedToRetrieveUserData()
    {
        Debug.Log($"[GlobalEvents] Invoking OnFailedToRetrieveUserData.");
        OnFailedToRetrieveUserData?.Invoke(string.Empty);
    }

    public static void InvokeOnQuestionFetchSuccess(Question question)
    {
        Debug.Log($"[GlobalEvents] Invoking InvokeOnQuestionFetchSuccess.");
        OnQuesitonFetchSuccess?.Invoke(question);
    }

    public static void InvokeOnCategoriesUpdated(string categories)
    {
        Debug.Log($"[GlobalEvents] Invoking OnCategoriesUpdated.");
        OnCategoriesUpdated?.Invoke(categories);
    }
    public static void InvokeOnDifficultySelected(string selectedDifficulty)
    {
        Debug.Log($"[GlobalEvents] Invoking OnDifficultySelected.");
        OnCategoriesUpdated?.Invoke(selectedDifficulty);
    } 
    public static void InvokeOnStartGame(string s)
    {
        Debug.Log($"[GlobalEvents] Invoking OnStartGame.");
        OnStartGame?.Invoke(s);
    }
    public static void InvokeOnEndGame(string s)
    {
        Debug.Log($"[GlobalEvents] Invoking OnEndGame.");
        OnEndGame?.Invoke(s);
    }

    public static void InvokeOnBackToSetupMenu(string end)
    {
        Debug.Log($"[GlobalEvents] Invoking OnBackToSetupMenu.");
        OnBackToSetupMenu?.Invoke(end);
    }

    public static void InvokeOnLeaderboardReceived(List<PlayerLeaderboardEntry> end)
    {
        Debug.Log($"[GlobalEvents] Invoking OnLeaderboardReceived.");
        OnLeaderboardReceived?.Invoke(end);
    }
     
    public static void InvokeOnSignOut(string signOut)
    {
        Debug.Log($"[GlobalEvents] Invoking OnSignOut.");
        OnSignOut?.Invoke(signOut);
    }

}
