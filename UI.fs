namespace SimpleStore
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

            // Create loading window
    
            let loadingForm = createLoadingForm()
            loadingForm.Show()

            // Simulate loading process (you can replace this with actual loading if needed)
            System.Threading.Thread.Sleep(000)
            loadingForm.Close()

            // Create main form and show it
            let mainForm = createMainForm user cart (Data.loadProducts Data.quantitiesFilePath)
            mainForm.ShowDialog() |> ignore