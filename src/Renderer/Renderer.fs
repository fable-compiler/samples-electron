module Renderer

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Electron
open Fable.Import
open Node.Exports
open Fable.PowerPack

module Ref =

    let openFolder = Browser.document.getElementById("act-open-folder")
    let quit = Browser.document.getElementById("act-quit")
    let about = Browser.document.getElementById("act-about")
    let addressBar = Browser.document.getElementById("address-bar")
    let filesList = Browser.document.getElementById("files-list")

let mutable aboutWindow : BrowserWindow option = Option.None

let remote = importMember<Remote> "electron"
let mutable currentDir = Node.Globals.``process``.cwd()

let createBreadcrumbSegment segment =
    let root = Browser.document.createElement_li()
    let link = Browser.document.createElement_a()
    link.href <- "#"
    link.innerText <- segment
    root.appendChild(link) |> ignore
    root

let generateBreadcrumb (path : string) =
    path.Split(char Path.sep)
    |> Array.map(fun segment ->
        createBreadcrumbSegment segment
    )

let replaceChildren (root: Browser.HTMLElement) children =
    while not (isNull root.firstChild) do
        root.removeChild(root.firstChild)
        |> ignore

    for child in children do
        root.appendChild(child)
        |> ignore

let generateFileRow file (fileStats : Node.Fs.Stats) =
    let root = Browser.document.createElement_tr()
    let icon = Browser.document.createElement_td()
    let name = Browser.document.createElement_td()
    let date = Browser.document.createElement_td()
    let ``type`` = Browser.document.createElement_td()
    let size = Browser.document.createElement_td()

    name.className <- "name"
    name.innerText <- file

    date.innerText <- Date.Format.format fileStats.birthtime "dd-MM-yyyy hh:mm:ss"

    replaceChildren root [ icon; name; date; ``type``; size]
    root

let updatePath path =
    currentDir <- path

    let segments = generateBreadcrumb currentDir

    // Clean and Fill addres bar
    replaceChildren Ref.addressBar segments

    Fs.readdir(!^currentDir, fun error files ->
        if error.IsSome then
            Browser.console.error error.Value

        unbox<List<string>> files
        |> List.map(fun file ->
            (file, Fs.statSync(!^Path.join(currentDir,file)))
        )
        |> List.map(fun (file, fileStats) ->
            generateFileRow file fileStats
        )
        // Clean and Fill files list
        |> replaceChildren Ref.filesList

    )

let init () =
    Ref.about.addEventListener_click(fun _ ->
        if aboutWindow.IsSome then
            aboutWindow.Value.show()
        else
            let options = createEmpty<BrowserWindowOptions>
            options?toolbar <- Some false
            options.resizable <- Some false
            options.show <- Some true
            options.height <- Some 250.
            options.width <- Some 600.

            let about = remote.BrowserWindow.Create(options)
            // For to remove the menu of the window
            about.setMenu(unbox null)

            about.on("closed", unbox(fun () ->
                // Dereference the about window object.
                aboutWindow <- Option.None
            )) |> ignore

            about.loadURL(Path.join("file://", Node.Globals.__dirname, "about.html"))

            aboutWindow <- about |> Some

        null

        // var params = {toolbar: false, resizable: false, show: true, height: 150, width: 400};
        // aboutWindow = new BrowserWindow(params);
        // aboutWindow.loadURL('file://' + __dirname + '/about.html');
    )

    for quickAccess in unbox<List<Browser.HTMLElement>> (Browser.document.querySelectorAll("[data-quick-access]")) do
        quickAccess.addEventListener_click(fun ev ->
            let element = ev.target :?> Browser.HTMLElement
            let quickAccessInfo = element.dataset.Item("quickAccess")
            updatePath (remote.app.getPath(!!quickAccessInfo))
            null
            //updatePath remote.app.getPath()
        )

    Ref.openFolder.addEventListener_click(fun _ ->
        updatePath currentDir
        null
    )

init()