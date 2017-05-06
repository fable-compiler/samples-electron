namespace Test

[<AutoOpen>]
module Util =
    open Fable.Core.Testing

    // Convenience method
    let equal (expected: 'T) (actual: 'T) =
        Assert.AreEqual(true, (expected = actual))
