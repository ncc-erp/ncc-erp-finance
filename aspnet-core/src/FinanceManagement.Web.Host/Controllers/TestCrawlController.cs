using FinanceManagement.Controllers;
using FinanceManagement.Web.Host.Startup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Host.Controllers
{
    /// <summary>
    /// This is a demo code to test crawl btransaction for account using mezonD
    /// Don't use this code in production !!!
    /// </summary>
    
    [Route("api/[controller]")]
    [ApiController]
    public class TestCrawlController : FinanceManagementControllerBase
    {
        private readonly CrawlBTransactionBackgroundWorker _worker;
        private readonly ILogger<TestCrawlController> _logger;

        public TestCrawlController(
            CrawlBTransactionBackgroundWorker worker,
            ILogger<TestCrawlController> logger)
        {
            _worker = worker;
            _logger = logger;
        }

        /// <summary>
        /// Test crawl Mezon transactions manually
        /// GET: api/TestCrawl/mezon
        /// </summary>
        [HttpGet("mezon")]
        public async Task<IActionResult> TestCrawlMezonAsync()
        {
            try
            {
                // Gọi function private CrawlBTransactionMezonD()
                var method = typeof(CrawlBTransactionBackgroundWorker)
                    .GetMethod("CrawlBTransactionMezonD", BindingFlags.NonPublic | BindingFlags.Instance);

                if (method == null)
                    return BadRequest("Không tìm thấy function CrawlBTransactionMezonD trong worker.");

                var task = (Task)method.Invoke(_worker, null);
                await task;

                return Ok("✅ Đã chạy xong CrawlBTransactionMezonD() thủ công.");
            }
            catch (Exception ex)
            {
                return BadRequest($"❌ Lỗi khi gọi CrawlBTransactionMezonD(): {ex.Message}");
            }
        }

        /// <summary>
        /// Check Mezon bank accounts in database
        /// GET: api/TestCrawl/accounts
        /// </summary>
        [HttpGet("accounts")]
        public IActionResult CheckMezonAccounts()
        {
            try
            {
                // Get accounts using reflection
                var method = typeof(CrawlBTransactionBackgroundWorker)
                    .GetMethod("GetDicMezonIdToBankAccountInfo",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var accounts = method.Invoke(_worker, null);

                return Ok(new
                {
                    message = "Mezon bank accounts found",
                    data = accounts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
