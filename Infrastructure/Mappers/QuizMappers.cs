using ApplicationCore.Models;
using Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mappers
{
    internal class QuizMappers
    {
        public static QuizItem FromEntityToQuizItem(QuizItemEntity entity)
        {
            return new QuizItem(
                entity.Id,
                entity.Question,
                entity.IncorrectAnswers.Select(e => e.Answer).ToList(),
                entity.CorrectAnswer);
        }

        public static Quiz FromEntityToQuiz(QuizEntity entity)
        {
            List<QuizItem> items = new List<QuizItem>();
            entity.Items.ToList().ForEach(item => items.Add(FromEntityToQuizItem(item)));
            return new Quiz(
                entity.Id,
                items,
                entity.Title
                );
        }
        public static QuizItemUserAnswer FromEntityToUserAnswer(QuizItemUserAnswerEntity entity)
        {
            return new QuizItemUserAnswer(
                FromEntityToQuizItem(entity.QuizItem),
                entity.UserId,
                entity.QuizId,
                entity.UserAnswer
                );
        }
    }
}
