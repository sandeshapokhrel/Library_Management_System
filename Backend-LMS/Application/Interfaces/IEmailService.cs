using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> TestConnectionAsync();
        Task SendTransactionNotificationAsync(
            string studentEmail,
            string adminEmail,
            string transactionType,
            string bookTitle,
            string studentName,
            DateTime? dueDate);
    }
}