using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Question
{
	public int Id;
	public string QuestionName;
	public List<Answer> Answers;
    public string TipForAnsweringQuestion;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Question other = (Question)obj;
        return QuestionName == other.QuestionName;
    }

    public override int GetHashCode()
    {
        return QuestionName?.GetHashCode() ?? 0;
    }
}

[System.Serializable]
public class Answer
{
	public string text;
	public bool isCorrect;
}



