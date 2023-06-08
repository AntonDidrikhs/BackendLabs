namespace WebAPI.Dto
{
    public class AnswerStatisticDto
    {
        public int QuizItemId { get; set; }
        public string Answer { get; set; }
        public bool isCorrect { get; set; }
    }
}
