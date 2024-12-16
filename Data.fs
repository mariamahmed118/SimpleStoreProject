namespace SimpleStore

module Data =
    open Newtonsoft.Json
    open System
    open System.IO
    open System.Security.Cryptography
    open System.Text
    open Models

    let quantitiesFilePath = "./Products.json"
    let usersFilePath = "./Users.json"

    // Product-related functions
    let loadProducts (path: string) : Product list =
        let json = File.ReadAllText(path)
        JsonConvert.DeserializeObject<Product list>(json)

    // Ensure users file exists and is valid
    let ensureUsersFileExists () =
        if not (File.Exists(usersFilePath)) then
            File.WriteAllText(usersFilePath, "[]")

    let saveUsers (users: User list) =
        let json = JsonConvert.SerializeObject(users)
        File.WriteAllText(usersFilePath, json)

    let loadUsers () : User list =
        try 
            ensureUsersFileExists()
            let json = File.ReadAllText(usersFilePath)
            JsonConvert.DeserializeObject<User list>(json)
        with 
        | _ -> []

    // Load a specific user's cart
    let loadUserCart (username: string) : Cart option =
        let users = loadUsers()
        users 
        |> List.tryFind (fun u -> u.Username = username)
        |> Option.bind (fun u -> u.Cart)

    let userExists (username: string) =
        loadUsers() |> List.exists (fun u -> u.Username = username)

    let hashPassword (password: string) =
        use sha256 = SHA256.Create()
        let bytes = Encoding.UTF8.GetBytes(password)
        let hash = sha256.ComputeHash(bytes)
        Convert.ToBase64String(hash)

    let updateUserCart (username: string) (cart: Cart) =
        let users = loadUsers()
        let updatedUsers = 
            users 
            |> List.map (fun user -> 
                if user.Username = username 
                then { user with Cart = Some cart }
                else user
            )
        saveUsers updatedUsers

    let getUserCart (username: string) : Cart option =
        let users = loadUsers()
        users 
        |> List.tryFind (fun u -> u.Username = username)
        |> Option.bind (fun u -> u.Cart)