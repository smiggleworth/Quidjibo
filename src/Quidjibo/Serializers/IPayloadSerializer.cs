using Quidjibo.Commands;

namespace Quidjibo.Serializers
{
    public interface IPayloadSerializer
    {
        byte[] Serialize(IQuidjiboCommand command);
        IQuidjiboCommand Deserialize(byte[] payload);
    }
}