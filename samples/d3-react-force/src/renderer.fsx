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

type RCom = React.ComponentClass<obj>
let Component = importMember<RCom> "react-d3-library"


// MODEL
//note Node/Link conform to interface definitions in D3.Layout.Force
//although f# does not have implicit interface implementations, the conversion to javascript seems to de facto do this
type Node =
    {
        index : float option
        x : float option
        y : float option
        px : float option
        py : float option
        ``fixed``: bool option
        weight: float option
    }

type Link =
    {
        source : Node
        target : Node
    }

type Model = {
    //from force we can recover the nodes/links as well as update the layout state
    force: D3.Layout.Force<Link,Node>
    }
let emptyModel =  
    { 
        force  = D3.Layout.Globals.force() :?> D3.Layout.Force<Link,Node> 
    }
    

type Msg =
  | AddNoise
  | AddNode

let init() = emptyModel


// VIEW
//we separate out this function to call it from both update methods and initial d3 graph constructor
let restart(force : D3.Layout.Force<Link,Node>) =
    let svg = D3.Globals.select("svg")
    svg
        ?selectAll("line.link")
        ?data(force.links())
        ?enter()
        ?insert("svg:line", "circle.node")
        ?attr("class", "link")
        |> ignore

    svg
        ?selectAll("circle.node")
        ?data(force.nodes())
        ?enter()
        ?insert("svg:circle", "circle.cursor")
        ?attr("class", "node")
        ?attr("r", 5)
        ?call(force.drag())
        |> ignore

    force.start();

//this is called exactly once to initialize the d3 graph for react
let createForceGraph( force : D3.Layout.Force<Link,Node> ) =
    let graph = Browser.document.createElement_div()

    let width = 1280
    let height = 800

    //configure the force. Note the empty model force is not configured and has goofy defaults
    force?charge(-80)?linkDistance(25)?size([|width; height|]) |> ignore

    let svg = 
        D3.Globals.select(graph)
            ?append("svg:svg")
            ?attr("width", width )
            ?attr("height", height )
            //special handler; part of react-d3-library pattern
            ?on("mount",fun () ->

        //react-d3-library requires that we reselect
        let svg = D3.Globals.select("svg")

        svg
            ?append("svg:rect")
            ?attr("width", width)
            ?attr("height", height)
            |> ignore

        let cursor = 
            svg
                ?append("svg:circle")
                ?attr("r", 30)
                ?attr("transform", "translate(-100,-100)")
                ?attr("class", "cursor")
        
        force.on("tick", fun _ ->
            svg
                ?selectAll("line.link")
                ?attr("x1", fun (d : Link) -> d.source.x )
                ?attr("y1", fun d ->  d.source.y)
                ?attr("x2", fun d ->  d.target.x)
                ?attr("y2", fun d ->  d.target.y)
                |> ignore

            svg
                ?selectAll("circle.node")
                ?attr("cx", fun d ->  d.x)
                ?attr("cy", fun d ->  d.y)
                |> ignore
        ) |> ignore

        svg?on( "mousemove", fun _ -> 
            let x,y = D3.Globals.mouse(Browser.event.currentTarget)
            cursor?attr("transform", "translate(" + unbox<string>(x) + "," + unbox<string>(y) + ")")
        ) |> ignore

        svg?on("mousedown", fun () ->
            //d3.event.preventDefault() //no equivalent?
            ()
        ) |> ignore

        svg?on("click", fun _ ->
            let x,y = D3.Globals.mouse(Browser.event.currentTarget)
            let node = {index = None; x = Some(x); y = Some(y); px = None; py = None; ``fixed``=None; weight = None}
            let nodes = force.nodes()
            let links = force.links() 
            nodes?push(node) |> ignore
        
            // add links to any nearby nodes
            nodes?forEach( fun target -> 
                let x = 
                    match target.x, node.x with
                    | Some(t), Some(n) -> t-n
                    | _,_ -> 0.0

                let y = 
                    match target.y, node.y with 
                    | Some( t), Some(n) -> t-n
                    | _,_ -> 0.0

                if Math.Sqrt(x * x + y * y) < 30.0 then
                    links?push({source= node; target= target}) |> ignore
                
            ) |> ignore

            restart( force )
        )
    )
    //react-d3-library pattern has use return the dom element
    graph

//Because we want to manipulate force programatically, it is global state we pass in as props
type ForceDirectedGraphProps =
    abstract force : D3.Layout.Force<Link,Node>

type ForceDirectedGraphState = 
    {
        //state we truly don't touch - d3 dom is isolated
        d3 : Browser.HTMLDivElement
        //we want the d3 dom to render only once on mount
        mounted : bool
        //on mount, we supply force to d3 through state
        force : D3.Layout.Force<Link,Node>
    }

type ForceDirectedGraph(props, ctx) as this =
    inherit React.Component<ForceDirectedGraphProps, ForceDirectedGraphState>(props, ctx)
    do this.state <- { d3 = null; mounted = false; force = props.force}

    //the react-d3-library pattern calls for d3 to be initialized when component mounts
    member this.componentDidMount() =
        this.setState {this.state with mounted=true; d3 = createForceGraph(this.state.force)  }
        
    //re-rendering will destroy any mutation we've done in d3 since mounting
    member this.shouldComponentUpdate () = not <| this.state.mounted

    member this.render() =
        R.div [] 
            [
                //the react-d3-library pattern calls for d3 element to be passed in as prop called data
                React.createElement( Component, createObj[ "data" ==> this.state.d3 ], [||] ) 
            ]

let view (model : Model) dispatch =
    R.div [ ]
        [
            RT.button [ Label "Add Noise to Data"; Raised true;
                OnClick( fun _ -> AddNoise |> dispatch)] []
            RT.button [ Label "AddNode"; Raised true;
                OnClick( fun _ -> AddNode |> dispatch)] []

            R.com<ForceDirectedGraph,_,_> 
                { new ForceDirectedGraphProps with
                    member __.force = model.force
                } []
         ]

// UPDATE
let inline rand() = JS.Math.random()

let update (msg:Msg) (model:Model)  =
  match msg with
  | AddNoise -> 
    //it is important to mutate existing nodes. if we create new ones, e.g. with Array.map, existing links will break
    model.force.nodes()?forEach( fun node -> 
        //even though node x/y is not mutable, we trick the compiler using the dynamic operator
        node?x <- node.x |> Option.map( fun v -> v* 10.0*rand() )
        node?y <- node.y |> Option.map( fun v -> v* 10.0*rand() )
        ) |> ignore
    restart( model.force ) |> ignore
    model
  | AddNode -> 
    //again, it is important to mutate nodes and not replace them
    let x,y = 10.0*rand(), 10.0*rand()
    let node = {index = None; x = Some(x); y = Some(y); px = None; py = None; ``fixed``=None; weight = None}
    model.force.nodes()?push(node) |> ignore
    restart( model.force ) |> ignore
    model

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
