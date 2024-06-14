using System;
using System.Text.Json;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        var gkConf = new Dictionary<string, string>
        {
            { "organization_id", "" },
            { "authgroup_id", "" },
            { "key", "" },
            { "iv", "" },
            { "service", "backend" },
            { "agentId", "server" }
        };

        GuardianKey gk = new GuardianKey(gkConf);

        string clientIp = "164.41.38.100";
        string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.146 Safari/537.36";
        string username = "test@test.com";
        string userEmail = "test@test.com";
        string loginFailed = "0";

        Dictionary<string, string> retDict = gk.CheckAccess(clientIp, userAgent, username, userEmail, loginFailed);
        Console.WriteLine(JsonSerializer.Serialize(retDict));
        Console.WriteLine(retDict["response"]);
    }
}

