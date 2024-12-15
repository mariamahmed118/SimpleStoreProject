module SimpleStore.Program

open System.Windows.Forms
open SimpleStore.UI
open SimpleStore.Data
open SimpleStore.Models

[<EntryPoint>]
let main _ =
    do
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        showAuthThenMainForm ()
    0 // Success code