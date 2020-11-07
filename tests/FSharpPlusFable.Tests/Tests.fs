module Tests

open ExtensionsTests
open Testing


let AllTests =  testList "AllTests" [ExtensionsTest]

#if FABLE_COMPILER2

open Fable.Core
open Fable.Core.JsInterop

flattenTest AllTests

#else

open Expecto
open Expecto.TestResults

[<EntryPoint>]
    let main args =
        printfn "Result: %i" (runTestsWithArgs defaultConfig args AllTests)
        0


#endif