using System;
using System.Threading.Tasks;
using Cars.EventSource.Storage;
using Cars.MessageBus;
using Cars.Testing.Shared.StubApplication.Commands.Foo;
using Cars.Testing.Shared.StubApplication.Domain.Foo;
using Microsoft.AspNetCore.Mvc;

namespace Cars.IntegrationTests.Controllers
{
    [Route("command/foo")]
    public class FooWritableController : Controller
    {
        private readonly ISession _session;
        private readonly ICommandDispatcher _dispatcher;
        private readonly IRepository _repository;

        public FooWritableController(ISession session, ICommandDispatcher dispatcher, IRepository repository)
        {
            _session = session;
            _dispatcher = dispatcher;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var cmd = new CreateFooCommand(Guid.NewGuid());

            var response = await _dispatcher.DispatchAsync<CreateFooCommand, CreateFooResponse>(cmd);

            await _session.CommitAsync();

            return Ok(cmd);
        }

        [HttpPost("{id}/doSomething")]
        public async Task<IActionResult> DoSomething(string id)
        {
            var cmd = new DoSomethingCommand(Guid.Parse(id));

			var response = await _dispatcher.DispatchAsync<DoSomethingCommand, DoSomethingResponse>(cmd);

			await _session.CommitAsync();

            return Ok(response);
        }

        [HttpPost("flood/{times:int}")]
        public async Task<IActionResult> DoFlood(int times)
        {
            var create = new CreateFooCommand(Guid.NewGuid());

            var response = await _dispatcher.DispatchAsync<CreateFooCommand, CreateFooResponse>(create);

            var stream = await _repository.GetByIdAsync<Foo>(create.AggregateId);

            for (var i = 1; i < times; i++)
            {
                stream.DoSomething();
            }

            await _session.CommitAsync();

            return Ok(create);
        }
    }
}