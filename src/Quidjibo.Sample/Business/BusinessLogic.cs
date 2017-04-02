using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Clients;
using Quidjibo.Commands;
using Quidjibo.Sample.Jobs;

namespace Quidjibo.Sample.Business
{
    public class BusinessLogic
    {
        private readonly IPublisherClient _publisherClient;

        public BusinessLogic(IPublisherClient publisherClient)
        {
            _publisherClient = publisherClient;
        }

        public async Task BusinessWithFireAndForget()
        {
            // your business logic...

            // publish a fire-and-forget w/ Quidjibo

            var command = new ExampleJob.Command("hello world!");

            await _publisherClient.PublishAsync(command, CancellationToken.None);

            // more of your business...
        }

        public async Task BusinessWithFireAndForgetWorkflow()
        {
            // your business logic...

            // publish a fire-and-forget workflow w/ Quidjibo

            var workflow = new WorkflowCommand(

                    // This stuff should be done fierst
                    new ExampleJob.Command("step 0 part 1"),
                    new ExampleJob.Command("step 0 part 2"),
                    new ExampleJob.Command("step 0 part 3")
                ).Then(step =>

                     // if the previous steps succeed then run all of these
                         new ExampleJob.Command("Just this one thing"))
                 .Then(step => new IWorkCommand[]
                 {
                     // if the previous steps succeed then run all of these
                     new ExampleJob.Command($"step {step} part 1"),
                     new ExampleJob.Command($"step {step} part 2"),
                     new ExampleJob.Command($"step {step} part 3")
                 }).Then(step => new IWorkCommand[]
                 {
                     // if the previous steps succeed then run all of these
                     new ExampleJob.Command($"step {step} part 1"),
                     new ExampleJob.Command($"step {step} part 2"),
                     new ExampleJob.Command($"step {step} part 3")
                 }).Then(step => new IWorkCommand[]
                 {
                     // if the previous steps succeed then run all of these
                     new ExampleJob.Command($"step {step} part 1"),
                     new ExampleJob.Command($"step {step} part 2"),
                     new ExampleJob.Command($"step {step} part 3")
                 });

            await _publisherClient.PublishAsync(workflow, CancellationToken.None);

            // more of your business...
        }
    }
}