using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quidjibo.Commands;
using Quidjibo.Extensions;
using Quidjibo.Models;
using Quidjibo.Protectors;

namespace Quidjibo.Serializers
{
    public class PayloadSerializer : IPayloadSerializer
    {
        /// <summary>
        ///     Serializes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<byte[]> SerializeAsync(IQuidjiboCommand command, CancellationToken cancellationToken)
        {
            var workPayload = new WorkPayload
            {
                Type = command.GetQualifiedName(),
                Content = GetContent(command)
            };
            var json = JsonConvert.SerializeObject(workPayload);
            var payload = Encoding.UTF8.GetBytes(json);
            return Task.FromResult(payload);
        }

        /// <summary>
        ///     Deserializes the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  Task<IQuidjiboCommand> DeserializeAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var json = Encoding.UTF8.GetString(payload);
            var jToken = JToken.Parse(json);
            var command = Deserialize(jToken);
            return Task.FromResult(command);
        }

        private static object GetContent(IQuidjiboCommand command)
        {
            if (command is WorkflowCommand workflow)
            {
                return new WorkflowPayload
                {
                    Step = workflow.Step,
                    CurrentStep = workflow.CurrentStep,
                    Entries = workflow.Entries.ToDictionary(
                        e => e.Key,
                        e => e.Value.Select(x => new WorkPayload
                        {
                            Type = x.GetType().AssemblyQualifiedName,
                            Content = x
                        }))
                };
            }

            return command;
        }

        private IQuidjiboCommand Deserialize(JToken jToken)
        {
            var typeName = jToken.SelectToken(nameof(WorkPayload.Type)).ToObject<string>();
            var commandType = Type.GetType(typeName, true);
            var worflowCommandType = typeof(WorkflowCommand);
            if (commandType == worflowCommandType)
            {
                var obj = jToken.SelectToken(nameof(WorkPayload.Content));

                var entries = obj.SelectToken(nameof(WorkflowPayload.Entries))
                                 .Children()
                                 .OfType<JProperty>()
                                 .ToDictionary(
                                     x => int.Parse(x.Name),
                                     x => x.Value.Select(Deserialize).ToList());


                return new WorkflowCommand
                {
                    Step = obj.SelectToken(nameof(WorkflowPayload.Step)).ToObject<int>(),
                    CurrentStep = obj.SelectToken(nameof(WorkflowPayload.CurrentStep)).ToObject<int>(),
                    Entries = entries
                };
            }

            var content = jToken.SelectToken(nameof(WorkPayload.Content));
            var command = content.ToObject(commandType);
            return (IQuidjiboCommand)command;
        }
    }
}