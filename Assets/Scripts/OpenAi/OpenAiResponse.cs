using System.Collections.Generic;

[System.Serializable]
public class OpenAiResponse
{
    public string Id;
    public string Object;
    public long created_at;
    public List<Choice> content;
}
[System.Serializable]
public class Choice
{
    public string type;
    public ContentText text;
}
[System.Serializable]
public class ContentText
{
    public string value;
}

