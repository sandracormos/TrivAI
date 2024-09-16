using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OpenAiRunRunResponse 
{
    public List<SomeData> data;
}

[System.Serializable]
public class SomeData
{
    public StepDetails step_details;
}

[System.Serializable]
public class StepDetails
{
    public MessageCreationData message_creation;
}

[System.Serializable]
public class MessageCreationData
{
    public string message_id;
}