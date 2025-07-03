using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class PushNotificationService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task SendChatNotification(string pushToken, string senderName, string messageContent)
    {
        var payload = new
        {
            to = pushToken,
            sound = "default",
            title = $"הודעה מ{senderName}",
            body = messageContent,
            data = new { sender = senderName }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);
        string result = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Expo push result: " + result);
    }
}
