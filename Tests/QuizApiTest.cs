using Infrastructure.EF.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using WebAPI.Controllers;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Tests
{
    public class QuizApiTest
    {
        public class QuizApiGetRequestTest : IClassFixture<QuizAppTestFactory<Program>>
        {
            private readonly HttpClient _client;
            private readonly QuizAppTestFactory<Program> _app;
            private readonly QuizDbContext _context;
            public QuizApiGetRequestTest(QuizAppTestFactory<Program> app)
            {
                _app = app;
                _client = app.CreateClient();
                using (var scope = app.Services.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetService<UserManager<UserEntity>>();
                    _context = scope.ServiceProvider.GetService<QuizDbContext>();
                    var items = new HashSet<QuizItemEntity>
            {
                new()
                {
                    Id = 1, CorrectAnswer = "7", Question = "2 + 5", IncorrectAnswers =
                        new HashSet<QuizItemAnswerEntity>
                        {
                            new() {Id = 11, Answer = "5"},
                            new() {Id = 12, Answer = "6"},
                            new() {Id = 13, Answer = "8"},
                        }
                },
                new()
                {
                    Id = 2, CorrectAnswer = "3", Question = "2 + 1", IncorrectAnswers =
                        new HashSet<QuizItemAnswerEntity>
                        {
                            new() {Id = 21, Answer = "1"},
                            new() {Id = 22, Answer = "6"},
                            new() {Id = 23, Answer = "2"},
                        }
                },
                new()
                {
                    Id = 3, CorrectAnswer = "2", Question = "12 % 6", IncorrectAnswers =
                        new HashSet<QuizItemAnswerEntity>
                        {
                            new() {Id = 31, Answer = "1"},
                            new() {Id = 32, Answer = "2"},
                            new() {Id = 33, Answer = "3"},
                        }
                }            
            };
                    if (_context.Quizzes.Count() == 0)
                    {
                        _context.Quizzes.Add(
                            new QuizEntity
                            {
                                Id = 1,
                                Items = items,
                                Title = "Matematyka"
                            }
                        );
                        _context.SaveChanges();
                    }
                    if (_context.Users.Count() == 0)
                    {
                        UserEntity user = new UserEntity() { Email = "karol@wsei.edu.pl", UserName = "karol" };
                        _context.Users.Add( user );
                        var saved = userManager?.CreateAsync(user, "123456Ab@!");
                        userManager.AddToRoleAsync(user, "USER");
                    }
                }
            }
            [Fact]
            public async void GetShouldReturnTwoQuizzes()
            {
                //Arrange

                //Act
                var result = await _client.GetFromJsonAsync<List<QuizDto>>("/api/v1/quizzes");

                //Assert
                if (result != null)
                {
                    Assert.Single(result);
                    Assert.Equal("Matematyka", result[0].Title);
                }
            }
            [Fact]
            public async void GetShouldRetutnFirstTest()
            {
                var result = await _client.GetFromJsonAsync<QuizDto>("/api/v1/quizzes/1");

                if (result != null)
                {
                    Assert.Equal(1, result.Id);
                }
            }
            [Fact]
            public async void PostShouldSaveAnswer()
            {
                /*ar TokenBody = " {\r\n  \"loginName\": \"karol\",\r\n  \"password\": \"123456Ab@!\"\r\n} ";
                HttpRequestMessage requestToken = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:7077/api/authentication/login"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                       {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    },
                    Content = JsonContent.Create(TokenBody);
                };
                var JWTToken = await _client.SendAsync(requestToken); */
                //keep getting "Bad Request"
                var body = "{\r\n  \"userId\": 1,\r\n  \"userAnswer\": \"string\"\r\n}";

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:7077/api/v1/quizzes/1/items/1/answers"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                       {HttpRequestHeader.Authorization.ToString(), "Bearer 3789..."},
                       {HttpRequestHeader.ContentType.ToString(), "application/json"}
                    },
                    Content = JsonContent.Create(body)
                };
                var response = await _client.SendAsync(request);
                if(response != null)
                {
                    Assert.True(response.IsSuccessStatusCode);
                }
            }
            [Fact]
            public async void GetShouldReturnOkStatus()
            {
                //Arrange

                //Act
                var result = await _client.GetAsync("/api/v1/quizzes");

                //Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("application/json", result.Content.Headers.GetValues("Content-Type").First());
            }
        }
    }
}
