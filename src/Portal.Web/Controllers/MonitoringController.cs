using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        class HealthCheckResult
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public DateTime LastCheck { get; set; }

            //enum HealthCheckStatus
            //{
            //    Healthy=0,
            //    UnHealthy=1,
            //    Degraded=2,
            //    Stopped=4
            //}
        }

        public async Task<IActionResult> GetHealth()
        {
            var client = new HttpClient();
            var postServiceResult = new HealthCheckResult();

            try
            {
                var response = await client.GetAsync("http://localhost:5301/health");
                
                response.EnsureSuccessStatusCode();
                postServiceResult = new HealthCheckResult
                {
                    Name = "PostService",
                    Status = (await response.Content.ReadAsStringAsync()),
                    LastCheck = DateTime.Now
                };
            }
            catch (Exception)
            {

                postServiceResult = new HealthCheckResult
                {
                    Name = "PostService",
                    Status = "Stopped",
                    LastCheck = DateTime.Now
                };
            }

            var result = new List<HealthCheckResult>
            {
                postServiceResult
            };

            return Ok(result);
        }

    }


}
