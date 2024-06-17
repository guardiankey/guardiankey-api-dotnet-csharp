using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GuardianKeyIntegration
{
    internal class GuardianKeyEvent
    {
        public string generatedTime { get; set; }
        public string agentId { get; set; }
        public string organizationId { get; set; }
        public string authGroupId { get; set; }
        public string service { get; set; }
        public string clientIP { get; set; }
        public string clientReverse { get; set; }
        public string userName { get; set; }
        public string authMethod { get; set; }
        public string loginFailed { get; set; }
        public string userAgent { get; set; }
        public string psychometricTyped { get; set; }
        public string psychometricImage { get; set; }
        public string event_type { get; set; }
        public string userEmail { get; set; }
    }

    internal class GuardianKey
    {
        private string organization_id;
        private string authgroup_id;
        private string key;
        private string iv;
        private string service;
        private string agentId;
        private string api_url;
        private int timeout;

        public GuardianKey(Dictionary<string, string> gk_conf)
        {
            organization_id = gk_conf["organization_id"];
            authgroup_id = gk_conf["authgroup_id"];
            key = gk_conf["key"];
            iv = gk_conf["iv"];
            service = gk_conf["service"];
            agentId = gk_conf["agentId"];
            api_url = gk_conf["api_url"];
            timeout = int.Parse(gk_conf["timeout"]);
        }

        public string create_event(string client_ip, string user_agent, string username, string useremail, string login_failed)
        {
            GuardianKeyEvent gkEvent = new GuardianKeyEvent
            {
                generatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                agentId = agentId,
                organizationId = organization_id,
                authGroupId = authgroup_id,
                service = service,
                clientIP = client_ip,
                clientReverse = "",
                userName = username,
                authMethod = "",
                loginFailed = login_failed,
                userAgent = user_agent,
                psychometricTyped = "",
                psychometricImage = "",
                event_type = "Authentication",
                userEmail = useremail
            };

            return JsonSerializer.Serialize(gkEvent);
        }

        public string SHA256Hash(string my_string)
        {
            using (System.Security.Cryptography.SHA256CryptoServiceProvider crypt = new System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                byte[] ByteString = Encoding.ASCII.GetBytes(my_string);
                byte[] result = crypt.ComputeHash(ByteString);

                StringBuilder sb = new StringBuilder();

                foreach (byte bt in result)
                {
                    sb.Append(bt.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public Dictionary<string, string> check_access(string client_ip, string user_agent, string username, string useremail, string login_failed)
        {
            string event_str = create_event(client_ip, user_agent, username, useremail, login_failed);
            string hash = SHA256Hash(event_str + key + iv);
            Dictionary<string, string> msg_dict = new Dictionary<string, string>
            {
                {"id", authgroup_id},
                {"message", event_str},
                {"hash", hash}
            };
            string payload = JsonSerializer.Serialize(msg_dict);
            return post_payload(payload, api_url);
        }

        private class MyWebClient : WebClient
        {
            public int timeout;
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest w = base.GetWebRequest(address);
                w.Timeout = timeout;
                return w;
            }
        }

        private Dictionary<string, string> post_payload(string payload, string api_url_here)
        {
            try
            {
                Uri Uri = new Uri(string.Format(api_url_here));
                MyWebClient webClient = new MyWebClient
                {
                    timeout = timeout * 1000
                };
                byte[] resByte;
                string resString;
                byte[] reqString = Encoding.Default.GetBytes(payload);
                webClient.Headers["content-type"] = "application/json";
                resByte = webClient.UploadData(Uri, "post", reqString);
                resString = Encoding.Default.GetString(resByte);
                Dictionary<string, string> return_dict = JsonSerializer.Deserialize<Dictionary<string, string>>(resString);
                return return_dict;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> return_dict = JsonSerializer.Deserialize<Dictionary<string, string>>("{\"response\":\"ERROR\"}");
                return return_dict;
            }
        }
    }
}

