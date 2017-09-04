module Renderer

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Electron
open Node.Exports
open Fable.PowerPack

let filesize = import<obj> "*" "file-size"
// This is a dynamic programming instruction because we don't have a binding for file-size lib
let sizeToHuman (size: float) : string = (filesize $ (size))?human $ ("si") |> unbox<string>

// Reference the element of the View used by the application
module Ref =

    let openFolder = Browser.document.getElementById("act-open-folder")
    let quit = Browser.document.getElementById("act-quit")
    let about = Browser.document.getElementById("act-about")
    let addressBar = Browser.document.getElementById("address-bar")
    let filesList = Browser.document.getElementById("files-list")

// Used to storage a reference of the about window
// Otherwise, the window is destroy by the Garbage Collector
let mutable aboutWindow : BrowserWindow option = Option.None

// Reference to the electron process
// This allow us to create new window later for example
let remote = importMember<Remote> "electron"

// Data pass on a navigation event
type Navigate =
    { Path : string }

// Create a navigation event which handle path update
let navigation = Event<Navigate>()


///**Description**
///
///**Parameters**
///  * `path` - parameter of type `string` - Absolute path of the directory
///  * `segment` - parameter of type `string` - Name to display
///
///**Output Type**
///  * `Browser.HTMLLIElement`
///
let createBreadcrumbSegment path segment =
    let root = Browser.document.createElement_li()
    let link = Browser.document.createElement_a()
    link.href <- "#"
    link.innerText <- segment

    link.addEventListener_click(fun _ ->
        navigation.Trigger({ Path = path })
        null
    )

    root.appendChild(link) |> ignore
    root

///**Description**
///
///**Parameters**
///  * `path` - parameter of type `string` - Absolute path to represent
///
///**Output Type**
///  * `Browser.HTMLLIElement []`
///
let generateBreadcrumb (path : string) =
    let segments = path.Split(char Path.sep)

    segments
    |> Array.mapi(fun index segment ->
        let subPath = segments.[0..index] |> String.concat Path.sep
        createBreadcrumbSegment subPath segment
    )


///**Description**
///
///**Parameters**
///  * `path` - parameter of type `string` - Absolute path to the file
///  * `filename` - parameter of type `string` - Name of the file to display
///  * `fileStats` - parameter of type `Node.Fs.Stats` - Stats over the file
///
///**Output Type**
///  * `Browser.HTMLTableRowElement`
///
let generateFileRow path filename (fileStats : Node.Fs.Stats) =
    let root = Browser.document.createElement_tr()
    let icon = Browser.document.createElement_td()
    let name = Browser.document.createElement_td()
    let date = Browser.document.createElement_td()
    let ``type`` = Browser.document.createElement_td()
    let size = Browser.document.createElement_td()

    let isDirectory = fileStats.isDirectory()

    icon.appendChild(Html.createIcon (Mime.determineIcon filename isDirectory)) |> ignore

    name.appendChild(Html.createLink filename) |> ignore
    name.addEventListener_click(fun _ ->
        navigation.Trigger({ Path = path })
        null
    )

    date.innerText <- fileStats.birthtime.ToString("dd-MM-yyyy hh:mm:ss")

    ``type``.innerText <- Mime.determineFileType filename isDirectory

    if not isDirectory then
        size.innerText <- sizeToHuman fileStats.size

    Html.replaceChildren root [ icon; name; date; ``type``; size]
    root

let init () =
    // Register a listener over the navigation event
    navigation.Publish.Add(fun ev ->
        let path = ev.Path

        // If the path is a direcotry then update the display
        if (Fs.statSync(!^Path.join(path))).isDirectory() then
            let segments = generateBreadcrumb path

            // Clean and Fill addres bar
            Html.replaceChildren Ref.addressBar segments

            Fs.readdir(!^path, fun error files ->
                if error.IsSome then
                    Browser.console.error error.Value

                unbox<List<string>> files
                // Remove all the files starting with '.'
                |> List.filter(fun file ->
                    not (file.StartsWith("."))
                )
                // Get stats of the files
                |> List.map(fun file ->
                    (file, Fs.statSync(!^Path.join(path,file)))
                )
                // Render the file list
                |> List.map(fun (file, fileStats) ->
                    generateFileRow (Path.join(path,file)) file fileStats
                )
                // Clean and Fill files list
                |> Html.replaceChildren Ref.filesList
            )
        else
            // Open the file using default application
            ChildProcess.exec(path) |> ignore
    )

    // Register click on the About menu
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
    )

    // Register click of the quickAccess menu
    // We use the data attribute to find them
    for quickAccess in unbox<List<Browser.HTMLElement>> (Browser.document.querySelectorAll("[data-quick-access]")) do
        quickAccess.addEventListener_click(fun ev ->
            let element = ev.target :?> Browser.HTMLElement
            let quickAccessInfo = element.dataset.Item("quickAccess")
            navigation.Trigger({ Path = remote.app.getPath(!!quickAccessInfo) })
            null
            //updatePath remote.app.getPath()
        )

    // Register click on the OpenFolder menu
    Ref.openFolder.addEventListener_click(fun _ ->
        let options = createEmpty<OpenDialogOptions>
        options.properties <- ResizeArray(["openDirectory"]) |> Some

        let result = remote.dialog.showOpenDialog(options)

        // Make sure something have been selected
        if not (isNull result) && result.Count > 0 then
            navigation.Trigger({ Path = result.[0] })

        null
    )

    // Register click on the Quit menu
    Ref.quit.addEventListener_click(fun _ ->
        let win = remote.getCurrentWindow()
        win.close()
        null
    )

    navigation.Trigger({ Path = Node.Globals.``process``.cwd() })

init()