using System;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IPayloadProtector _payloadProtector;


        /// <summary>
        ///     Serializes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public byte[] Serialize(IWorkCommand command)
        {
            var workPayload = new WorkPayload
            {
                Type = command.GetQualifiedName(),
                Content = GetContent(command)
            };

            var json = JsonConvert.SerializeObject(workPayload);
            var raw = Encoding.UTF8.GetBytes(json);
            var data = _payloadProtector.Protect(raw);
            var hash = ComputeHash(data);
            var payload = new byte[hash.Length + data.Length];
            Buffer.BlockCopy(hash, 0, payload, 0, hash.Length);
            Buffer.BlockCopy(data, 0, payload, hash.Length, data.Length);
            return payload;
        }

        /// <summary>
        ///     Deserializes the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public IWorkCommand Deserialize(byte[] payload)
        {
            var validatedData = GetValidatedData(payload);
            var data = _payloadProtector.Unprotect(validatedData);
            var json = Encoding.UTF8.GetString(data);
            var jToken = JToken.Parse(json);
            if (jToken != null)
            {
                return Deserialize(jToken);
            }

            return null;
        }

        public PayloadSerializer(IPayloadProtector payloadProtector)
        {
            _payloadProtector = payloadProtector;
        }

        private static object GetContent(IWorkCommand command)
        {
            var workflow = command as WorkflowCommand;
            if (workflow != null)
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

        private IWorkCommand Deserialize(JToken jToken)
        {
            var typeName = jToken.SelectToken(nameof(WorkPayload.Type)).ToObject<string>();
            var type = Type.GetType(typeName, true);
            var worflowCommandType = typeof(WorkflowCommand);
            if (type == worflowCommandType)
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

            return (IWorkCommand)jToken.SelectToken(nameof(WorkPayload.Content)).ToObject(type);
        }

        /// <summary>
        ///     Gets the validated data.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="System.Security.SecurityException">Signature does not match the computed hash</exception>
        private byte[] GetValidatedData(byte[] payload)
        {
            var hash = new byte[32];
            Buffer.BlockCopy(payload, 0, hash, 0, hash.Length);
            var data = new byte[payload.Length - hash.Length];
            Buffer.BlockCopy(payload, hash.Length, data, 0, data.Length);
            if (!ComputeHash(data).SequenceEqual(hash))
            {
                throw new SecurityException("Signature does not match the computed hash");
            }

            return data;
        }

        /// <summary>
        ///     Computes the hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private byte[] ComputeHash(byte[] data)
        {
            using (var hasher = SHA256.Create())
            {
                return hasher.ComputeHash(data);
            }
        }
    }
}