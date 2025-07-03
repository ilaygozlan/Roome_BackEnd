using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.IO;
using Roome_BackEnd.BL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roome_BackEnd.DAL
{
    public class DBserviceChat
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceChat()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Opens SQL connection
        public SqlConnection connect()
        {
            string? cStr = configuration.GetConnectionString("myProjDB");

            if (string.IsNullOrEmpty(cStr))
            {
                throw new Exception("Connection string 'myProjDB' not found in appsettings.json");
            }

            SqlConnection con = new(cStr);
            con.Open();
            return con;
        }

        // Adds new chat message, stores it in DB, marks it unread, and sends push notification
        public async Task AddChatMessage(ChatMessage message)
        {
            using (SqlConnection con = connect())
            {
            

                using (SqlCommand cmd = new SqlCommand("sp_AddChatMessage", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromUserId", message.FromUserId);
                    cmd.Parameters.AddWithValue("@ToUserId", message.ToUserId);
                    cmd.Parameters.AddWithValue("@Content", message.Content);

                    // Save message and retrieve the new message ID
                    int messageId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    // Save unread status for the recipient
                    using (SqlCommand readCmd = new SqlCommand(@"
                        INSERT INTO MessageReads (MessageId, UserId, IsRead)
                        VALUES (@MessageId, @UserId, 0)", con))
                    {
                        readCmd.Parameters.AddWithValue("@MessageId", messageId);
                        readCmd.Parameters.AddWithValue("@UserId", message.ToUserId);
                        await readCmd.ExecuteNonQueryAsync();
                    }

                    // Get push token of recipient
                    string pushToken;
                    using (SqlCommand tokenCmd = new SqlCommand("SELECT Token FROM [User] WHERE ID = @ToUserId", con))
                    {
                        tokenCmd.Parameters.AddWithValue("@ToUserId", message.ToUserId);
                        pushToken = (await tokenCmd.ExecuteScalarAsync())?.ToString();
                    }

                    // Get full name of sender for notification title
                    string senderName;
                    using (SqlCommand nameCmd = new SqlCommand("SELECT FullName FROM [User] WHERE ID = @FromUserId", con))
                    {
                        nameCmd.Parameters.AddWithValue("@FromUserId", message.FromUserId);
                        senderName = (await nameCmd.ExecuteScalarAsync())?.ToString();
                    }

                    // Send push notification if token exists
                    if (!string.IsNullOrEmpty(pushToken))
                    {
                        var pushService = new PushNotificationService();
                        await pushService.SendChatNotification(pushToken, senderName, message.Content);
                    }
                }
            }
        }

        // Returns list of all chat messages between two users
        public List<ChatMessage> GetChatMessages(int user1Id, int user2Id)
        {
            List<ChatMessage> messages = new();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("sp_GetChatMessages", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@User1Id", user1Id);
                cmd.Parameters.AddWithValue("@User2Id", user2Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChatMessage message = new ChatMessage
                        {
                            Id = (int)reader["Id"],
                            FromUserId = (int)reader["FromUserId"],
                            ToUserId = (int)reader["ToUserId"],
                            Content = reader["Content"].ToString() ?? "",
                            SentAt = (DateTime)reader["SentAt"]
                        };
                        messages.Add(message);
                    }
                }
            }

            return messages;
        }

        // Updates unread messages from a specific user to "read"
        public void MarkMessagesAsRead(int fromUserId, int toUserId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE MR
                SET IsRead = 1
                FROM MessageReads MR
                JOIN ChatMessages M ON MR.MessageId = M.Id
                WHERE MR.UserId = @toUserId AND M.FromUserId = @fromUserId", con))
            {
                cmd.Parameters.AddWithValue("@toUserId", toUserId);
                cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                cmd.ExecuteNonQuery();
            }
        }

        // Returns the chat list for the user, with last message and unread count per conversation
        public List<ChatListItem> GetUserChatList(int userId)
        {
            List<ChatListItem> chatList = new();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("sp_GetUserChatList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChatListItem chat = new ChatListItem
                        {
                            OtherUserId = (int)reader["OtherUserId"],
                            LastMessage = reader["LastMessage"].ToString(),
                            LastMessageTime = (DateTime)reader["LastMessageTime"],
                            UnreadCount = reader["UnreadCount"] != DBNull.Value ? (int)reader["UnreadCount"] : 0
                        };
                        chatList.Add(chat);
                    }
                }
            }

            return chatList;
        }
    }
}
