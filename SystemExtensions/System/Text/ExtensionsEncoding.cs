using System.Text;

namespace SystemExtensions
{
    public static class ExtensionsEncoding
    {
        public static void Encoding_RegisterProvider(EncodingProvider provider)
        {
            EncodingProvider.AddProvider(provider);
        }
    }
}
