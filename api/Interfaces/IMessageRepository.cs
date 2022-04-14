using api.Entities;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IMessageRepository
    {
         void AddMessage(Message message);
         void DeleteMessage(Message message);
         Task<Message> GetMessage(int id);
         Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
         Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserId, string recipientId);
         Task<bool> SaveAllAsync();
    }
}