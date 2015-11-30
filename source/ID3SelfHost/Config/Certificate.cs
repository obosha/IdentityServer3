using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SelfHost.Config
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
			//var assembly = typeof(Certificate).Assembly;
			//using (var stream = assembly.GetManifestResourceStream("SelfHost.Config.idsrv3test.pfx"))
			//{
			//    return new X509Certificate2(ReadStream(stream), "localhost");
			//}


			var thumbprint = "6276be3ef89d804b1384ae86f8ef436888f28eda";
			X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			store.Open(OpenFlags.OpenExistingOnly);
			var c = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

	        return c[0];
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}