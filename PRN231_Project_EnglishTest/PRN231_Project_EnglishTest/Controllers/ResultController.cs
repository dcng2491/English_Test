using AutoMapper;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_Project_EnglishTest.Dto;
using PRN231_Project_EnglishTest.DTO;
using PRN231_Project_EnglishTest.DTO.AdminHis;
using PRN231_Project_EnglishTest.DTO.Chart;
using PRN231_Project_EnglishTest.DTO.ResultHistory;
using PRN231_Project_EnglishTest.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                var data = context.Results
                    .Include(x => x.Test)
                    .Where(x => x.UserId == userid && x.SubmittedAt != null)
                    .OrderByDescending(x => x.SubmittedAt)
                    .ToList();

                var result = (from a in data
                              select new ResultHistory
                              {
                                  Userid = a.UserId,
                                  Testid = a.TestId,
                                  ResultId = a.ResultId,
                                  TestName = a.Test.TestName,
                                  Score = a.Score,
                                  StartAt = a.StartAt,
                                  SubmitAt = a.SubmittedAt,
                              }).ToList();
                for (int i = 1; i <= result.Count; i++)
                {
                    result[i - 1].No = i;
                }

                return Ok(result);
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

        //// API chấm điểm.
        //[HttpPut]
        //public IActionResult UpdateResultList(SubmitDto submit)
        //{
        //    try
        //    {
        //        int correct = 0;
        //        foreach (var item in submit.listAnswer)
        //        {
        //            var correctOption = context.Options
        //                .FirstOrDefault(x => x.QuestionId == item.QuestionId && x.IsCorrect == true);
        //            if (item.OptionId == correctOption.OptionId)
        //            {
        //                correct++;
        //            }
        //        }

        //        int numberQuestion = context.Tests
        //            .Include(x => x.Questions)
        //            .Where(x => x.TestId == submit.testId).Count();

        //        double score = (double)correct / (double)numberQuestion * 10;
        //        var result = context.Results.FirstOrDefault(x => x.ResultId == submit.resultId);
        //        result.Score = score;
        //        result.SubmittedAt = DateTime.Now;

        //        context.Update(result);
        //        context.SaveChanges();
        //        return Ok();
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        //// API chấm điểm.
        //[HttpPut("UpdateResult")]
        //public IActionResult UpdateResult(int resultId)
        //{
        //    try
        //    {
        //        int correct = 0;
        //        var result = context.Results.FirstOrDefault(x => x.ResultId == resultId);
        //        var list = context.ResultDetails
        //            .Include(x => x.Question)
        //            .Where(x => x.ResultId == resultId)
        //            .ToList();

        //        foreach(var item in list)
        //        {
        //            var options = context.Options
        //                .Include(x => x.Question)
        //                .FirstOrDefault(x => x.Question.QuestionId == item.QuestionId
        //                                    && x.IsCorrect == true);

        //            if (item.OptionId == options.OptionId)
        //            {
        //                correct++;
        //            }
        //        }

        //        var questions = context.Questions.Where(x => x.TestId == result.TestId).ToList();
        //        double score = (double)correct / (double)questions.Count * 10;
        //        result.Score = score;
        //        result.SubmittedAt = DateTime.Now;
        //        context.Update(result);
        //        context.SaveChanges();
        //        return Ok();
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        [HttpGet("Chart/{testid}")]
        public IActionResult GetDataChart(int testid)
        {
            try
            {
                var query = context.Results
                        .Where(tr => tr.TestId == testid && tr.StartAt != null && tr.SubmittedAt != null) // Lọc ra các bản ghi có testid = 1
                        .GroupBy(tr => tr.UserId) // Nhóm các bản ghi theo UserId
                        .Select(g => new
                        {
                            UserId = g.Key,
                            MaxScore = g.Max(tr => tr.Score), // Lấy điểm cao nhất trong từng nhóm
                            Number = g.Count()
                        })
                        .ToList();

                var chartdto = new ChartDto()
                {
                    Moc0 = query.Where(x => Convert.ToInt32(x.MaxScore) == 0).Count(),
                    Moc1 = query.Where(x => Convert.ToInt32(x.MaxScore) == 1).Count(),
                    Moc2 = query.Where(x => Convert.ToInt32(x.MaxScore) == 2).Count(),
                    Moc3 = query.Where(x => Convert.ToInt32(x.MaxScore) == 3).Count(),
                    Moc4 = query.Where(x => Convert.ToInt32(x.MaxScore) == 4).Count(),
                    Moc5 = query.Where(x => Convert.ToInt32(x.MaxScore) == 5).Count(),
                    Moc6 = query.Where(x => Convert.ToInt32(x.MaxScore) == 6).Count(),
                    Moc7 = query.Where(x => Convert.ToInt32(x.MaxScore) == 7).Count(),
                    Moc8 = query.Where(x => Convert.ToInt32(x.MaxScore) == 8).Count(),
                    Moc9 = query.Where(x => Convert.ToInt32(x.MaxScore) == 9).Count(),
                    Moc10 = query.Where(x => Convert.ToInt32(x.MaxScore) == 10).Count(),
                };

                return Ok(chartdto);
            }
            catch
            {

                return BadRequest();
            }
        }

        [HttpGet("AdminHis/{testid}")]
        public IActionResult GetDataAdminHis(int testid)
        {
            try
            {
                var query = context.Results
                        .Where(tr => tr.TestId == testid && tr.StartAt != null && tr.SubmittedAt != null) // Lọc ra các bản ghi có testid = 1
                        .GroupBy(tr => tr.UserId) // Nhóm các bản ghi theo UserId
                        .Select(g => new AdminHisDto
                        {
                            Userid = g.Key,
                            Testid = testid,
                            Username = context.Users.FirstOrDefault(x => x.UserId == g.Key).Username,
                            MaxScore = g.Max(tr => tr.Score), // Lấy điểm cao nhất trong từng nhóm
                            DoNumber = g.Count()
                        })
                        .ToList();

                return Ok(query);
            }
            catch
            {

                return BadRequest();
            }
        }
    }
}
