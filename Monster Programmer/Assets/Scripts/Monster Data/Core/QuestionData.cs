using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public Sprite questionText;
    public Sprite[] answerOptions = new Sprite[4]; // 4 gambar opsi jawaban
    public int correctAnswerIndex; // 0-3, index jawaban yang benar
}
