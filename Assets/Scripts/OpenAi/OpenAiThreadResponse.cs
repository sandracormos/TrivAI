using System.Collections.Generic;

[System.Serializable]
public class OpenAiThreadResponse
{
    public string Id;
    public string Object;
    public string Thread_Id;
}

[System.Serializable]
public class OpenAiCreateThreadResponse
{
    public string id;
}