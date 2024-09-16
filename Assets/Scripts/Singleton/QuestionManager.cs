using Singleton.Example;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [Header("Tweakables")]
    public int maxBuffer = 10;

    [Header("OpenAi References")]
    public List<UiAnswerOptions> uiAnswerOptions = new();
    public ToggleGroup toggleGroup;
    public TextMeshProUGUI questionName;
    public OpenAi_OriginalMessagePoster messagePoster;
    public OpenAi_CreateAssistantThreadRunner runThread;
    public OpenAi_ResultMessageFetcher content;
    public PlayFabManager playFabManager;

    [Header("Buttons")]
    [SerializeField]
    public Button helpMethod_FiftyFifty;
    [SerializeField]
    public Button helpMethod_GetCorrectAnswer;
    [SerializeField]
    public Button helpMethod_GetTipFromRobot;
    [SerializeField]
    public GameObject exitPopUp;
    [SerializeField]
    public Button submitButton;


    [Header("Properties")]
    public List<Question> questions = new();
    public Question currentQuestion;
    public int questionCount = 0;
    [SerializeField]
    public Image triggerWarning;
    [SerializeField]
    public TextMeshProUGUI tipFromRobot;
    [SerializeField]
    public Image helpMethod_TipBackground;

    [SerializeField]
    float delayToLoadNextQuestion = 3f;
    double timeFromLastAnswerChecked = 0;

    [Header("Panels")]
    [SerializeField]
    LoadingPanel loadingPanel;
    [SerializeField]
    GameObject problemPanel;



    private void Awake()
    {
        GlobalEvents.OnQuesitonFetchSuccess += GlobalEvents_OnQuesitonFetchSuccess;
        GlobalEvents.OnStartGame += GlobalEvents_OnStartGame;
        GlobalEvents.OnEndGame += GlobalEvents_OnEndGame;

    }  

    private void OnDestroy()
    {
        GlobalEvents.OnQuesitonFetchSuccess -= GlobalEvents_OnQuesitonFetchSuccess;   
        GlobalEvents.OnStartGame -= GlobalEvents_OnStartGame;
        GlobalEvents.OnEndGame -= GlobalEvents_OnEndGame;
    }

    private void GlobalEvents_OnEndGame(string username)
    {
        gameObject.SetActive(false);
    }

    private void GlobalEvents_OnStartGame(string username)
    {
        problemPanel.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(true);

        tipFromRobot.gameObject.SetActive(false);
        helpMethod_TipBackground.gameObject.SetActive(false);
        gameObject.SetActive(true);

        ClearQuestionList();
        currentQuestion = null;
        questionCount = 0;
        References.Instance.numberOfCorrectAnsweredQuestions = 0;
        References.Instance.numberOfAnsweredQuestions = 0;
        References.Instance.OpenAiCreateAssistant.PostAssistant();
        References.Instance.OpenAiCreateThread.PostThread();


        helpMethod_FiftyFifty.gameObject.SetActive(true);

        helpMethod_GetCorrectAnswer.gameObject.SetActive(true);
        helpMethod_GetTipFromRobot.gameObject.SetActive(true);
        helpMethod_TipBackground.gameObject.SetActive(true);

    }
 
    private void GlobalEvents_OnQuesitonFetchSuccess(Question question)
    {
        if (!questions.Contains(question))
        {
            questions.Add(question);

            //Assign current question as first
            if (questions.Count <= 1 && question is not null && 
                Time.timeSinceLevelLoadAsDouble - timeFromLastAnswerChecked > delayToLoadNextQuestion)
            {
                LoadNextQuestion();
            }
        }

        if (questions.Count < maxBuffer)
        {
            StartCoroutine(FetchAnotherQuestion());
        }
    }

    public void AddQuestionToList(Question question)
    {
        questions.Add(question);
    }


    public void LoadNextQuestion()
    {
        helpMethod_FiftyFifty.interactable = true;
        helpMethod_GetCorrectAnswer.interactable = true;
        helpMethod_GetTipFromRobot.interactable = true;
        References.Instance.numberOfAnsweredQuestions++;
        if (questions.Count == 0)
            return;

        loadingPanel.gameObject.SetActive(false);

        //enables submit button
        submitButton.enabled = true;
        tipFromRobot.gameObject.SetActive(false);
        helpMethod_TipBackground.gameObject.SetActive(false);
        questionCount ++;
        currentQuestion = questions[0];     
        triggerWarning.gameObject.SetActive(false);

        uiAnswerOptions.ForEach(o => o.gameObject.SetActive(true));
        uiAnswerOptions.ForEach(o => o.Toggle.interactable = false);

        toggleGroup.enabled = !(currentQuestion.Answers.Count(a => a.isCorrect)  > 1);
        questionName.SetText(currentQuestion.QuestionName);
        for (int i = 0; i < currentQuestion.Answers.Count; i++)
        {
            uiAnswerOptions[i].LoadAnswer(currentQuestion.Answers[i]);
        }
        uiAnswerOptions.ForEach(b => b.Toggle.interactable = true);
    }

    public void DeleteCurrentQUestionFromBuffer(Question question)
    {
        questions.Remove(question);
    }

    public void CheckIfAnswerSubmitted()
    {
        var selectedUserOptions = uiAnswerOptions.Where(o => o.Toggle.isOn).ToList();
        if (selectedUserOptions.Count <= 0)
        {
            Debug.Log("no option selected");
            triggerWarning.gameObject.SetActive(true);

        }
        else
        {           
            CheckAnswers();
        }
    }
  
    public void CheckAnswers()
    {
        //Remove first question, we done with it
        questions.RemoveAt(0);
        currentQuestion = null;

        timeFromLastAnswerChecked = Time.timeSinceLevelLoadAsDouble;
        Invoke(nameof(LoadNextQuestion), delayToLoadNextQuestion);

        //disables submit button
        submitButton.enabled = false;

        StartCoroutine(FetchAnotherQuestion());
  
        triggerWarning.gameObject.SetActive(false);
        Debug.Log("Submitting");

        uiAnswerOptions.ForEach(b => b.Toggle.interactable = false);

        uiAnswerOptions.ForEach(o => o.CheckAnswer());
        foreach (var uiAnswerOption in uiAnswerOptions)
        {
            uiAnswerOption.deactivateOption();
        }
        var selectedUserOptions = uiAnswerOptions.Where(o => o.Toggle.isOn).ToList();

        toggleGroup.enabled = false;
        var correctAnswer = uiAnswerOptions.Where(o => o.loadedAnswer.isCorrect).ToList();

        if(selectedUserOptions[0] == correctAnswer[0])
        {
            ScoreManager.Instance.AddXP();
            References.Instance.numberOfCorrectAnsweredQuestions++;
        }
       
        uiAnswerOptions.ForEach(b => b.Toggle.interactable = false);
        helpMethod_FiftyFifty.interactable = false;
        helpMethod_GetCorrectAnswer.interactable = false;  
        helpMethod_GetTipFromRobot.interactable = false;    
        if(questionCount >= References.Instance.numberOfQuestionsPerGameByPreference)
        {
            Debug.Log("End game");
            Invoke(nameof(EndGame), 3f);       
        }
    }



    public void ClearQuestionList()
    {
        questions.Clear();
        Debug.Log($"Question List is cleared");
        
    }
    public void EndGame()
    {
        Debug.Log("End Game and sendind data");
        GlobalEvents.InvokeOnEndGame("");
    }

    public IEnumerator FetchAnotherQuestion()
    {
        yield return null;
        References.Instance.OpenAi_OriginalMessagePoster.PostMessage();
        References.Instance.OpenAi_CreateAssistantThreadRunner.PostMessage();
        References.Instance.OpenAi_ResultMessageFetcher.TryFetchingMessageContent();

        foreach (var uiAnswerOption in uiAnswerOptions)
        {
            uiAnswerOption.activateOption();
        }
    }

    public void FiftyFiftyHelpMethod()
    {
        var falseOptions = uiAnswerOptions.Where(x => !x.loadedAnswer.isCorrect).ToList();
        falseOptions.RemoveAt(Random.Range(0, falseOptions.Count));
        falseOptions.ForEach(x => x.gameObject.SetActive(false));
        helpMethod_FiftyFifty.gameObject.SetActive(false);
    }

    public void GetCorrectAnswer()
    {
        var falseOptions = uiAnswerOptions.Where(x => !x.loadedAnswer.isCorrect).ToList();
        falseOptions.ForEach(x => x.gameObject.SetActive(false));
        helpMethod_GetCorrectAnswer.gameObject.SetActive(false);
    }
    public void GetTipFromRobot()
    {
        helpMethod_TipBackground.gameObject.SetActive(true);
        tipFromRobot.gameObject.SetActive(true);
        tipFromRobot.text = currentQuestion.TipForAnsweringQuestion;
        helpMethod_GetTipFromRobot.gameObject.SetActive(false);
    }

    public void OpenPopUpForExitGame()
    { 
        exitPopUp.SetActive(true);
    }
    public void ClosePopUpForExitGame()
    {  
        exitPopUp.SetActive(false); 
    }

    public void LeaveGame()
    {
        ClosePopUpForExitGame();
        GlobalEvents.InvokeOnEndGame("");
    }

    public void CloseProblemScreen()
    {
        problemPanel.SetActive(false);
        loadingPanel.gameObject.SetActive(false);
        GlobalEvents.InvokeOnSignOut("signOut");
    }
}
