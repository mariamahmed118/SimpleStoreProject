namespace SimpleStore


module Data =
    open Newtonsoft.Json
    open System
    open System.IO
    open System.Security.Cryptography
    open System.Text
    open Models

    // Product-related functions
    let loadProducts (path: string) : Product list =
        let json = File.ReadAllText(path)
        JsonConvert.DeserializeObject<Product list>(json)

    let quantitiesFilePath = "./Products.json"
    let usersFilePath = "./Users.json"

    // Product Quantity Management
    let saveProductQuantities (quantities: (Product * int) list) =
        let quantitiesToSave = 
            quantities 
            |> List.map (fun (product, qty) -> 
                { product with Quantity = qty })

        let json = JsonConvert.SerializeObject(quantitiesToSave)
        File.WriteAllText(quantitiesFilePath, json)

    let loadProductQuantities (products: Product list) : (Product * int) list =
        try 
            if File.Exists(quantitiesFilePath) then
                let json = File.ReadAllText(quantitiesFilePath)
                let savedQuantities = JsonConvert.DeserializeObject<Product list>(json)
                
                products 
                |> List.map (fun product -> 
                    match savedQuantities |> List.tryFind (fun p -> p.Name = product.Name) with
                    | Some saved -> (saved, saved.Quantity)
                    | None -> (product, product.Quantity)
                )
            else
                products |> List.map (fun p -> (p, p.Quantity))
        with 
        | _ -> products |> List.map (fun p -> (p, p.Quantity))

    // User Management Functions
    let hashPassword (password: string) =
        use sha256 = SHA256.Create()
        let bytes = Encoding.UTF8.GetBytes(password)
        let hash = sha256.ComputeHash(bytes)
        Convert.ToBase64String(hash)

    let saveUsers (users: User list) =
        let json = JsonConvert.SerializeObject(users)
        File.WriteAllText(usersFilePath, json)

    let loadUsers () : User list =
        try 
            if File.Exists(usersFilePath) then
                let json = File.ReadAllText(usersFilePath)
                JsonConvert.DeserializeObject<User list>(json)
            else
                []
        with 
        | _ -> []

    let userExists (username: string) =
        loadUsers() |> List.exists (fun u -> u.Username = username)
    let accountsFilePath = "./Accounts.json"

    let saveUserAccounts (accounts: UserAccount list) =
        let json = JsonConvert.SerializeObject(accounts)
        File.WriteAllText(accountsFilePath, json)

    let loadUserAccounts () : UserAccount list =
        try 
            if File.Exists(accountsFilePath) then
                let json = File.ReadAllText(accountsFilePath)
                JsonConvert.DeserializeObject<UserAccount list>(json)
            else
                []
        with 
        | _ -> []

    let getOrCreateUserAccount (username: string) =
        let accounts = loadUserAccounts()
        match accounts |> List.tryFind (fun a -> a.Username = username) with
        | Some account -> account
        | None -> 
            let newAccount = {
                Username = username
                Balance = 0m
                Transactions = []
            }
            saveUserAccounts (newAccount :: accounts)
            newAccount

    let updateUserAccount (username: string) (transaction: Transaction) =
        let accounts = loadUserAccounts()
        let updatedAccounts = 
            accounts 
            |> List.map (fun account ->
                if account.Username = username then
                    { account with 
                        Balance = account.Balance + transaction.Amount
                        Transactions = transaction :: account.Transactions }
                else account
            )
        saveUserAccounts updatedAccounts
        transaction
        