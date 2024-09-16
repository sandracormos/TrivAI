using Best.HTTP.Request.Upload;
using Best.HTTP;
using Newtonsoft.Json;
using System;
using UnityEngine;
using Singleton.Example;

public class OpenAi_OriginalMessagePoster : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField]
    [TextArea(2,15)]
    string unformattedPostContent = string.Empty;

    [SerializeField]
    [TextArea(2,15)]
    string jsonFormatAndRest = string.Empty;


    [SerializeField]
    public OpenAiPostMessageThread postData;

    public string unformattedUriString = "https://api.openai.com/v1/threads/{0}/messages";
    public string formattedUriString = string.Empty;


    public OpenAiMessageToThreadResponse openAiMessageToThreadResponse;

    private void Awake()
    {
        References.Instance.OpenAi_OriginalMessagePoster = this;
    }

    public void PostMessage()
    {
        formattedUriString = string.Format(unformattedUriString, References.Instance.OpenAi.ThreadId);
        postData.content = string.Format(unformattedPostContent, string.IsNullOrEmpty(References.Instance.categories) ? "anything" : References.Instance.categories,
                                                                 string.IsNullOrEmpty(References.Instance.difficultyLevel) ? "diverse" : References.Instance.difficultyLevel)
                                                                + jsonFormatAndRest;
        HTTPRequest request = new HTTPRequest(new Uri(formattedUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.UploadSettings.UploadStream = new JSonDataStream<OpenAiPostMessageThread>(postData);
        request.Send();
        Debug.Log($"[OpenAiPostMessageToThread] Request sent!");
    }

    private void OnPostRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    openAiMessageToThreadResponse = JsonConvert.DeserializeObject<OpenAiMessageToThreadResponse>(resp.DataAsText);
                }
                else
                {
                    Debug.LogWarning($"[OpenAiPostMessageToThread] Request finished Successfully, but the server sent an error. Status Code: " +
                        $"{resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiPostMessageToThread] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message +
                                                                            "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiPostMessageToThread] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiPostMessageToThread] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiPostMessageToThread] Processing the request Timed Out!");
                break;
        }
    }

}

