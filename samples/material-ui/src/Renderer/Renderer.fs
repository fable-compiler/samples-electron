module Renderer

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

module R = Fable.Helpers.React
open R.Props
type RCom = React.ComponentClass<obj>

// Imports
(importDefault "react-tap-event-plugin")()

let deepOrange500 = importMember<string> "material-ui/styles/colors"
let RaisedButton = importDefault<RCom> "material-ui/RaisedButton"
let Dialog = importDefault<RCom> "material-ui/Dialog"
let FlatButton = importDefault<RCom> "material-ui/FlatButton"
let MuiThemeProvider = importDefault<RCom> "material-ui/styles/MuiThemeProvider"
let getMuiTheme = importDefault<obj->obj> "material-ui/styles/getMuiTheme"

// Convenience operator to create JS literal objects
let inline (~%) x = createObj x

let muiTheme =
    %["palette" ==>
        %["accent1Color" ==> deepOrange500]]
    |> getMuiTheme

type [<Pojo>] MainState = { isOpen: bool; secret: string }

type Main(props, ctx) as this =
    inherit React.Component<obj,MainState>(props, ctx)
    do this.setInitState({isOpen=false; secret=""})

    member this.handleRequestClose() =
        this.setState({isOpen=false; secret=""})

    member this.handleTouchTap() =
        this.setState({isOpen=true; secret="1-2-3-4-5"})

        // Comment the line above and uncomment those below to read the secret
        // from a file instead using Node APIs
        // Node.fs.readFile(Node.__dirname + "/data/secret.txt", fun err buffer ->
        //     if err <> null then
        //         failwith "Couldn't read file"
        //     this.setState({isOpen=true; secret=buffer.toString()}))

    member this.render() =
        let standardActions =
            R.from FlatButton
                %["label" ==> "Ok"
                  "primary" ==> true
                  "onTouchTap" ==> this.handleRequestClose] []
        R.from MuiThemeProvider
            %["muiTheme" ==> muiTheme] [
                R.div [Style [TextAlign "center"
                              PaddingTop 200]] [
                    R.from Dialog
                        %["open" ==> this.state.isOpen
                          "title" ==> "Super Secret Password"
                          "actions" ==> standardActions
                          "onRequestClose" ==> this.handleRequestClose]
                        [R.str this.state.secret]
                    R.h1 [] [R.str "Material-UI"]
                    R.h2 [] [R.str "example project"]
                    R.from RaisedButton
                        %["label" ==> "Super Secret Password"
                          "secondary" ==> true
                          "onTouchTap" ==> this.handleTouchTap] []
                ]
            ]

ReactDom.render(R.com<Main,_,_> None [], Browser.document.getElementById("app"))