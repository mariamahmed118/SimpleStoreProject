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
        Cart: Cart option
    }