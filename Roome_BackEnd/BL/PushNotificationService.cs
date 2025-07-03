using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class PushNotificationService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task SendChatNotification(string pushToken, string senderName, string messageContent, int fromUserId)
    {
        // Construct the notification payload
        var payload = new
        {
            to = pushToken,
            sound = "default",
            title = $"הודעה מ{senderName}",
            body = messageContent,
            data = new
            {
                type = "chat",              // used on the client to identify the type of notification
                sender = senderName,       // sender's display name
                fromUserId = fromUserId    // sender's user ID to navigate to the right chat
            }
        };

        // Log the full payload being sent
        var json = JsonSerializer.Serialize(payload);
        Console.WriteLine("Sending push notification with the following payload:");
        Console.WriteLine(json);

        // Set up HTTP headers for the Expo push API
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        // Send the push notification
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);

        // Log the result from the Expo server
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Expo push result: {result}");
    }
}
