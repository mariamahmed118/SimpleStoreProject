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

         
