using Quidjibo.Commands;

namespace Quidjibo.Serializers
{
    public interface IPayloadSerializer
    {
        byte[] Serialize(IWorkCommand command);
        IWorkCommand Deserialize(byte[] payload);
    }
}