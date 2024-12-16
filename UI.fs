namespace SimpleStore

module UI =
    open System
    open System.Drawing
    open System.Windows.Forms
    open Logic
    open Data
    open Models
    open Newtonsoft.Json
    open System.IO

    // Authentication Colors
    let primaryColor = Color.FromArgb(0x14, 0x3a, 0x51)
    let accentColor = Color.FromArgb(0xff, 0x5d, 0x68)
    let backgroundColor = primaryColor


    // Create Authentication Form
    let createAuthForm () =
        let form = new Form(
            Text = "Simple Store - Authentication", 
            Width = 400, 
            Height = 550,
            BackColor = backgroundColor,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        )
        
        // Tab Control for Login and Sign Up
        let tabControl = new TabControl(
            Dock = DockStyle.Fill,
            BackColor = accentColor
        )
        
        // Login Tab
        let loginTab = new TabPage("Login")
        loginTab.BackColor <- backgroundColor

        // Login Username Label and TextBox
        let loginUsernameLabel = new Label(
            Text = "Username", 
            Left = 50, 
            Top = 50, 
            Width = 300,
            ForeColor = accentColor
        )
        let loginUsernameBox = new TextBox(
            Left = 50, 
            Top = 80, 
            Width = 300,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        )

        // Login Password Label and TextBox
        let loginPasswordLabel = new Label(
            Text = "Password", 
            Left = 50, 
            Top = 130, 
            Width = 300,
            ForeColor = accentColor
        )
        let loginPasswordBox = new TextBox(
            Left = 50, 
            Top = 160, 
            Width = 300,
            UseSystemPasswordChar = true,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        )

        // Login Button
        let loginButton = new Button(
            Text = "Login", 
            Left = 50, 
            Top = 220, 
            Width = 100,
            BackColor = Color.Black,
            ForeColor = accentColor,
            FlatStyle = FlatStyle.Flat
        )
        loginButton.FlatAppearance.BorderSize <- 0

        // Sign Up Tab
        let signupTab = new TabPage("Sign Up")
        signupTab.BackColor <- backgroundColor

        // Signup Username Label and TextBox
        let signupUsernameLabel = new Label(
            Text = "Username", 
            Left = 50, 
            Top = 50, 
            Width = 300,
            ForeColor = accentColor
        )
        let signupUsernameBox = new TextBox(
            Left = 50, 
            Top = 80, 
            Width = 300,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        )

        // Signup Email Label and TextBox
        let signupEmailLabel = new Label(
            Text = "Email", 
            Left = 50, 
            Top = 130, 
            Width = 300,
            ForeColor = accentColor
        )
        let signupEmailBox = new TextBox(
            Left = 50, 
            Top = 160, 
            Width = 300,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        )

        // Signup Password Label and TextBox
        let signupPasswordLabel = new Label(
            Text = "Password", 
            Left = 50, 
            Top = 210, 
            Width = 300,
            ForeColor = accentColor
        )
        let signupPasswordBox = new TextBox(
            Left = 50, 
            Top = 240, 
            Width = 300,
            UseSystemPasswordChar = true,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        )

        // Signup Button
        let signupButton = new Button(
            Text = "Sign Up", 
            Left = 50, 
            Top = 300, 
            Width = 100,
            BackColor = Color.Black,
            ForeColor = accentColor,
            FlatStyle = FlatStyle.Flat
        )
        signupButton.FlatAppearance.BorderSize <- 0

        // Add controls to tabs
        loginTab.Controls.AddRange([| 
            loginUsernameLabel; 
            loginUsernameBox; 
            loginPasswordLabel; 
            loginPasswordBox; 
            loginButton 
        |])

        signupTab.Controls.AddRange([| 
            signupUsernameLabel; 
            signupUsernameBox; 
            signupEmailLabel; 
            signupEmailBox;
            signupPasswordLabel; 
            signupPasswordBox; 
            signupButton 
        |])

        // Add tabs to tab control
        tabControl.Controls.Add(loginTab)
        tabControl.Controls.Add(signupTab)

        // Add tab control to form
        form.Controls.Add(tabControl)

        // Login Button Click Event
        loginButton.Click.Add(fun _ -> 
            let username = loginUsernameBox.Text.Trim()
            let password = loginPasswordBox.Text.Trim()

            match Logic.authenticateUser username password with
            | Ok (user, cart) -> 
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                form.DialogResult <- DialogResult.OK
                form.Tag <- (user, cart) // Store user and cart in form's Tag
                form.Close()
            | Error msg -> 
                MessageBox.Show(msg, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )

        // Signup Button Click Event
        signupButton.Click.Add(fun _ -> 
            let username = signupUsernameBox.Text.Trim()
            let password = signupPasswordBox.Text.Trim()
            let email = signupEmailBox.Text.Trim()

            match Logic.registerUser username password email with
            | Ok user -> 
                MessageBox.Show("Registration Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                form.DialogResult <- DialogResult.OK
                form.Tag <- (user, { Items = [] }) // Initialize empty cart
                form.Close()
            | Error msg -> 
                MessageBox.Show(msg, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )

        form

    // Function to save the updated cart to JSON
    let saveCartToJson (cart: Cart) =
        let json = JsonConvert.SerializeObject(cart)
        File.WriteAllText("./Cart.json", json)

    // Product card creation function
    let createProductCard (product: Product) (quantity: int) =
        let productCard = new Panel(
            Width = 200,
            Height = 380,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(10)
        )

        let productImage = new PictureBox(
            SizeMode = PictureBoxSizeMode.Zoom,
            Width = 200,
            Height = 200
        )
        try 
            productImage.Image <- Image.FromFile(product.ImageURL)
        with 
        | _ -> productImage.BackColor <- Color.LightGray

        let nameLabel = new Label(
            Text = product.Name, 
            Top = 210, 
            Left = 0, 
            Width = 200, 
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = primaryColor
        )

        let priceLabel = new Label(
            Text = sprintf "$%.2f" product.Price, 
            Top = 240, 
            Left = 0, 
            Width = 200, 
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = accentColor
        )

        let descriptionLabel = new Label(
            Text = product.Description,
            Top = 270,
            Left = 0,
            Width = 200,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = primaryColor
        )

        let quantityLabel = new Label(
            Text = sprintf "Available: %d" quantity, 
            Top = 320, 
            Left = 0, 
            Width = 200, 
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = primaryColor
        )

        let buyButton = new Button(
            Text = "Add to Cart", 
            Top = 350, 
            Left = 0, 
            Width = 200, 
            Height = 30,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        )
        buyButton.FlatAppearance.BorderSize <- 0

        productCard.Controls.AddRange([| 
            productImage; 
            nameLabel; 
            priceLabel; 
            descriptionLabel; 
            quantityLabel;
            buyButton 
        |])
        
        // Tag the buy button for easy identification
        buyButton.Tag <- (product, quantityLabel)
        
        productCard

    let createMainForm (user: User) (initialCart: Cart) (products: Product list) =
        // Declare currentCart and currentProducts as mutable
        let mutable currentCart = initialCart
        let mutable currentProducts = products
        
        // Set up form with modern styling
        let form = new Form(
            Text = "Simple Store", 
            Width = 1000, 
            Height = 750,
            BackColor = primaryColor, 
            StartPosition = FormStartPosition.CenterScreen
        )

        // Welcome Label
        let welcomeLabel = new Label(
            Text = sprintf "Hello, %s!" user.Username,
            Top = 10,
            Left = 10,
            Width = 300,
            Height = 30,
            ForeColor = Color.White,
            Font = new Font("Arial", 14f, FontStyle.Bold)
        )
        form.Controls.Add(welcomeLabel)

        // Search bar
        let searchBox = new TextBox(
            Top = 50,
            Left = 10,
            Width = 400,
            PlaceholderText = "Search products..."
        )
        
        // Product panel
        let productPanel = new FlowLayoutPanel(
            Top = 100,
            Left = 10,
            Width = 600,
            Height = 600,
            BackColor = primaryColor,
            FlowDirection = FlowDirection.LeftToRight,
            AutoScroll = true
        )
        
        // Cart icon
        let cartIcon = new PictureBox(
            Image = Image.FromFile("./Images/cart2.png"),
            SizeMode = PictureBoxSizeMode.Zoom,
            Width = 200,
            Height = 45,
            Top = 0,
            Left = 850
        )

        // Cart panel (hidden initially)
        let cartPanel = new Panel(
            Top = 100,
            Left = 620,
            Width = 350,
            Height = 600,
            BackColor = Color.White,
            BorderStyle = BorderStyle.None,
            Visible = false
        )

        // Cart title
        let cartTitle = new Label(
            Text = "Shopping Cart", 
            Top = 10, 
            Left = 0, 
            Width = 350, 
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Arial", 16f, FontStyle.Bold),
            ForeColor = primaryColor
        )

        // Cart list view
        let cartListView = new ListView(
            Top = 60, 
            Left = 0, 
            Width = 350, 
            Height = 400,
            View = View.Details,
            FullRowSelect = true,
            BackColor = Color.White,
            ForeColor = primaryColor
        )
        cartListView.Columns.Add("Product", 150) |> ignore
        cartListView.Columns.Add("Price", 100) |> ignore
        cartListView.Columns.Add("Quantity", 100) |> ignore

        // Total label
        let totalLabel = new Label(
            Top = 470,
            Left = 0,
            Width = 350,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Arial", 16f, FontStyle.Bold),
            ForeColor = accentColor,
            Text = "Total: $0.00"
        )

        // Buttons
        let removeButton = new Button(
            Text = "Remove Item", 
            Top = 520, 
            Left = 10, 
            Width = 160,
            BackColor = accentColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        )
        removeButton.FlatAppearance.BorderSize <- 0

        let checkoutButton = new Button(
            Text = "Checkout", 
            Top = 520, 
            Left = 180, 
            Width = 160,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        )
        checkoutButton.FlatAppearance.BorderSize <- 0

        // Function to update product quantities in the UI
        let updateProductQuantities () =
            productPanel.Controls 
            |> Seq.cast<Panel> 
            |> Seq.iter (fun productCard ->
                let buyButton = 
                    productCard.Controls 
                    |> Seq.cast<Control> 
                    |> Seq.find (fun c -> c :? Button)
                    :?> Button

                let (product, quantityLabel) = buyButton.Tag :?> (Product * Label)
                
                // Calculate available quantity
                let availableQuantity = Logic.calculateAvailableQuantity product currentCart
                quantityLabel.Text <- sprintf "Available: %d" availableQuantity
            )

        // Update cart display
        let updateCart () =
            cartListView.Items.Clear()
            currentCart.Items 
            |> List.iter (fun item -> 
                let itemView = new ListViewItem(item.Product.Name)
                itemView.SubItems.Add(sprintf "$%.2f" (item.Product.Price * decimal(item.Quantity))) |> ignore
                itemView.SubItems.Add(sprintf "Qty: %d" item.Quantity) |> ignore
                cartListView.Items.Add(itemView) |> ignore
            )
            let total = Logic.calculateTotal currentCart
            totalLabel.Text <- sprintf "Total: $%.2f" total
            
            // Update product quantities in the UI
            updateProductQuantities()

        // Function to refresh product display
        let refreshProductDisplay () =
            productPanel.Controls.Clear()
            currentProducts |> List.iter (fun product -> 
                let quantity = Logic.calculateAvailableQuantity product currentCart
                let productCard = createProductCard product quantity

                // Find the buy button
                let buyButton = 
                    productCard.Controls 
                    |> Seq.cast<Control> 
                    |> Seq.find (fun c -> c :? Button)
                    :?> Button

                // Add click event to buy button
                buyButton.Click.Add(fun _ -> 
                    let (product, quantityLabel) = buyButton.Tag :?> (Product * Label)
                    match Logic.addToCart user.Username currentCart product currentProducts with
                    | Ok updatedCart ->
                        // Update cart and products
                        currentCart <- updatedCart
                        currentProducts <- Logic.updateProductQuantities currentProducts currentCart
                        
                        // Update cart display
                        updateCart()

                        // Save the updated cart to JSON
                        saveCartToJson currentCart
                    | Error msg ->
                        MessageBox.Show(msg, "Add to Cart Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore
                )

                productPanel.Controls.Add(productCard)
            )
        
        // Cart icon click event
        cartIcon.Click.Add(fun _ -> 
            cartPanel.Visible <- not cartPanel.Visible // Toggle visibility
        )
        
        // Remove button click event
        removeButton.Click.Add(fun _ -> 
            if cartListView.SelectedItems.Count > 0 then
                let selectedItemName = cartListView.SelectedItems.[0].Text
                // Find the selected item in the cart
                currentCart <- Logic.removeItemFromCart user.Username currentCart selectedItemName
                updateCart()
                // Save the updated cart to JSON
                saveCartToJson currentCart
            else
                MessageBox.Show("No item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore
        )

       // Checkout button click event
       
        checkoutButton.Click.Add(fun _ -> 
            if currentCart.Items.Length > 0 then
                let total = Logic.calculateTotal currentCart
                MessageBox.Show(
                    sprintf "Checkout complete!\nTotal: $%.2f" total, 
                    "Checkout", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information
                ) |> ignore
                
                // حفظ المنتجات الحالية كما هي في الملف
                let json = JsonConvert.SerializeObject(currentProducts)
                File.WriteAllText(Data.quantitiesFilePath, json)
                
                // إعادة ضبط السلة
                currentCart <- { Items = [] }
                updateUserCart user.Username currentCart
                
                // تحديث عرض السلة والمنتجات
                updateCart()
                refreshProductDisplay()
            else
                MessageBox.Show(
                    "Your cart is empty.", 
                    "Checkout", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                ) |> ignore
        )
        // Add controls to cart panel
        cartPanel.Controls.AddRange([| 
            cartTitle; 
            cartListView; 
            totalLabel; 
            removeButton; 
            checkoutButton 
        |])
        
        // Add controls to form
        form.Controls.AddRange([| cartIcon; productPanel; cartPanel |])

        // Create and add all product cards initially
        refreshProductDisplay ()
        updateCart()

        // Return the form
        form

    // Main Program Entry Point
    let showAuthThenMainForm () =
        let authForm = createAuthForm()

        // Show the authentication window
        if authForm.ShowDialog() = DialogResult.OK then
            let (user, cart) = authForm.Tag :?> (User * Cart)

            // Create main form and show it
            let mainForm = createMainForm user cart (Data.loadProducts Data.quantitiesFilePath)
            mainForm.ShowDialog() |> ignore

    