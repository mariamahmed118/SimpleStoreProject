namespace SimpleStore

module Models =
    type Product = {
        Name: string
        Price: decimal
        Description: string
        ImageURL: string
        Quantity: int
    }

    type CartItem = {
        Product: Product
        Quantity: int
    }

    type Cart = {
        Items: CartItem list
    }

    type User = {
        Username: string
        PasswordHash: string
        Email: string
        CreatedAt: System.DateTime
    }

    type PaymentMethod =
    | CreditCard of cardNumber: string * expiryDate: string * cvv: string
    | PayPal of email: string
    | BankTransfer of accountNumber: string

    type TransactionStatus =
    | Pending
    | Completed
    | Failed

    type Transaction = {
        Id: System.Guid
        UserId: string
        Amount: decimal
        Date: System.DateTime
        PaymentMethod: PaymentMethod
        Status: TransactionStatus
    }

    type UserAccount = {
        Username: string
        Balance: decimal
        Transactions: Transaction list
    }