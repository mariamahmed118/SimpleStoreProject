namespace SimpleStore

module UI =
    open System
    open System.Drawing
    open System.Windows.Forms
    open Logic
    open Data
    open Models

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
        