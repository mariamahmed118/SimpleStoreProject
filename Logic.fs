//hiii
module Logic
open System
open SimpleStore.Models
open SimpleStore.Data

let registerUser (username: string) (password: string) (email: string) =
    if userExists username then 
        Error "Username already exists"
    else
        let newUser = {
            Username = username
            PasswordHash = hashPassword password
            Email = email
            CreatedAt = DateTime.UtcNow
        }
        let users = loadUsers()
        saveUsers (newUser :: users)
        Ok newUser

let authenticateUser (username: string) (password: string) =
    let users = loadUsers()
    let hashedPassword = hashPassword password
    users 
    |> List.tryFind (fun u -> u.Username = username && u.PasswordHash = hashedPassword)
    |> function
    | Some user -> Ok user
    | None -> Error "Invalid username or password"


// Cart Management
let addToCart (cart: Cart) (product: Product) =
    match cart.Items |> List.tryFind (fun item -> item.Product.Name = product.Name) with
    | Some item ->
        { cart with Items = cart.Items |> List.map (fun i -> if i.Product.Name = item.Product.Name then { i with Quantity = i.Quantity + 1 } else i) }
    | None ->
        { cart with Items = { Product = product; Quantity = 1 } :: cart.Items }

let calculateTotal (cart: Cart) =
    cart.Items |> List.fold (fun acc item -> acc + (item.Product.Price * decimal(item.Quantity))) 0M

// Search Functions
let removeItemFromCart (cart: Cart) (productName: string) =
    match cart.Items |> List.tryFind (fun item -> item.Product.Name = productName) with
    | Some item when item.Quantity > 1 ->
        { cart with Items = cart.Items |> List.map (fun i -> if i.Product.Name = item.Product.Name then { i with Quantity = i.Quantity - 1 } else i) }
    | Some item when item.Quantity = 1 ->
        { cart with Items = cart.Items |> List.filter (fun i -> i.Product.Name <> productName) }
    | None -> cart    
    | Some(_) -> failwith "Not Implemented"
// Search Products Function
let searchProducts (products: Product list) (searchTerm: string) =
    products 
    |> List.filter (fun product -> 
        product.Name.ToLower().Contains(searchTerm.ToLower()) || 
        product.Description.ToLower().Contains(searchTerm.ToLower())
    )

// Payment Processing Function
let processPayment (username: string) (cart: Cart) (paymentMethod: PaymentMethod) =
    let total = calculateTotal cart
    let transaction = {
        Id = Guid.NewGuid()
        UserId = username
        Amount = total
        Date = DateTime.UtcNow
        PaymentMethod = paymentMethod
        Status = Completed
    }
    
    // Update user account and return transaction
    updateUserAccount username transaction    

