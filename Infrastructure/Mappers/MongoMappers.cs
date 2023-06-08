using ApplicationCore.Models;
using Infrastructure.MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mappers
{
    public class MongoMappers
    {
        public static QuizItem FromMongoToQuiz(QuizItemMongoEntity quizItem)
        {
            return new QuizItem(
                quizItem.ItemId,
                quizItem.Question,
                quizItem.IncorrectAnswers,
                quizItem.CorrectAnswer
                );
        }
        /*public static IQueryable<QuizItem> FromMongoToQuer(this IQueryable<QuizItemMongoEntity> quizItems)
        {
            return quizItems.Select(quizItem => new QuizItem
            {
                quizItem.ItemId,
                quizItem.Question,
                quizItem.IncorrectAnswers,
                quizItem.CorrectAnswer
            });
        

        } */
    }
}
