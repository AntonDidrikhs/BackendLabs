using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.EF.Entities;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EF.Services
{
    public class QuizUserServiceEF : IQuizUserService
    {
        private QuizDbContext _context;
        public QuizUserServiceEF(QuizDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Quiz> FindAllQuizzes()
        {
            return _context
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Items)
                .ThenInclude(i => i.IncorrectAnswers)
                .Select(QuizMappers.FromEntityToQuiz)
                .ToList();
        }

        public Quiz? FindQuizById(int id)
        {
            var entity = _context
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Items)
                .ThenInclude(i => i.IncorrectAnswers)
                .FirstOrDefault(e => e.Id == id);
            return entity is null ? null : QuizMappers.FromEntityToQuiz(entity);
        }

        public Quiz CreateAndGetQuizRandom(int count)
        {
            throw new NotImplementedException();
        }

        public List<QuizItemUserAnswer> GetUserAnswersForQuiz(int quizId, int userId)
        {
            List<QuizItemUserAnswerEntity> AnswersEntityList = new List<QuizItemUserAnswerEntity>();
            AnswersEntityList = _context
                .UserAnswers
                .AsNoTracking()
                .Where(e => e.UserId == userId || e.QuizId == quizId).ToList();
            return AnswersEntityList.Select(QuizMappers.FromEntityToUserAnswer).ToList();
        }

        public QuizItemUserAnswer SaveUserAnswerForQuiz(int quizId, int quizItemId, int userId, string answer)
        {
            QuizItemUserAnswerEntity entity = new QuizItemUserAnswerEntity()
            {
                UserId = userId,
                QuizItemId = quizItemId,
                QuizId = quizId,
                QuizItem = _context.QuizItems.First(i => i.Id == quizItemId),
                UserAnswer = answer
            };
            try
            {
                var saved = _context.UserAnswers.Add(entity).Entity;
                _context.SaveChanges();
                return new QuizItemUserAnswer()
                {
                    UserId = saved.UserId,
                    QuizItem = QuizMappers.FromEntityToQuizItem(saved.QuizItem),
                    QuizId = saved.QuizId,
                    Answer = saved.UserAnswer
                };
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.Message.StartsWith("The INSERT"))
                {
                    throw new QuizNotFoundException("Quiz, quiz item or user not found. Can't save!");
                }
                if (e.InnerException.Message.StartsWith("Violation of"))
                {
                    throw new QuizAnswerItemAlreadyExistsException(quizId, quizItemId, userId);
                }
                throw new Exception(e.Message);
            }
        }
    }
}
