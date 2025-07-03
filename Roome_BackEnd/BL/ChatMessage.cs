public class ChatMessage
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Content { get; set; }
    public int UnreadCount { get; set; }
    public DateTime SentAt { get; set; }
}
public class ChatMessageDto
{
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Content { get; set; }
}
public class ChatListItem
{
    public int OtherUserId { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public async Task SendChatNotification(string pushToken, string senderName, string messageContent)
{
    var payload = new
    {
        to = pushToken,
        sound = "default",
        title = $"הודעה חדשה מ־{senderName}",
        body = messageContent,
        data = new { type = "chat" }
    };

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);

    var result = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Expo push result: {result}");
}

}

