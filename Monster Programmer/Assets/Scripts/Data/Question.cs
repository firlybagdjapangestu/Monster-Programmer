using UnityEngine;

namespace Data {

    [CreateAssetMenu(fileName = "New Question", menuName = "Question/NewQuestion")]
    public class Question : ScriptableObject
    {
        [SerializeField, TextArea] private string questionText;
        [SerializeField] private string[] answers = new string[4];
        [SerializeField] private int indexRightAnswer;
        [SerializeField] private MonsterType specializeType;

        public string QuestionText => questionText;
        public string[] Answers => answers;
        public int IndexRightAnswer => indexRightAnswer;
        public MonsterType SpecializeType => specializeType;

        public bool CheckAnswer(int index) => index == indexRightAnswer;
        public bool CheckAnswer(string _answer) => answers[indexRightAnswer] == _answer;
    }

}


