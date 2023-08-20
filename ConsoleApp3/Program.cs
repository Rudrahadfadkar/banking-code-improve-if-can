using System;

public interface ITransaction
{
    void ProcessTransaction();
}

public class BankAccount
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; protected set; }

    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void DisplayAccountInfo()
    {
        Console.WriteLine($"Account Number: {AccountNumber}");
        Console.WriteLine($"Balance: ${Balance}");
    }
}

public class SavingsAccount : BankAccount, ITransaction
{
    public decimal InterestRate { get; private set; }

    public SavingsAccount(string accountNumber, decimal initialBalance, decimal interestRate)
        : base(accountNumber, initialBalance)
    {
        InterestRate = interestRate;
    }

    public override void DisplayAccountInfo()
    {
        base.DisplayAccountInfo();
        Console.WriteLine($"Interest Rate: {InterestRate}%");
    }

    public void ProcessTransaction()
    {
        Console.Write("Enter deposit amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
        {
            Balance += depositAmount;
            Console.WriteLine($"Deposited ${depositAmount}");
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }
}

public class CheckingAccount : BankAccount, ITransaction
{
    public CheckingAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public void ProcessTransaction()
    {
        Console.Write("Enter withdrawal amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawalAmount))
        {
            if (withdrawalAmount > Balance)
            {
                Console.WriteLine("Insufficient balance.");
            }
            else
            {
                Balance -= withdrawalAmount;
                Console.WriteLine($"Withdrew ${withdrawalAmount}");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }
}

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message) : base(message)
    {
    }
}

public class OverdraftProtectionAccount : CheckingAccount
{
    public decimal OverdraftLimit { get; private set; }

    public OverdraftProtectionAccount(string accountNumber, decimal initialBalance, decimal overdraftLimit)
        : base(accountNumber, initialBalance)
    {
        OverdraftLimit = overdraftLimit;
    }

    public new void ProcessTransaction()
    {
        Console.Write("Enter withdrawal amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawalAmount))
        {
            if (withdrawalAmount > Balance + OverdraftLimit)
            {
                throw new InsufficientBalanceException("Exceeds available balance including overdraft.");
            }
            else if (withdrawalAmount > Balance)
            {
                Balance -= withdrawalAmount;
                Console.WriteLine($"Withdrew ${withdrawalAmount} with overdraft protection");
            }
            else
            {
                Balance -= withdrawalAmount;
                Console.WriteLine($"Withdrew ${withdrawalAmount}");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        SavingsAccount savingsAccount = new SavingsAccount("SA123", 1000, 5);
        CheckingAccount checkingAccount = new CheckingAccount("CA456", 500);
        OverdraftProtectionAccount overdraftAccount = new OverdraftProtectionAccount("OA789", 200, 300);

        Console.WriteLine("Savings Account:");
        savingsAccount.DisplayAccountInfo();
        Console.WriteLine();

        Console.WriteLine("Checking Account:");
        checkingAccount.DisplayAccountInfo();
        Console.WriteLine();

        Console.WriteLine("Overdraft Protection Account:");
        overdraftAccount.DisplayAccountInfo();
        Console.WriteLine();

        try
        {
            savingsAccount.ProcessTransaction();
            checkingAccount.ProcessTransaction();
            overdraftAccount.ProcessTransaction();

            Console.WriteLine("\nUpdated Account Information:");
            savingsAccount.DisplayAccountInfo();
            checkingAccount.DisplayAccountInfo();
            overdraftAccount.DisplayAccountInfo();
        }
        catch (InsufficientBalanceException ex)
        {
            Console.WriteLine($"Insufficient balance: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
