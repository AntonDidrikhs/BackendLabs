using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.MongoDB;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace WebAPI.Controllers;
[ApiController]
[Route("/api/v2/quizzes")]
public class QuizControllerV2 : ControllerBase
{
    private readonly QuizUserServiceMongoDB _service;
    public QuizControllerV2(QuizUserServiceMongoDB service)
    {
        _service = service;
    }
    [HttpGet]
    public IEnumerable<QuizDto> FindAll()
    {
        return _service.FindAllQuizzes().Select(QuizDto.of).AsEnumerable();
    }
    [HttpGet]
    [Route("{id}")]
    public ActionResult<QuizDto> FindById(int id)
    {
        var result = QuizDto.of(_service.FindQuizById(id));
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [Route("{quizId}/items/{itemId}/answers")]
    public ActionResult SaveAnswer([FromBody] QuizItemAnswerDto dto, int quizId, int itemId)
    {
        try
        {
            var answer = _service.SaveUserAnswerForQuiz(quizId, itemId, dto.UserId, dto.UserAnswer);
            return Created("", answer);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet, Produces("application/json")]
    [Route("{userId}/{quizId}/feedbacks")]
    public FeedbackQuizDto GetFeedback(int quizId, int userId)
    {
        var answers = _service.GetUserAnswersForQuiz(quizId, userId);
        return new FeedbackQuizDto()
        {
            QuizId = quizId,
            UserId = 1,
            QuizItemsAnswers = answers.Select(i => new FeedbackQuizItemDto()
            {
                Question = i.QuizItem.Question,
                Answer = i.Answer,
                IsCorrect = i.IsCorrect(),
                QuizItemId = i.QuizItem.Id
            }).ToList()
        };
    }

}
