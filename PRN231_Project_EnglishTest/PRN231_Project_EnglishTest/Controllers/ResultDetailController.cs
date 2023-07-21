using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_Project_EnglishTest.Dto;
using PRN231_Project_EnglishTest.DTO.DoExam;
using PRN231_Project_EnglishTest.DTO.ReviewPage;
using PRN231_Project_EnglishTest.Models;
using System.Diagnostics;

namespace PRN231_Project_EnglishTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultDetailController : ControllerBase
    {
        private readonly Prn231Project1Context context;
        private IMapper mapper;
        public ResultDetailController(Prn231Project1Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("ResultDetailById/{resultid}")]
        public IActionResult GetById(int resultid)
        {
            try
            {
                var res = context.Results
                    .FirstOrDefault(x => x.ResultId == resultid);

                var test = context.Tests.Include(x => x.Questions).ThenInclude(x => x.Options)
                    .FirstOrDefault(x => x.TestId == res.TestId);
                var answer = context.ResultDetails.Where(x => x.ResultId == resultid).ToList();

                List<ReviewPageDto> list = new List<ReviewPageDto>();
                foreach(var item in test.Questions)
                {
                    var dto = new ReviewPageDto
                    {
                        question = item,
                        selectedOption = answer.FirstOrDefault(x  => x.QuestionId == item.QuestionId) == null ? 0 : answer.FirstOrDefault(x => x.QuestionId == item.QuestionId).OptionId,
                    };
                    list.Add(dto);
                }

                return Ok(list);
            }
            catch
            {

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult AddResultDetail(ResultDetailDto detailDto)
        {
            try
            {
                var detail = mapper.Map<ResultDetail>(detailDto);
                context.Add(detail);
                context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("Submit")]
        public IActionResult Submit(ListSubmitOptionDto submit)
        {
            try
            {
                var test = context.Tests.
                    Include(x => x.Questions)
                    .ThenInclude(x => x.Options)
                    .FirstOrDefault(x => x.TestId == submit.testid);

                List<int> options = new List<int>();
                foreach(var item in test.Questions)
                {
                    options.AddRange(item.Options.Where(x => x.IsCorrect == true).Select(x => x.OptionId).ToList());
                }

                int correct = 0;
                foreach(var item in submit.selectedOptions)
                {
                    if (options.Contains(item)) correct++;
                }

                double score = (double)correct / (double)test.Questions.Count() * 10;
                var result = context.Results.FirstOrDefault(x => x.ResultId == submit.resultid);
                result.Score = Math.Round(score, 2);
                result.SubmittedAt = DateTime.Now;

                context.Update(result);
                context.SaveChanges();

                List<ResultDetail> resultDetails = new List<ResultDetail>();
                foreach(var item in submit.selectedOptions)
                {
                    var quesid = context.Options.FirstOrDefault(x => x.OptionId == item).QuestionId;
                    ResultDetail b = new ResultDetail()
                    {
                        ResultId = submit.resultid,
                        QuestionId = quesid.Value,
                        OptionId = item,
                    };
                    resultDetails.Add(b);
                }

                context.AddRange(resultDetails);
                context.SaveChanges();

                return Ok(result);
            }
            catch
            {

                return BadRequest();
            }
        }
    }
}
