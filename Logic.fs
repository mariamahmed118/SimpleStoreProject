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
            Cart = None
        }
        let users = loadUsers()
        saveUsers (newUser :: users)
        Ok newUser

let authenticateUser (username: string) (password: string) =
    let users = loadUsers()
    let hashedPassword = hashPassword password
    match users |> List.tryFind (fun u -> u.Username = username && u.PasswordHash = hashedPassword) with
    | Some user -> 
        let cart = user.Cart |> Option.defaultValue { Items = [] }
        Ok (user, cart)
    | None -> Error "Invalid username or password"

// Cart Management
let addToCart (username: string) (cart: Cart) (product: Product) (products: Product list) =
    // Find the current product in the product list
    let currentProduct = 
        products 
        |> List.tryFind (fun p -> p.Name = product.Name)
        |> Option.defaultValue product

    // Check if there's enough quantity available
    let availableQuantity = 
        cart.Items 
        |> List.tryFind (fun item -> item.Product.Name = product.Name)
        |> Option.map (fun item -> currentProduct.Quantity - item.Quantity)
        |> Option.defaultValue currentProduct.Quantity

    if availableQuantity > 0 then
        let updatedCart =
            match cart.Items |> List.tryFind (fun item -> item.Product.Name = product.Name) with
            | Some item ->
                // Increase quantity correctly
                let newQuantity = item.Quantity + 1
                { cart with Items = cart.Items |> List.map (fun i -> if i.Product.Name = item.Product.Name then { i with Quantity = newQuantity } else i) }
            | None ->
                { cart with Items = { Product = product; Quantity = 1 } :: cart.Items }

        // Save the updated cart for the user
        updateUserCart username updatedCart
        Ok updatedCart
    else
        Error "Not enough quantity available"

let removeItemFromCart (username: string) (cart: Cart) (productName: string) =
    let updatedCart =
        match cart.Items |> List.tryFind (fun item -> item.Product.Name = productName) with
        | Some item when item.Quantity > 1 ->
            // Decrease quantity correctly
            let newQuantity = item.Quantity - 1
            { cart with Items = cart.Items |> List.map (fun i -> if i.Product.Name = productName then { i with Quantity = newQuantity } else i) }
        | Some item when item.Quantity = 1 ->
            { cart with Items = cart.Items |> List.filter (fun i -> i.Product.Name <> productName) }
        | None -> cart        
        | Some _ -> failwith "Not Implemented"

    // Save the updated cart for the user
    updateUserCart username updatedCart
    updatedCart

let calculateTotal (cart: Cart) =
    cart.Items |> List.fold (fun acc item -> acc + (item.Product.Price * decimal(item.Quantity))) 0M

let updateProductQuantities (products: Product list) (cart: Cart) =
    products
    |> List.map (fun product ->
        let cartQuantity = 
            cart.Items 
            |> List.tryFind (fun item -> item.Product.Name = product.Name)
            |> Option.map (fun item -> item.Quantity)
            |> Option.defaultValue 0
        
        // Adjust the product's available quantity based on cart items
        { product with Quantity = max 0 (product.Quantity - cartQuantity) }
    )

let calculateAvailableQuantity (product: Product) (cart: Cart) =
    let cartQuantity = 
        cart.Items 
        |> List.tryFind (fun item -> item.Product.Name = product.Name)
        |> Option.map (fun item -> item.Quantity)
        |> Option.defaultValue 0
    
    max 0 (product.Quantity - cartQuantity)