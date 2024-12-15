//11
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

    let createLoadingForm () =
        let form = new Form(
            Text = "Loading...", 
            Width = 300, 
            Height = 100,
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            BackColor = Color.White
        )

        let progressBar = new ProgressBar(
            Dock = DockStyle.Fill,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 50
        )

        form.Controls.Add(progressBar)
        form

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

