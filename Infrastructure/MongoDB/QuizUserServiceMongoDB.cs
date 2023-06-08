using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.MongoDB.Entities;
using Infrastructure.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.EF.Entities;
using System.Runtime.CompilerServices;

namespace Infrastructure.MongoDB
{
    public class QuizUserServiceMongoDB : IQuizUserService
    {
        private readonly IMongoCollection<QuizMongoEntity> _quizzes;
        private readonly IMongoCollection<AnswerMongoEntity> _answers;
        private readonly MongoClient _client;

        public QuizUserServiceMongoDB(IOptions<MongoDBSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionUri);
            IMongoDatabase database = _client.GetDatabase(settings.Value.DatabaseName);
            _quizzes = database.GetCollection<QuizMongoEntity>(settings.Value.QuizCollection);
            _answers = database.GetCollection<AnswerMongoEntity>(settings.Value.AnswerCollection);
        }

        public IEnumerable<Quiz> FindAllQuizzes()
        {
            var quizMongoEntities = _quizzes.Find(Builders<QuizMongoEntity>.Filter.Empty).ToList();
            return _quizzes
                .Find(Builders<QuizMongoEntity>.Filter.Empty)
                .Project(
                    q =>
                        new Quiz(
                            q.QuizId,
                            q.Items.Select(i => new QuizItem(
                                    i.ItemId,
                                    i.Question,
                                    i.IncorrectAnswers,
                                    i.CorrectAnswer
                                )
                            ).ToList(),
                            q.Title
                        )
                ).ToEnumerable();
        }

        public Quiz? FindQuizById(int id)
        {
            return _quizzes
                .Find(Builders<QuizMongoEntity>.Filter.Eq(q => q.QuizId, id))
                .Project(q =>
                    new Quiz(
                        q.QuizId,
                        q.Items.Select(i => new QuizItem(
                                i.ItemId,
                                i.Question,
                                i.IncorrectAnswers,
                                i.CorrectAnswer
                            )
                        ).ToList(),
                        q.Title
                    )
                ).FirstOrDefault();
        }

        public QuizItemUserAnswer SaveUserAnswerForQuiz(int quizId, int quizItemId, int userId, string answer)
        {
            PipelineDefinition<QuizMongoEntity, QuizItemMongoEntity> pipeline = new[]
{
    new BsonDocument("$unwind",
    new BsonDocument
        {
            { "path", "$items" },
            { "includeArrayIndex", "index" },
            { "preserveNullAndEmptyArrays", true }
        }),
    new BsonDocument("$match",
    new BsonDocument("id", quizId)),
    new BsonDocument("$match",
    new BsonDocument("items.id", quizItemId)),
    new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", 0 },
            { "index", 0 },
            { "title", 0 },
            { "id", 0 }
        }),
    new BsonDocument("$replaceWith", "$items")
};
            AnswerMongoEntity entity = new AnswerMongoEntity();
            {
                entity.QuizId = quizId;
                entity.QuizItemId = quizItemId;
                entity.UserId = userId;
                entity.UserAnswer = answer;
                entity.QuizItem = _quizzes.Aggregate(pipeline).FirstOrDefault();
            }
            _answers.InsertOne( entity );

            return new QuizItemUserAnswer()
            {
                UserId = userId,
                QuizId = quizId,
                Answer = answer,
                QuizItem = MongoMappers.FromMongoToQuiz(entity.QuizItem)
            };
        }

        public List<QuizItemUserAnswer> GetUserAnswersForQuiz(int quizId, int userId)
        {
            var filterUserId = Builders<AnswerMongoEntity>.Filter.Eq(a => a.UserId, userId);
            var filterQuizId = Builders<AnswerMongoEntity>.Filter.Eq(a => a.QuizId, quizId);
            //var project = Builders<AnswerMongoEntity>.Projection.Expression(i => MongoMappers.FromMongoToQuiz(i.QuizItem));
            List<AnswerMongoEntity> Answers = new List<AnswerMongoEntity>();
            Answers = _answers.Find(Builders<AnswerMongoEntity>.Filter.And(filterQuizId, filterUserId))
                /*.Project(
                e => new QuizItemUserAnswer(
                    //MongoMappers.FromMongoToQuiz(e.QuizItem),
                    e.QuizItem.,
                    e.UserId,
                    e.QuizId,
                    e.UserAnswer
                    )
                )*/
                .ToList();
            return Answers.Select(a =>
            new QuizItemUserAnswer(
                MongoMappers.FromMongoToQuiz(a.QuizItem),
                a.UserId,
                a.QuizId,
                a.UserAnswer
                )
            ).ToList();
        }

        public Quiz CreateAndGetQuizRandom(int count)
        {
            throw new NotImplementedException();
        }
    }
}
