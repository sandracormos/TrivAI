using Singleton.Example;
using UnityEngine;

public class ScoreManager : SingletonBehaviour<ScoreManager>
{

    [Header("Properties")]
    int currentScore = 0;
    int neededXpForLevelUp = 0;
    int currentLevel = 0;


    private void OnEnable()
    {
        GlobalEvents.OnUserRegistered += GlobalEvents_OnUserRegistered;
    }
    private void OnDisable()
    {
        GlobalEvents.OnUserRegistered -= GlobalEvents_OnUserRegistered;
    }
    private void GlobalEvents_OnUserRegistered(string registered)
    {
        currentLevel = 1;
        currentScore = 0;
    }

    public int CurrentScore 
    { 
        get => currentScore; 
        set
        { 
            currentScore = value;
            OnScoreUpdated?.Invoke(currentScore);
        }
    }

    public int NeededXpForLevelUp
    {
        get => neededXpForLevelUp;
        set
        {
            neededXpForLevelUp = value;
            OnXpRequiredUpdated?.Invoke(neededXpForLevelUp);
        }
    }

    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            OnLevelUpdated?.Invoke(currentLevel);
        }
    }

    public delegate void IntDelegate(int intValue);

    public static event IntDelegate OnScoreUpdated;
    public static event IntDelegate OnXpRequiredUpdated;
    public static event IntDelegate OnLevelUpdated;

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.G))
    //    {
    //        AddXP();
    //    }
    //}

    public void AddXP(int xpToAdd = 100)
    {
        CurrentScore += xpToAdd;       

        if (CurrentScore >= NeededXpForLevelUp)
        {            
            LevelUp();
        }
    }

    public void LevelUp()
    {
        if (CurrentScore >= NeededXpForLevelUp) 
        {
            //Update level and score
            CurrentLevel++;
            CurrentScore = 0;

            SetXpNeededLabel();
        }
    }

    public void SetXpNeededLabel()
    {
        //fetch first digit of current level
        int auxLevel = CurrentLevel;

        if (auxLevel < 10)
        {
            auxLevel = 0;
        }
        else
        {
            while (auxLevel >= 10)
            {
                auxLevel /= 10;
            }
        }
        NeededXpForLevelUp = (auxLevel + 1) * 150;
    }



}
