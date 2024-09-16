using Singleton.Example;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Properties")]
    public TMP_Dropdown category;
    public TMP_Dropdown difficulty;
    public TMP_Dropdown gameDuration;

    [SerializeField]
    public GameObject PopUpCategoryChoices;

    [SerializeField]
    public List<string> PopUpCategories =new();

    [SerializeField]
    public TMP_InputField categories;
  
    public TMP_Dropdown ChooseCategory;

    [SerializeField]
    public TMP_InputField username;

    [SerializeField]
    [TextArea(2, 5)]
    public string randomCategories;
    [Header("References")]
    [SerializeField]
    public GameObject info_FiftyFiftyMethod;
    [SerializeField]
    public GameObject info_AskRobotMethod;
    [SerializeField]
    public GameObject info_GetTipMethod;

    [SerializeField]
    [TextArea(2, 5)]
    public string noUsernameNames;
    [SerializeField]
    public GameObject signOutPopUp;
    [SerializeField]
    public GameObject authenticationPage;
    public int defaultDropDownCategoryValue;



    private void Start()
    {
        defaultDropDownCategoryValue = ChooseCategory.value;
    }

    private void Awake()
    {
        GlobalEvents.OnUserRegistered += GlobalEvents_OnUserRegistered;
        GlobalEvents.OnUserLoggedIn += GlobalEvents_OnUserLoggedIn;
        GlobalEvents.OnBackToSetupMenu += GlobalEvents_OnBackToSetupMenu;
        GlobalEvents.OnStartGame += GlobalEvents_OnStartGame;

        info_FiftyFiftyMethod.SetActive(false);
        info_AskRobotMethod.SetActive(false);
        info_GetTipMethod.SetActive(false);

    }   

    private void OnDestroy()
    {
        GlobalEvents.OnUserRegistered -= GlobalEvents_OnUserRegistered;
        GlobalEvents.OnUserLoggedIn -= GlobalEvents_OnUserLoggedIn;
        GlobalEvents.OnStartGame -= GlobalEvents_OnStartGame;

        GlobalEvents.OnBackToSetupMenu -= GlobalEvents_OnBackToSetupMenu;

    }
    private void GlobalEvents_OnBackToSetupMenu(string username)
    {
        gameObject.SetActive(true);
        ChooseCategory.value = defaultDropDownCategoryValue;

    }
    private void GlobalEvents_OnStartGame(string username)
    {
        gameObject.SetActive(false);
    }

    private void GlobalEvents_OnUserRegistered(string username)
    {
        gameObject.SetActive(true);        
    }
    private void GlobalEvents_OnUserLoggedIn(string username)
    {
        gameObject.SetActive(true);
    }

    public void ClosePopUp()
    {
        PopUpCategoryChoices.SetActive(false);
    }

    public void OpenPopUp()
    {
        PopUpCategoryChoices.SetActive(true);        
    }

    public void CheckTypeOfCategoriesPrefered()
    {
        if (ChooseCategory.options[ChooseCategory.value].text.Equals("Custom"))
        {
            OpenPopUp();
        }
      
    }

    public void CloseCategoriesPopUp()
    {
        PopUpCategoryChoices.SetActive(false);
    }

    public void SelectRandomCategories()
    {
        if (ChooseCategory.options[ChooseCategory.value].text.Equals("Combined"))
        {
          
            List<string> randomCat = randomCategories.Split(',').ToList();
            int numberOfRandomCategories = 0;
            string result = string.Empty;
            while (numberOfRandomCategories < 4)
            {
                numberOfRandomCategories++;
                int number = UnityEngine.Random.Range(0, randomCat.Count);
                result += randomCat[number] + ",";
                randomCat.Remove(randomCat[number]);

            }
            Debug.Log($"random choosed categories: {result}");
            References.Instance.categories = result;
        }
           

    }

    public void OnCategoriesConfirmedButtonPress()
    {
        CloseCategoriesPopUp();
        FetchCustomCategories();
    }

    public void FetchCustomCategories()
    {
        References.Instance.categories = categories.text;
      //  GlobalEvents.InvokeOnCategoriesUpdated(References.Instance.categories);
    }

    public void FetchDifficultyLevelSelected()
    {
        References.Instance.difficultyLevel = difficulty.options[difficulty.value].text;

    }

    public void StartGame()
    {
        FetchCustomCategories() ; 
        FetchDifficultyLevelSelected();
        FetchDurationOfGameSelected();
        FetchUsername();
        SelectRandomCategories();
        GlobalEvents.InvokeOnStartGame(References.Instance.categories);
    }

    private void FetchDurationOfGameSelected()
    {
        if (gameDuration.options[gameDuration.value].text.Equals("Short Version"))
        {
            References.Instance.numberOfQuestionsPerGameByPreference = 10;
        }
        else if (gameDuration.options[gameDuration.value].text.Equals("Medium Version"))
        {
            References.Instance.numberOfQuestionsPerGameByPreference = 20;
        }
        else if (gameDuration.options[gameDuration.value].text.Equals("Long Version"))
        {
            References.Instance.numberOfQuestionsPerGameByPreference = 30;
        }
    }

    public void FetchUsername()
    {
        Debug.Log("[GameSetup] Fetching username...");
        if (string.IsNullOrEmpty(References.Instance.Username))
        {
            var possibleUsernames = noUsernameNames.Split(',');
            if (string.IsNullOrEmpty(username.text))
            {
                Debug.Log("[GameSetup] Giving Random username...");
                int number = UnityEngine.Random.Range(0, possibleUsernames.Length);
                username.text = possibleUsernames[number];
            }
        }
        Debug.Log("[GameSetup] Got username...");
        References.Instance.Username = username.text;

    }

    public void ShowFiftyFiftyHelpMethodInfo()
    {
        info_AskRobotMethod.SetActive(false);

        info_GetTipMethod.SetActive(false);
        info_FiftyFiftyMethod.SetActive(true);
    }
    public void ShowAskRobotHelpMethodInfo()
    {
        info_FiftyFiftyMethod.SetActive(false);

        info_GetTipMethod.SetActive(false);
        info_AskRobotMethod.SetActive(true);
    }

    public void ShowGetTipHelpMethodInfo()
    {
        info_FiftyFiftyMethod.SetActive(false);
        info_AskRobotMethod.SetActive(false);
        info_GetTipMethod.SetActive(true);
    }

    public void OpenSignOutPopUp()
    {
        signOutPopUp.SetActive(true);
    }
    public void CloseSignOutPopUp()
    {
        signOutPopUp.SetActive(false);
    }
    public void SignOut()
    {
        CloseSignOutPopUp();
        authenticationPage.SetActive(true);
    }
}
