using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // Core model: immutable record for financial data
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Payment behavior interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Concrete processor 1
    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processing payment of {transaction.Amount:C} " +
                              $"for {transaction.Category} (ID: {transaction.Id}) via mobile wallet.");
        }
    }

    // Concrete processor 2
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Initiating transfer of {transaction.Amount:C} " +
                              $"for {transaction.Category} (ID: {transaction.Id}) to merchant account.");
        }
    }

    // Concrete processor 3
    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Confirming on-chain payment of {transaction.Amount:C} " +
                              $"for {transaction.Category} (ID: {transaction.Id}). Awaiting block confirmation.");
        }
    }

    // Base account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"{transaction.Amount:C} deducted from account {AccountNumber}. " +
                              $"New balance: {Balance:C}");
        }
    }

    // Sealed specialized account
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"{transaction.Amount:C} deducted from savings account {AccountNumber}. " +
                                  $"Updated balance: {Balance:C}");
            }
        }
    }

    // Application layer
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            // Instantiate a SavingsAccount with initial balance 1000
            var savings = new SavingsAccount("ACC-1001", 1000m);

            // Create three transaction records
            var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, 120m, "Entertainment");

            // Process each transaction with the required processor
            ITransactionProcessor processor1 = new MobileMoneyProcessor();
            ITransactionProcessor processor2 = new BankTransferProcessor();
            ITransactionProcessor processor3 = new CryptoWalletProcessor();

            processor1.Process(transaction1);
            processor2.Process(transaction2);
            processor3.Process(transaction3);

            Console.WriteLine();

            // Apply each transaction to the savings account
            savings.ApplyTransaction(transaction1);
            savings.ApplyTransaction(transaction2);
            savings.ApplyTransaction(transaction3);

            Console.WriteLine();

            // Store all transactions
            _transactions.Add(transaction1);
            _transactions.Add(transaction2);
            _transactions.Add(transaction3);

            Console.WriteLine($"Total transactions recorded: {_transactions.Count}");
        }
    }

    // Entry point
    public class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}