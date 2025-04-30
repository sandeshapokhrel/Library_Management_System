using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IStudentRepository _studentRepository;
        private readonly IBookRepository _bookRepository;

        public TransactionService(
            ITransactionRepository repository,
            IMapper mapper,
            IEmailService emailService,
            IStudentRepository studentRepository,
            IBookRepository bookRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _emailService = emailService;
            _studentRepository = studentRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactionEntities = await _repository.GetAllTransactionsAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactionEntities);
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int transactionId)
        {
            var transactionEntity = await _repository.GetTransactionByIdAsync(transactionId);
            return _mapper.Map<TransactionDto>(transactionEntity);
        }

     /*   public async Task<IEnumerable<TransactionDto>> GetTransactionsByStudentIdAsync(int studentId)
        {
            var transactions = await _repository.GetTransactionsByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(int userId)
        {
            var transactions = await _repository.GetTransactionsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }
     */
        public async Task<int> CreateTransactionAsync(TransactionDto transactionDto)
        {
            try
            {
                Console.WriteLine($"Starting CreateTransaction for book {transactionDto.BookId}, student {transactionDto.StudentId}");
                
                var transactionEntity = _mapper.Map<Transaction>(transactionDto);
                var transactionId = await _repository.AddTransactionAsync(transactionEntity);
                
                Console.WriteLine($"Transaction created with ID: {transactionId}, Type: {transactionDto.TransactionType}");
                
                if (transactionDto.TransactionType?.ToLower() == "borrow")
                {
                    Console.WriteLine("Processing borrow transaction - attempting to send email");
                    
                    Console.WriteLine($"Looking up student ID: {transactionDto.StudentId}");
                    var student = await _studentRepository.GetStudentByIdAsync(transactionDto.StudentId);
                    Console.WriteLine($"Student found: {student != null}, Email: {student?.Email ?? "NULL"}");
                    
                    Console.WriteLine($"Looking up book ID: {transactionDto.BookId}");
                    var book = await _bookRepository.GetBookByIdAsync(transactionDto.BookId);
                    Console.WriteLine($"Book found: {book != null}, Title: {book?.Title ?? "NULL"}");
                
                    if (student != null && book != null && !string.IsNullOrEmpty(student.Email))
                    {
                        Console.WriteLine($"Preparing to send email to: {student.Email}");
                        try
                        {
                            var emailTask = _emailService.SendTransactionNotificationAsync(
                                studentEmail: student.Email,
                                adminEmail: null, // No admin email specified in original logic
                                transactionType: "Book Issued",
                                bookTitle: book.Title,
                                studentName: student.Name,
                                dueDate: transactionDto.DueDate);
                            
                            Console.WriteLine("Email service call initiated");
                            await emailTask;
                            Console.WriteLine("Email service call completed successfully");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Email sending failed: {ex.ToString()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Student or book not found - email not sent");
                    }
                }
            
            return transactionId;
        }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateTransactionAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateTransactionAsync(TransactionDto transactionDto)
        {
            var existingTransaction = await _repository.GetTransactionByIdAsync(transactionDto.TransactionId);
            var transactionEntity = _mapper.Map<Transaction>(transactionDto);
            var result = await _repository.UpdateTransactionAsync(transactionEntity);
            
            Console.WriteLine($"Transaction updated: {result}, Old Type: {existingTransaction?.TransactionType}, New Type: {transactionDto.TransactionType}");
            
            if (result && existingTransaction?.TransactionType == "Borrow" && transactionDto.TransactionType == "Return")
            {
                Console.WriteLine("Processing return transaction - attempting to send email");
                
                var student = await _studentRepository.GetStudentByIdAsync(transactionDto.StudentId);
                var book = await _bookRepository.GetBookByIdAsync(transactionDto.BookId);
                
                Console.WriteLine($"Retrieved student: {student?.Name}, book: {book?.Title}");
                
                if (student != null && book != null)
                {
                    Console.WriteLine($"Preparing to send email to: {student.Email}");
                    try
                    {
                        await _emailService.SendTransactionNotificationAsync(
                            studentEmail: student.Email,
                            adminEmail: null, // No admin email specified in original logic
                            transactionType: "Book Returned",
                            bookTitle: book.Title,
                            studentName: student.Name,
                            dueDate: null); // No due date needed for return notification
                        Console.WriteLine("Email service called successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email sending failed: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Student or book not found - email not sent");
                }
            }
            
            return result;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            return await _repository.DeleteTransactionAsync(transactionId); // Return true if delete is successful
        }
    }
}
