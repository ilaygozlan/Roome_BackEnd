using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class PushNotificationService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task SendChatNotification(string pushToken, string senderName, string messageContent, int fromUserId)
    {
        var payload = new
        {
            to = pushToken,
            sound = "default",
            title = $"הודעה מ{senderName}",
            body = messageContent,
            data = new
            {
                type = "chat",
                sender = senderName,
                recipientId = fromUserId // <-- זהו השדה שהאפליקציה שלך מחפשת!
            }
        };

        var json = JsonSerializer.Serialize(payload);
        Console.WriteLine("Sending push notification with the following payload:");
        Console.WriteLine(json);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);

        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Expo push resultVVV: {result}");
    }
}