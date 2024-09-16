using Best.HTTP;
using Best.HTTP.Request.Upload;
using System;
using UnityEngine;

public class OpenAiCommunicator : MonoBehaviour
{
    [SerializeField]
    string apiKey = "Bearer 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";

    [SerializeField]
    OpenAiPostDataQuestion postData;

    [SerializeField]
    [Tooltip("URI link for this containers post request.")]
    string PostUriString = string.Empty;
    [SerializeField]
    string bodyJsonData = string.Empty;
    [SerializeField]
    string postResponseData = string.Empty;

    public Question deserializedQuestion;


    public OpenAiResponse openAiResponse;


    public QuestionManager manager;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PostData();
        }
    }

    public void PostData()
    {
        if (string.IsNullOrEmpty(PostUriString))
            return;

        HTTPRequest request = new HTTPRequest(new Uri(PostUriString), HTTPMethods.Post, OnPostRequestFinished);
        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", apiKey);
        request.UploadSettings.UploadStream = new JSonDataStream<OpenAiPostDataQuestion>(postData);
        request.Send();
        Debug.Log($"[OpenAiCommunicator] Request sent!");
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
                   //openAiResponse = JsonConvert.DeserializeObject<OpenAiResponse>(resp.DataAsText);
                    //deserializedQuestion = JsonConvert.DeserializeObject<Question>(openAiResponse.Choices[0].Message.Content);   
                    //manager.LoadQuestion(deserializedQuestion);
                    

                }
                else
                {
                    Debug.LogWarning($"[OpenAiCommunicator] Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError($"[OpenAiCommunicator] Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning($"[OpenAiCommunicator] Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError($"[OpenAiCommunicator] Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError($"[OpenAiCommunicator] Processing the request Timed Out!");
                break;
        }
    }

}
