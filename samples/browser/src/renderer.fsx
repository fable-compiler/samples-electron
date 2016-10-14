// Load Fable.Core and bindings to JS global objects
#r "../node_modules/fable-core/Fable.Core.dll"
#load "../node_modules/fable-import-react/Fable.Import.React.fs"
#load "../node_modules/fable-import-react/Fable.Helpers.React.fs"
#load "../node_modules/fable-react-toolbox/Fable.Helpers.ReactToolbox.fs"
#load "../node_modules/fable-elmish/elmish.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.Import.Node
open Fable.Helpers.ReactToolbox
open Fable.Helpers.React.Props
open Elmish

module R = Fable.Helpers.React
module RT = Fable.Helpers.ReactToolbox

type RCom = React.ComponentClass<obj>
let WebView = importDefault<RCom> "react-electron-webview" 
let inline (!!) x = createObj x
let inline (=>) x y = x ==> y

open R.Props

// MODEL

type Model = {
    url : string
    }


type Msg =
    | Url of string
    | Navigate
    | NavigateForward
    | NavigateBackward
    | Refresh
    | Close
    | UpdateNavigationUrl of string

let init() = { url = "http://fable.io/" }


// UPDATE
let update (msg:Msg) (model:Model)  =
    let webView = Browser.document.getElementById("webview")
    match msg with
    | Url(i) ->
        { model with url = i}
    | Navigate ->
        webView?loadURL( model.url ) |> ignore
        model
    | NavigateForward ->
        webView?goForward() |> ignore
        model
    | NavigateBackward ->
        webView?goBack() |> ignore
        model
    | Refresh ->
        webView?reload() |> ignore
        model
    | Close ->
        webView?stop() |> ignore
        model
    | UpdateNavigationUrl(i) ->
        { model with url = i}

// VIEW
let internal onClick msg dispatch =
    OnClick <| fun _ -> msg |> dispatch 

let [<Literal>] ENTER_KEY = 13.
let internal onEnter msg dispatch =
    function 
    | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
        ev.preventDefault() 
        dispatch msg
    | _ -> ()
    |> OnKeyDown


let view model dispatch =
    R.div [ Style [ CSSProp.Width (U2.Case2 "100%"); CSSProp.Height (U2.Case2 "100%");] ][
        R.div [] [
            RT.iconButton [ Icon "arrow_back"; onClick NavigateBackward dispatch][]
            RT.iconButton [ Icon "arrow_forward"; onClick NavigateForward dispatch][]
            RT.iconButton [ Icon "refresh"; onClick Refresh dispatch][]
            RT.iconButton [ Icon "close"; onClick Close dispatch][]
            RT.input [ Type "text"; InputProps.Value model.url; InputProps.OnChange ( Url >> dispatch ); onEnter Navigate dispatch ] []               
        ]
        R.from WebView
            !![
                "src" => "http://www.google.com";
                "style" => [CSSProp.Height "100%"];
                "id" => "webview";
         ][]
    ]

// App

let program = 
    Program.mkSimple init update
    |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj, Model>()
    
    let safeState state =
        match unbox this.props with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        let webView = Browser.document.getElementById("webview")
        webView?addEventListener("did-start-loading", 
            fun ev -> UpdateNavigationUrl( "Loading..." ) |> dispatch ) |> ignore
        webView?addEventListener("did-stop-loading", 
            fun () -> UpdateNavigationUrl (unbox (webView?getURL()))  |> dispatch ) |> ignore
        this.props <- true

    member this.render() =
        view this.state dispatch
        
ReactDom.render(
        R.com<App,_,_> () [],
        Browser.document.getElementById("app")
    ) |> ignore
