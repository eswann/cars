using System.Threading.Tasks;
using EnjoyCQRS.Handlers;
using EnjoyCQRS.UnitTests.Shared.StubApplication.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace EnjoyCQRS.IntegrationTests.Controllers
{
    [Route("command/bar")]
    public class HandlersController : Controller
    {
        private readonly IDispatcher _dispatcher;

        public HandlersController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("transient")]
        public async Task<IActionResult> ByInstanceHandlers()
        {
            var message1 = new TransientBarAsyncMessage();
            var message2 = new TransientBarAsyncMessage();

            using (_dispatcher)
            {
                await _dispatcher.PublishAsync(message1);
                await _dispatcher.PublishAsync(message2);
            }

            return Ok(new { counter1 = message1.Counter, counter2 = message2.Counter });
        }

        [HttpGet("scoped")]
        public async Task<IActionResult> ByScopeHandlers()
        {
            var message1 = new ScopedBarAsyncMessage();
            var message2 = new ScopedBarAsyncMessage();

            using (_dispatcher)
            {
                await _dispatcher.PublishAsync(message1);  
                await _dispatcher.PublishAsync(message2);
            }

            return Ok(new { counter1 = message1.Counter, counter2 = message2.Counter });
        }
    }


}