using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reach.Utilities
{
    /// <summary>
    /// Wrapper interface around System.Net.WebClient to allow its methods to be mocked in tests
    /// </summary>
    public interface IWebClient : IDisposable
    {
        void DownloadFile(string address, string filePath);
    }

    /// <summary>
    /// Implementation of IWebClient interface using the built-in System.Net.WebClient
    /// </summary>
    public class SystemWebClient : System.Net.WebClient, IWebClient
    {
    }
}
