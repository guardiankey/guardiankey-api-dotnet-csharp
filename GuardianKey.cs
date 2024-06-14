
    Public Sub New(gk_conf as Dictionary(Of String, String))
      organization_id = gk_conf.Item("organization_id")
      authgroup_id    = gk_conf.Item("authgroup_id")
      key             = gk_conf.Item("key")
      iv              = gk_conf.Item("iv")
      service         = gk_conf.Item("service")
      agentId         = gk_conf.Item("agentId")
      api_url         = "https://api.guardiankey.io/v2/checkaccess"
      api_url_gktinc  = "https://api.guardiankey.io/v2/checkgktinc"
    End Sub

  Public Function create_event(client_ip As String,user_agent  As String, username  As String, useremail  As String, login_failed  As String)
    Dim gkEvent As New GuardianKeyEvent()
    With gkEvent
         .generatedTime   = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
         .agentId         = agentId
         .organizationId  = organization_id
         .authGroupId     = authgroup_id
         .service         = service
         .clientIP        = client_ip
         .clientReverse   = ""
         .userName        = username
         .authMethod      = ""
         .loginFailed     = login_failed
         .userAgent       = user_agent
         .psychometricTyped = ""
         .psychometricImage = ""
         .event_type      = "Authentication"
         .userEmail       = useremail
    End With
    Return JsonSerializer.Serialize(gkEvent)
  End Function

  Public Function SHA256Hash(my_string As String)
      Dim crypt As New System.Security.Cryptography.SHA256CryptoServiceProvider
      Dim ByteString() As Byte = System.Text.Encoding.ASCII.GetBytes(my_string)
      Dim result As String = Nothing
      ByteString = crypt.ComputeHash(ByteString)
      For Each bt As Byte In ByteString
          result &= bt.ToString("x2")
      Next
      Return result
  End Function

  Public Function check_access(client_ip As String, user_agent As String, username As String, useremail As String, login_failed As String)
    dim event_str :  event_str     = create_event(client_ip,user_agent,username,useremail,login_failed)
    dim hash      :  hash =  SHA256Hash(event_str & key &  iv )
    Dim msg_dict = new Dictionary(Of String, String)
    msg_dict.Add("id",      authgroup_id)
    msg_dict.Add("message", event_str)
    msg_dict.Add("hash",    hash)
    dim payload As String = JsonSerializer.Serialize(msg_dict)
    Return post_payload(payload,api_url)
  End Function

  Private Function post_payload(payload As String, api_url_here As String)
    Try
      Dim Uri As New Uri(String.Format(api_url_here))
      Dim webClient As New WebClient()
      Dim resByte As Byte()
      Dim resString As String
      Dim reqString() As Byte
      webClient.Headers("content-type") = "application/json"
      reqString = Encoding.Default.GetBytes(payload)
      resByte = webClient.UploadData(Uri, "post", reqString)
      resString = Encoding.Default.GetString(resByte)
      Dim return_dict As Dictionary(Of String, String) = JsonSerializer.Deserialize(Of Dictionary(Of String, String))(resString)
      Return return_dict
    Catch ex As Exception
      Dim return_dict As Dictionary(Of String, String) = JsonSerializer.Deserialize(Of Dictionary(Of String, String))("{""response"":""ERROR""}")
      Return return_dict
    End Try
  End Function

End Class
ChatGPT
Claro! Vou converter o código VB para C# para você. Aqui está o código convertido:

csharp
Copiar código
using System;
using System.Text.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public class GuardianKeyEvent
{
    public string GeneratedTime { get; set; }
    public string AgentId { get; set; }
    public string OrganizationId { get; set; }
    public string AuthGroupId { get; set; }
    public string Service { get; set; }
    public string ClientIP { get; set; }
    public string ClientReverse { get; set; }
    public string UserName { get; set; }
    public string AuthMethod { get; set; }
    public string LoginFailed { get; set; }
    public string UserAgent { get; set; }
    public string PsychometricTyped { get; set; }
    public string PsychometricImage { get; set; }
    public string EventType { get; set; }
    public string UserEmail { get; set; }
}

public class GuardianKey
{
    private string organizationId;
    private string authgroupId;
    private string key;
    private string iv;
    private string service;
    private string agentId;
    private string apiUrl;
    private string apiUrlGktinc;

    public GuardianKey(Dictionary<string, string> gkConf)
    {
        organizationId = gkConf["organization_id"];
        authgroupId = gkConf["authgroup_id"];
        key = gkConf["key"];
        iv = gkConf["iv"];
        service = gkConf["service"];
        agentId = gkConf["agentId"];
        apiUrl = "https://api.guardiankey.io/v2/checkaccess";
        apiUrlGktinc = "https://api.guardiankey.io/v2/checkgktinc";
    }

    public string CreateEvent(string clientIp, string userAgent, string username, string userEmail, string loginFailed)
    {
        var gkEvent = new GuardianKeyEvent
        {
            GeneratedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            AgentId = agentId,
            OrganizationId = organizationId,
            AuthGroupId = authgroupId,
            Service = service,
            ClientIP = clientIp,
            ClientReverse = string.Empty,
            UserName = username,
            AuthMethod = string.Empty,
            LoginFailed = loginFailed,
            UserAgent = userAgent,
            PsychometricTyped = string.Empty,
            PsychometricImage = string.Empty,
            EventType = "Authentication",
            UserEmail = userEmail
        };
        return JsonSerializer.Serialize(gkEvent);
    }

    public string SHA256Hash(string myString)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.ASCII.GetBytes(myString);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }
    }

    public Dictionary<string, string> CheckAccess(string clientIp, string userAgent, string username, string userEmail, string loginFailed)
    {
        string eventStr = CreateEvent(clientIp, userAgent, username, userEmail, loginFailed);
        string hash = SHA256Hash(eventStr + key + iv);
        var msgDict = new Dictionary<string, string>
        {
            { "id", authgroupId },
            { "message", eventStr },
            { "hash", hash }
        };
        string payload = JsonSerializer.Serialize(msgDict);
        return PostPayload(payload, apiUrl);
    }

    private Dictionary<string, string> PostPayload(string payload, string apiUrlHere)
    {
        try
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                byte[] reqString = Encoding.Default.GetBytes(payload);
                byte[] resByte = webClient.UploadData(new Uri(apiUrlHere), "POST", reqString);
                string resString = Encoding.Default.GetString(resByte);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(resString);
            }
        }
        catch (Exception)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>("{\"response\":\"ERROR\"}");
        }
    }
}
