using System;
using System.Text.Json;
using System.Collections.Generic;

namespace GuardianKeyIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GuardianKey API integration Test");

            var gkConf = new Dictionary<string, string>
        {
            { "organization_id", "" },
            { "authgroup_id", "" },
            { "key", "" },
            { "iv", "" },
            { "service", "Csharp-test" },
            { "agentId", "serverXXXXX" },
            { "api_url", "https://api.guardiankey.io/v2/checkaccess" },
            { "timeout", "3" } 
        };

            GuardianKey gk = new GuardianKey(gkConf);

            string clientIp = "164.41.38.100";
            string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, como Gecko) Chrome/88.0.4324.146 Safari/537.36";
            string username = "test@test.com";
            string userEmail = "test@test.com";
            string loginFailed = "0";

            Dictionary<string, string> retDict = gk.check_access(clientIp, userAgent, username, userEmail, loginFailed);
            Console.WriteLine(JsonSerializer.Serialize(retDict));
            if (retDict.ContainsKey("response"))
            {
		// If respose is not BLOCK, then Allow the access in the protected system!
                Console.WriteLine(retDict["response"]);
            }
            else
            {
                Console.WriteLine("No response key found in the returned dictionary.");
            }
        }
    }
}

