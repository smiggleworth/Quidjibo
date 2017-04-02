namespace Quidjibo.Protectors
{
    public interface IPayloadProtector
    {
        byte[] Protect(byte[] payload);
        byte[] Unprotect(byte[] payload);
    }
}