// Load Fable.Core and bindings to JS global objects
#r "../node_modules/fable-core/Fable.Core.dll"
#load "../node_modules/fable-import-react/Fable.Import.React.fs"
#load "../node_modules/fable-import-react/Fable.Helpers.React.fs"
#load "../node_modules/fable-react-toolbox/Fable.Helpers.ReactToolbox.fs"
#load "../node_modules/fable-elmish/elmish.fs"
#load "../node_modules/fable-import-d3/Fable.Import.D3.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Node
open Fable.Helpers.ReactToolbox
open Fable.Helpers.React.Props
open Elmish

module R = Fable.Helpers.React
module RT = Fable.Helpers.ReactToolbox

open R.Props
let ReactFauxDOM = importAll<obj> "react-faux-dom/lib/ReactFauxDOM"
let MyD3 = importAll<obj> "d3"


// MODEL
type Datum =
    {
    Date : System.DateTime
    Close : float
}

type Model = {
    Data: Datum array
    }
let ModelFromFile =
    let parseDate = D3.Time.Globals.format("%d-%b-%y").parse
    let tsv = Node.fs.readFileSync("app/data/data.tsv", "utf8")
    let data =
        tsv.Trim().Split('\n')
        |> Array.skip 1 //skip header
        |> Array.map( fun row ->
            let s = row.Split('\t')
            let date = parseDate( s.[0] )
            let close =  s.[1] |> float 
            {Date=date;Close=close})
    { Data = data }

let emptyModel =  
    ModelFromFile

type Msg =
  | AddNoise

let init() = emptyModel

// UPDATE
let inline rand() = JS.Math.random()
let update (msg:Msg) (model:Model)  =
  match msg with
  | AddNoise -> 
        {model with Data = model.Data |> Array.map(fun d -> {d with Close=d.Close+rand()*100.0} ) }


// VIEW
let ReactD3 (model:Model) =
    let marginTop,marginRight,marginBottom,marginLeft = 20,20,30,50
    let width = 960 - marginLeft  - marginRight
    let height = 500 - marginTop  - marginBottom

    //30-Apr-12
    let parseDate = D3.Time.Globals.format("%d-%b-%y").parse

    let x = 
        D3.Time.Globals.scale<float,float>()
            .range([|0.0; float width|])

    let y = 
        D3.Scale.Globals.linear()
            .range([|float height; 0.0|])

    let xAxis = 
        D3.Svg.Globals.axis()
            .scale(x)
            .orient("bottom")

    let yAxis = 
        D3.Svg.Globals.axis()
            .scale(y)
            .orient("left")

    let line2 = 
        D3.Svg.Globals.line<Datum>() 
            .x( System.Func<Datum,float,float>(fun d _ -> x.Invoke(d.Date) ) )
            .y( System.Func<Datum,float,float>(fun d _ -> y.Invoke(d.Close) ) )

    //this is a dynamic version of the typed line above
    //most of the below goes dynamic, especially for things like attr, which otherwise would need erasable types
    let line =
        D3.Svg.Globals.line() 
            ?x( fun d -> x$(d.Date ))
            ?y( fun d -> y$(d.Close ))

    let node  = ReactFauxDOM?createElement("svg") :?>  Browser.EventTarget
    let svg = 
        D3.Globals.select(node)
            ?attr("width", width + marginLeft + marginRight )
            ?attr("height", height + marginTop + marginBottom )
            ?append("g")
            ?attr("transform", "translate(" + marginLeft.ToString() + "," + marginTop.ToString() + ")" )


    //D3.Globals.Extent doesn't have good method for DateTime, so I used dynamic
    x?domain$( MyD3?extent( model.Data, fun d -> d.Date) ) |> ignore

    //looks like extent is improperly returning a tuple instead of an array so we restructure
    let yMin,yMax = D3.Globals.extent<Datum>( model.Data, System.Func<Datum, float, float>(fun d _ -> d.Close))
    ignore <| y.domain( [|yMin;yMax|] )

    svg?append("g")
        ?attr("class",  "x axis")
        ?attr("transform", "translate(0," + height.ToString() + ")") 
        ?call(xAxis) 
        |> ignore

    svg?append("g")
        ?attr("class", "y axis")
        ?call(yAxis)
        ?append("text")
        ?attr("transform", "rotate(-90)")
        ?attr("y", 6)
        ?attr("dy", ".71em")
        ?style("text-anchor", "end")
        ?text( "Price ($)")
        |> ignore

    svg?append("path")
        ?datum( model.Data )
        ?attr("class", "line")
        ?attr("fill", "none")
        ?attr("stroke", "#000")
        ?attr("d", line)  
        |> ignore

    node?toReact() :?> React.ReactElement<obj>

let view model dispatch =
    R.div [ ]
        [
            RT.button [ Label "Add Noise to Data"; Raised true;
                OnClick( fun _ -> AddNoise |> dispatch)] []
            R.fn ReactD3 model []
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
        this.props <- true

    member this.render() =
        view this.state dispatch

ReactDom.render(
        R.com<App,_,_> () [],
        Browser.document.getElementById("app")
    ) |> ignore
