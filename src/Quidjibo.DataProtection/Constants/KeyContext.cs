namespace Quidjibo.DataProtection.Constants
{
    public class KeyContext
    {
        /// <summary>
        /// Key derived for cipher
        /// </summary>
        public static byte[] Cipher = new byte[] { 0x01 };

        /// <summary>
        /// Key derived for MAC
        /// </summary>
        public static byte[] Mac = new byte[] { 0x02 };
    }
}
