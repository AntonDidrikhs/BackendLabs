using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dto;
using WebAPI.Services;

namespace WebAPI.Controllers;
[ApiController]
[Route("/api/v1/quizzes")]
public class QuizController: ControllerBase
{
    private readonly IQuizUserService _service;
    private readonly IMessageProducer _producer;

    public QuizController(IMessageProducer messageProducer, IQuizUserService service)
    {
        _producer = messageProducer;
        _service = service;
    }
    /*public QuizController(IQuizUserService service)
    {
        _service = service;
    }*/
    [HttpGet]
    [Route("{id}")]
    public ActionResult<QuizDto> FindById(int id)
    {
        var result = QuizDto.of(_service.FindQuizById(id));
        return result is null ?  NotFound() : Ok(result);
    }

    [HttpGet]
    public IEnumerable<QuizDto> FindAll()
    {
        return _service.FindAllQuizzes().Select(QuizDto.of).AsEnumerable();
    }

    [HttpPost]
    //[Authorize(Policy = "Bearer")]
    [Route("{quizId}/items/{itemId}/answers")]
    public ActionResult SaveAnswer([FromBody] QuizItemAnswerDto dto, LinkGenerator linker, int quizId, int itemId)
    {
        try
        {
            var answer = _service.SaveUserAnswerForQuiz(quizId, itemId, dto.UserId, dto.UserAnswer);
            _producer.SendMessage(new AnswerStatisticDto()
            {
                Answer = dto.UserAnswer,
                QuizItemId = itemId,
                isCorrect = _service.FindQuizById(quizId).Items.Find(i => i.Id == itemId).CorrectAnswer == dto.UserAnswer
            });
            return Created("", new
            {
                QuizId = answer.QuizId,
                QuizItemId = itemId,
                dto.UserAnswer
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }

    [HttpGet, Produces("application/json")]
    [Route("{quizId}/feedbacks")]
    public FeedbackQuizDto GetFeedback(int quizId)
    {
        int userId = 1;
        var answers = _service.GetUserAnswersForQuiz(quizId, userId);
        //TODO: zdefiniuj mapper listy odpowiedzi na obiekt FeedbackQuizDto 
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