using Best.HTTP;
using Best.HTTP.Request.Upload;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Singleton.Example;

public class OpenAiCreateAssistant : MonoBehaviour
{
    [Header("Properties")]

    [SerializeField]
    OpenAiPostAssistant postData = new();

    [SerializeField]
    [Tooltip("URI link for this containers post request.")]
    string PostUriString = "https://api.openai.com/v1/assistants";
    [SerializeField]
    string bodyJsonData = string.Empty;
    [SerializeField]
    string postResponseData = string.Empty;

    public OpenAiAssistantResponse openAiAssistantResponse;

    public void PostAssistant()
    {
        if (string.IsNullOrEmpty(PostUriString))
            return;

        HTTPRequest request = new HTTPRequest(new Uri(PostUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.UploadSettings.UploadStream = new JSonDataStream<OpenAiPostAssistant>(postData);
        request.Send();
        Debug.Log($"[OpenAiCreateAssistant] Request sent!");
    }


    private void OnPostRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        Debug.Log($"[OpenAiCreateAssistant] Response received..");
        postResponseData = resp.DataAsText;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    Debug.Log($"[OpenAiCreateAssistant] Request response received! with : {resp.DataAsText}");
                    openAiAssistantResponse = JsonConvert.DeserializeObject<OpenAiAssistantResponse>(resp.DataAsText);
                    //runThreadSender.InjectAssistantId(openAiAssistantResponse.Id);
                    References.Instance.OpenAi.AssistantId = openAiAssistantResponse.Id;
                    if (!string.IsNullOrEmpty(References.Instance.OpenAi.ThreadId) && !string.IsNullOrEmpty(References.Instance.OpenAi.AssistantId))
                    {
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
                    Debug.LogWarning($"[OpenAiCreateAssistant] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiCreateAssistant] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiCreateAssistant] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiCreateAssistant] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiCreateAssistant] Processing the request Timed Out!");
                break;
        }
    }



}

