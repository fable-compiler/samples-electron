namespace Test

open Fable.Core.Testing

[<TestFixture>]
module Message =
    // This must be before everything else
    Fable.Import.Node.require.Invoke("babel-polyfill") |> ignore

    [<Test>]
    let ``message should be correct`` () =
        App.Message.message |> equal "Hello world!"
