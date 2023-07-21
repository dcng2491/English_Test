using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_Project_EnglishTest.Dto;
using PRN231_Project_EnglishTest.DTO.DoExam;
using PRN231_Project_EnglishTest.Models;

namespace PRN231_Project_EnglishTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly Prn231Project1Context context;
        private IMapper mapper;
        public QuestionsController(Prn231Project1Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{testid}")]
        public IActionResult GetQuestionByTestId(int testid)
        {
            try
            {
                var questions = context.Questions.Include(x => x.Options).Where(x => x.TestId == testid).ToList();
                var test = context.Tests.FirstOrDefault(x => x.TestId == testid);
                var doExam = new DoExamDto
                {
                    TestName = test.TestName,
                    Durantion = test.Duration.Value,
                    questions = mapper.Map<List<Question>>(questions),
                };

                return Ok(doExam);
            }
            catch (Exception)
            {

                throw;
            }         
        }

        [HttpPost("addQuestions")]
        public IActionResult AddListQuest(List<QuestionDto> questions)
        {
            try
            {
                var list = mapper.Map<List<Question>>(questions);
                context.AddRange(list);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }
}
