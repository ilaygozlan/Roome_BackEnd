using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;
using System.Collections.Generic;

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

        public void AddChatMessage(ChatMessage message)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("sp_AddChatMessage", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromUserId", message.FromUserId);
                cmd.Parameters.AddWithValue("@ToUserId", message.ToUserId);
                cmd.Parameters.AddWithValue("@Content", message.Content);

                cmd.ExecuteNonQuery();
            }
        }

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
                            LastMessageTime = (DateTime)reader["LastMessageTime"]
                        };
                        chatList.Add(chat);
                    }
                }
            }

            return chatList;
        }

    }
}
