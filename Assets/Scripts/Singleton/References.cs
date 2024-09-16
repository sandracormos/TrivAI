using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace Singleton.Example
{
    public class References : SingletonBehaviour<References>
    {
        /// <summary>
        /// Contains all properties required to obtain questions via OpenAi.
        /// </summary>
        public OpenAiPrroperties OpenAi = new();

        public OpenAiCreateThread OpenAiCreateThread;

        public OpenAiCreateAssistant OpenAiCreateAssistant;

        /// <summary>
        /// Posts the original request message on thread.
        /// </summary>
        public OpenAi_OriginalMessagePoster OpenAi_OriginalMessagePoster;
        
        /// <summary>
        /// Creates a runner by attaching the assistant to the thread.
        /// Returns with a runner ID that must be executed by ThreadRunner
        /// </summary>
        public OpenAi_CreateAssistantThreadRunner OpenAi_CreateAssistantThreadRunner;

        /// <summary>
        /// Executes the last receives RunnerID on the thread
        /// </summary>
        public OpenAi_ExecuteThreadRunner OpenAi_ExecuteThreadRunner;

        /// <summary>
        /// Retrieves the generated message content after executing the runner on the thread.
        /// Result message contains question with multiple answers.
        /// </summary>
        public OpenAi_ResultMessageFetcher OpenAi_ResultMessageFetcher;

        [TextArea(2, 10)]
        public string categories = string.Empty;

        [TextArea(2, 10)]
        public string difficultyLevel = string.Empty;


        public int numberOfQuestionsPerGameByPreference = -1;

        public int numberOfCorrectAnsweredQuestions = 0;

        public int numberOfAnsweredQuestions = 0;

        public string UserId =string.Empty;
        public int rank = -1;
        public int Rank
        {
            get { return rank; }
            set
            {
                rank = value +1;
                Debug.Log($"[References] Rank set to: {rank}");
            }

        }


        string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;

                Debug.Log($"[References] Username set to: {username}");
                PlayFabManager.Instance.ChangeDisplayName();
            }
        }
       
    }



    [System.Serializable]
    public class OpenAiPrroperties
    {
        public string ApiKey = "Your OpenAI GPT API Key";

        public string ThreadId;
        public string AssistantId;
        public string ThreadRunnerId = string.Empty;
        public string ResultMessageId = string.Empty;
    }
}
