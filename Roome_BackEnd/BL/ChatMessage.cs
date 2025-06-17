public class ChatMessage
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Content { get; set; }
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
}

