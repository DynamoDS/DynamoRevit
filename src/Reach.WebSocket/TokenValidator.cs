using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Reach.Logging;

namespace Reach.WebSocket
{
    public interface ITokenValidator
    {
        TokenValidation Validate(Uri uri);
    }

    class FloodTokenValidator : ITokenValidator
    {
        private readonly string validationEndpoint;
        private readonly ILogger Logger;

        public FloodTokenValidator(string validationEndpoint, ILogger logger)
        {
            this.validationEndpoint = validationEndpoint;
            Logger = logger;
        }

        public TokenValidation Validate(Uri uri)
        {
            // Parse the query string variables into a NameValueCollection.
            // The uri value is considered a relative URI because it doesn't
            // have a scheme and hostname, and .Query doesn't work on relative
            // URIs for some strange reason, so make it an absolute URI.
            var root = new Uri("http://localhost");
            var absolute = new Uri(root, uri);
            var query = HttpUtility.ParseQueryString(absolute.Query);

            var userid = query.Get("email");
            var token = query.Get("token");

            if (userid == null)
            {
                return new TokenValidation("You did not supply an email!");
            }

            if (token == null)
            {
                return new TokenValidation("You did not supply a token!");
            }

            var req = WebRequest.Create(String.Join("/",
                validationEndpoint,
                userid,
                token)) as HttpWebRequest;

            req.Method = WebRequestMethods.Http.Post;

            HttpWebResponse res;

            try
            {
                res = req.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Environment.MachineName, "");
                return new TokenValidation("There was an error validating the token");
            }

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return new TokenValidation("The token is invalid!");
            }

            TokenValidation tokenValidation;

            try
            {
                using (var sr = new StreamReader(res.GetResponseStream()))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        tokenValidation = serializer.Deserialize<TokenValidation>(jsonTextReader);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Environment.MachineName, "");
                return new TokenValidation("There was an error parsing the token validation response");
            }

            return tokenValidation; // success!
        }
    }

    class TolerantTokenValidator : ITokenValidator
    {
        public TokenValidation Validate(Uri uri)
        {
            return new TokenValidation("DefaultUser", "DefaultSession");
        }
    }
}
