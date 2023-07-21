using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRN231_Project_EnglishTest_Client.DTO.ReviewPage;
using PRN231_Project_EnglishTest_Client.Models;

namespace PRN231_Project_EnglishTest_Client.Controllers
{
    public class ResultDetailController : Controller
    {
        public async Task<IActionResult> IndexAsync(int? resultid)
        {

            string link = "http://localhost:5166/api/ResultDetail/ResultDetailById/" + resultid;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(link))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        using (HttpContent content = res.Content)
                        {
                            string result = await content.ReadAsStringAsync();
                            List<ReviewPageDto> rv = JsonConvert.DeserializeObject<List<ReviewPageDto>>(result);
                            ViewBag.Detail = rv;

                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }
        }
    }
}
