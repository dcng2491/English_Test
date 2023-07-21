using AutoMapper;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_Project_EnglishTest.Dto;
using PRN231_Project_EnglishTest.DTO;
using PRN231_Project_EnglishTest.Models;

namespace PRN231_Project_EnglishTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly Prn231Project1Context context;
        private IMapper mapper;
        public ResultController(Prn231Project1Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{resultid}")]
        public IActionResult Get(int resultid)
        {
            try
            {
                return Ok(context.Results.Include(x => x.ResultDetails).FirstOrDefault(x => x.ResultId == resultid));
            }
            catch
            {

                return BadRequest();
            }
        }

        [HttpGet("GetByUserId/{userid}")]
        public IActionResult GetById(int userid)
        {
            try
            {
                return Ok(context.Results
                    .Include(x => x.Test)
                    .Where(x => x.UserId == userid && x.SubmittedAt != null)
                    .ToList());
            }
            catch
            {

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult AddResult(ResultDto resultDto)
        {
            try
            {
                Result result = mapper.Map<Result>(resultDto);
                result.StartAt = DateTime.Now;
                context.Add(result);
                context.SaveChanges();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // API chấm điểm.
        [HttpPut]
        public IActionResult UpdateResultList(SubmitDto submit)
        {
            try
            {
                int correct = 0;
                foreach(var item in submit.listAnswer)
                {
                    var correctOption = context.Options
                        .FirstOrDefault(x => x.QuestionId == item.QuestionId && x.IsCorrect == true);
                    if (item.OptionId == correctOption.OptionId)
                    {
                        correct++;
                    }
                }

                int numberQuestion = context.Tests
                    .Include(x => x.Questions)
                    .Where(x => x.TestId == submit.testId).Count();

                double score = (double)correct / (double)numberQuestion * 10;
                var result = context.Results.FirstOrDefault(x => x.ResultId == submit.resultId);
                result.Score = score;
                result.SubmittedAt = DateTime.Now;

                context.Update(result);
                context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // API chấm điểm.
        [HttpPut("UpdateResult")]
        public IActionResult UpdateResult(int resultId)
        {
            try
            {
                int correct = 0;
                var result = context.Results.FirstOrDefault(x => x.ResultId == resultId);
                var list = context.ResultDetails
                    .Include(x => x.Question)
                    .Where(x => x.ResultId == resultId)
                    .ToList();

                foreach(var item in list)
                {
                    var options = context.Options
                        .Include(x => x.Question)
                        .FirstOrDefault(x => x.Question.QuestionId == item.QuestionId
                                            && x.IsCorrect == true);

                    if (item.OptionId == options.OptionId)
                    {
                        correct++;
                    }
                }

                var questions = context.Questions.Where(x => x.TestId == result.TestId).ToList();
                double score = (double)correct / (double)questions.Count * 10;
                result.Score = score;
                result.SubmittedAt = DateTime.Now;
                context.Update(result);
                context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
