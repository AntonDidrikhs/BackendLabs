using ApplicationCore.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MongoDB.Entities
{
    public class AnswerMongoEntity : BaseMongoEntity
    {
        [BsonElement("id")]
        public int UserId { get; set; }
        [BsonElement("quizItemId")]
        public int QuizItemId { get; set; }
        [BsonElement("quizId")]
        public int QuizId { get; set; }
        [BsonElement("answer")]
        public string UserAnswer { get; set; }
        [BsonElement("item")]
        public QuizItemMongoEntity QuizItem { get; set; }
    }

}
