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