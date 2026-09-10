using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a. Record to represent financial data
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b. Interface for payment processing
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Concrete implementations of ITransactionProcessor
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Category} transaction of ${transaction.Amount}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Category} transaction of ${transaction.Amount}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Category} transaction of ${transaction.Amount}.");
        }
    }

    // d. Base Account class
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
        }
    }

    // e. Sealed SavingsAccount class inheriting from Account
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Transaction failed for {transaction.Category}: Insufficient funds.");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Successfully applied {transaction.Category} (${transaction.Amount}). Updated Balance: ${Balance}");
            }
        }
    }

    // f. FinanceApp simulation class
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Instantiate SavingsAccount
            SavingsAccount savings = new SavingsAccount("SA-100293", 1000m);
            Console.WriteLine($"Initial Account Balance: ${savings.Balance}\n");

            // ii. Create three Transaction records
            Transaction t1 = new Transaction(1, DateTime.Now, 150.00m, "Groceries");
            Transaction t2 = new Transaction(2, DateTime.Now, 200.50m, "Utilities");
            Transaction t3 = new Transaction(3, DateTime.Now, 75.00m, "Entertainment");

            // Processors
            ITransactionProcessor mobileProc = new MobileMoneyProcessor();
            ITransactionProcessor bankProc = new BankTransferProcessor();
            ITransactionProcessor cryptoProc = new CryptoWalletProcessor();

            // iii & iv & v. Process, Apply, and Add transactions
            // Transaction 1: MobileMoneyProcessor
            mobileProc.Process(t1);
            savings.ApplyTransaction(t1);
            _transactions.Add(t1);
            Console.WriteLine();

            // Transaction 2: BankTransferProcessor
            bankProc.Process(t2);
            savings.ApplyTransaction(t2);
            _transactions.Add(t2);
            Console.WriteLine();

            // Transaction 3: CryptoWalletProcessor
            cryptoProc.Process(t3);
            savings.ApplyTransaction(t3);
            _transactions.Add(t3);
            Console.WriteLine();

            Console.WriteLine($"Final Balance: ${savings.Balance}");
            Console.WriteLine($"Total Transactions Logged: {_transactions.Count}");
        }

        public static void Main(string[] args)
        {
            FinanceApp app = new FinanceApp();
            app.Run();
        }
    }
}