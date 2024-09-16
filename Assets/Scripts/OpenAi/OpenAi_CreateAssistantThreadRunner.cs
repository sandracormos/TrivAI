using Best.HTTP.Request.Upload;
using Best.HTTP;
using Newtonsoft.Json;
using System;
using UnityEngine;
using Singleton.Example;
using System.Threading;

public class OpenAi_CreateAssistantThreadRunner : MonoBehaviour
{
    [Header("References")]
    public OpenAiRunThreadResponse response;

    [Header("Properties")]

    [SerializeField]
    OpenAiPostRun postData;

    public string unformattedPostUriString = "https://api.openai.com/v1/threads/{0}/runs";
    public string formattedPostUriString = string.Empty;
    [SerializeField]
    string bodyJsonData = string.Empty;
    [SerializeField]
    string postResponseData = string.Empty;

    private void Awake()
    {
        References.Instance.OpenAi_CreateAssistantThreadRunner = this;
    }
 
    public void TryToLinkAssisstantToThread()
    {
        if (string.IsNullOrEmpty(References.Instance.OpenAi.ThreadId))
            return;
        if (string.IsNullOrEmpty(References.Instance.OpenAi.AssistantId))
            return;

        PostMessage();
    }



    public void PostMessage()
    {
        //Format UriString
        formattedPostUriString = string.Format(unformattedPostUriString, References.Instance.OpenAi.ThreadId);
        postData.assistant_id = References.Instance.OpenAi.AssistantId;

        HTTPRequest request = new HTTPRequest(new Uri(formattedPostUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.UploadSettings.UploadStream = new JSonDataStream<OpenAiPostRun>(postData);
        request.Send();
        Debug.Log($"[OpenAiRunThread] Request sent!");
    }

    private void OnPostRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        postResponseData = resp.DataAsText;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    Debug.Log($"[OpenAiRunThread] Request response received!");
                    response = JsonConvert.DeserializeObject<OpenAiRunThreadResponse>(resp.DataAsText);
                    References.Instance.OpenAi.ThreadRunnerId = response.Id;
                    References.Instance.OpenAi_ExecuteThreadRunner.TryExecuteRunner();
                }
                else
                {
                    Debug.LogWarning($"[OpenAiRunThread] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiRunThread] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiRunThread] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiRunThread] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiRunThread] Processing the request Timed Out!");
                break;
        }
    }

}

