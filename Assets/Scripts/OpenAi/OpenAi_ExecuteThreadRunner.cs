using Best.HTTP;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using Singleton.Example;

public class OpenAi_ExecuteThreadRunner : MonoBehaviour
{
    [Header("Properties")]
    public OpenAiRunRunResponse response;
    [SerializeField]
    

    public string unformattedPostUriString = "https://api.openai.com/v1/threads/{0}/runs/{1}/steps";
    public string formattedPostUriString = string.Empty;

    [SerializeField]
    string bodyJsonData = string.Empty;
    [SerializeField]
    string getResponseData = string.Empty;


    private void Awake()
    {
        References.Instance.OpenAi_ExecuteThreadRunner = this;
    }

    public void TryExecuteRunner()
    {
        if (string.IsNullOrEmpty(References.Instance.OpenAi.ThreadId))
            return;
        if (string.IsNullOrEmpty(References.Instance.OpenAi.ThreadRunnerId))
            return;
        formattedPostUriString = string.Format(unformattedPostUriString,
                                               References.Instance.OpenAi.ThreadId,
                                               References.Instance.OpenAi.ThreadRunnerId);

        StartCoroutine(ExecuteRunner());
    }
 
    IEnumerator ExecuteRunner()
    {
        if (string.IsNullOrEmpty(formattedPostUriString))
            yield break;

        yield return new WaitForSeconds(1);

        HTTPRequest request = new HTTPRequest(new Uri(formattedPostUriString), HTTPMethods.Get, OnGetRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", References.Instance.OpenAi.ApiKey);
        request.SetHeader("OpenAI-Beta", "assistants=v2");
        request.Send();
        Debug.Log($"[OpenAiRunRun] Request sent!");
    }

    void OnGetRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        getResponseData = resp.DataAsText;
        switch (req.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {

                    response = JsonConvert.DeserializeObject<OpenAiRunRunResponse>(resp.DataAsText);
                    if (response is null || response.data is null || response.data.Count == 0)
                    {
                        StartCoroutine(ExecuteRunner());
                        return;
                    }
                    References.Instance.OpenAi.ThreadRunnerId = string.Empty;
                    References.Instance.OpenAi.ResultMessageId = response.data[0].step_details.message_creation.message_id;
                    References.Instance.OpenAi_ResultMessageFetcher.TryFetchingMessageContent();

                }
                else
                {
                    Debug.LogWarning($"[OpenAiRunRun] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiRunRun] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiRunRun] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiRunRun] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiRunRun] Processing the request Timed Out!");
                break;
        }
    }

}

