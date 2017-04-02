namespace Quidjibo.Protectors
{
    public class PayloadProtector : IPayloadProtector
    {
        public byte[] Protect(byte[] payload)
        {
            return payload;
        }

        public byte[] Unprotect(byte[] payload)
        {
            return payload;
        }
    }
}