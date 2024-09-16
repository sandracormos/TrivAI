using Best.HTTP;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using Singleton.Example;

public class OpenAi_ResultMessageFetcher : MonoBehaviour
{
    [Header("References")]

    public Question deserializedQuestion;

    public OpenAiResponse openAiResponse;

    public QuestionManager manager;



    [Header("Properties")]

    [SerializeField]
    string unformattedPostUriString = "https://api.openai.com/v1/threads/{0}/messages/{1}";
    string formattedPostUriString = string.Empty;
    [SerializeField]
    string bodyJsonData = string.Empty;
    [SerializeField]
    string postResponseData = string.Empty;


    public string threadId = string.Empty;
    public string messageId = string.Empty;

    private void Awake()
    {
        References.Instance.OpenAi_ResultMessageFetcher = this;
    }

    public void TryFetchingMessageContent()
    {
        if (string.IsNullOrEmpty(References.Instance.OpenAi.ThreadId))
            return;
        if (string.IsNullOrEmpty(References.Instance.OpenAi.ResultMessageId))
            return;

        formattedPostUriString = string.Format(unformattedPostUriString, 
                                               References.Instance.OpenAi.ThreadId,
                                               References.Instance.OpenAi.ResultMessageId);
        StartCoroutine(FetchMessageContent());
    }

    IEnumerator FetchMessageContent()
    {
        if (string.IsNullOrEmpty(formattedPostUriString))
            yield break;

        yield return new WaitForSeconds(1);

        HTTPRequest request = new HTTPRequest(new Uri(formattedPostUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.Send();
        
        Debug.Log($"[OpenAi_ResultMessageFetcher] Request sent!");
    }

    void OnPostRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        postResponseData = resp.DataAsText;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    openAiResponse = JsonConvert.DeserializeObject<OpenAiResponse>(resp.DataAsText);
                    if (openAiResponse is null || openAiResponse.content is null || openAiResponse.content.Count == 0 ||
                        openAiResponse.content[0].text is null || string.IsNullOrEmpty(openAiResponse.content[0].text.value))
                    {
                        StartCoroutine(FetchMessageContent());
                        return;
                    }

                    bodyJsonData = openAiResponse.content[0].text.value;
                    bodyJsonData = bodyJsonData.Replace("json", "");
                    bodyJsonData = bodyJsonData.Replace("`", "");

                    References.Instance.OpenAi.ResultMessageId = string.Empty;
                 
                    deserializedQuestion = JsonConvert.DeserializeObject<Question>(bodyJsonData);
                    GlobalEvents.InvokeOnQuestionFetchSuccess(deserializedQuestion);                    
                }
                else
                {
                    Debug.LogWarning($"[OpenAiRequestMessageContent] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiRequestMessageContent] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiRequestMessageContent] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiRequestMessageContent] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiRequestMessageContent] Processing the request Timed Out!");
                break;
        }
    }

}


