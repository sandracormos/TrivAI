using Best.HTTP;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Singleton.Example;

public class OpenAiCreateThread : MonoBehaviour
{
    [Header("References")]

    public OpenAi_ResultMessageFetcher GetOpenAiResponseSender;

    public OpenAi_ExecuteThreadRunner RunRunSender ;

    public OpenAi_ResultMessageFetcher Content;

    [Header("Properties")]

    [SerializeField]
    [Tooltip("URI link for this containers post request.")]
    string PostUriString = "https://api.openai.com/v1/threads";
    [SerializeField]
    string postResponseData = string.Empty;

    public OpenAiCreateThreadResponse openAiCreateThreadResponse;

    public void PostThread()
    {
        if (string.IsNullOrEmpty(PostUriString))
            return;

        HTTPRequest request = new HTTPRequest(new Uri(PostUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.Send();
        Debug.Log($"[OpenAiCreateThread] Request sent!");
    }


    private void OnPostRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        Debug.Log($"[OpenAiCreateThread] Response received !");
        postResponseData = resp.DataAsText;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    Debug.Log($"[CreateThread] Response received with data: {resp.DataAsText}");
                    openAiCreateThreadResponse = JsonConvert.DeserializeObject<OpenAiCreateThreadResponse>(resp.DataAsText);
                    References.Instance.OpenAi.ThreadId = openAiCreateThreadResponse.id;
                    if (!string.IsNullOrEmpty(References.Instance.OpenAi.ThreadId) && !string.IsNullOrEmpty(References.Instance.OpenAi.AssistantId)) {
                        PlayFabManager.Instance.SaveUsersOpenAiId();
                    }
                    
                
                    //Post initial message to thread
                    References.Instance.OpenAi_OriginalMessagePoster.PostMessage();

                    //Attach assistant on thread and create a runner
                    References.Instance.OpenAi_CreateAssistantThreadRunner.TryToLinkAssisstantToThread();

                    //Execute Runner
                    References.Instance.OpenAi_ExecuteThreadRunner.TryExecuteRunner();
                    References.Instance.OpenAi_ResultMessageFetcher.TryFetchingMessageContent();
                }
                else
                {
                    Debug.LogWarning($"[OpenAiCreateThread] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiCreateThread] Request Finished with Error! " + 
                               (req.Exception != null ? (req.Exception.Message + 
                               "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiCreateThread] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiCreateThread] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiCreateThread] Processing the request Timed Out!");
                break;
        }
    }



}

