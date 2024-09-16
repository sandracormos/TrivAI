
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class OpenAiPostDataQuestion
{
    public string model = "gpt-3.5-turbo";
    public int max_tokens = 250;    
    public List<OpenAiMessage> messages = new();
}

[System.Serializable]
public class OpenAiMessage
{
    public string role = "user";
    public string content = string.Empty;
}

[System.Serializable]
public class OpenAiPostMessageThread
{
    public string role = "user";
    [TextArea(5,10)]
    public string content = string.Empty;
}

[System.Serializable]
public class OpenAiPostAssistant
{
    public string model = "gpt-3.5-turbo";
    public string name = "Robo";
}

[System.Serializable]
public class OpenAiPostRun
{
    public string assistant_id = string.Empty;
    public string model = "gpt-3.5-turbo";
}

