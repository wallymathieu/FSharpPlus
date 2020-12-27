module General.Lensing


open Testing
open FSharpPlus
open FSharpPlus.Data
open System
#if !FABLE_COMPILER || FABLE_COMPILER_3
open FSharpPlus.Lens
type Person = { Name: string; DateOfBirth: DateTime }
module Person =
    let inline _name f { Name = a; DateOfBirth = b } = f a <&> fun a' -> { Name = a'; DateOfBirth = b }
type Book = { Title: string; Author: Person }
module Book =
    let inline _author f { Author = a; Title = b } = f a <&> fun a' -> { Author = a'; Title = b }
    let inline _authorName b = _author << Person._name <| b
#endif
let lensing = testList "Lensing" [
#if !FABLE_COMPILER || FABLE_COMPILER_3
    testCase "Lens" (fun () ->
               // equal (view _1 (1, '2')) 1
               // equal (view _2 ('1', 2)) 2
               let rayuela =
                    { Book.Title = "Rayuela"
                      Author = { Person.Name = "Julio Cortázar"
                                 DateOfBirth = DateTime (1914, 8, 26) } }
               equal "Julio Cortázar" (view Book._authorName rayuela)
               
               // equal None (preview _Ok (Error 1))
               // equal (Some 1) (preview _Ok (Ok 1))
               // equal (Some 1) (preview _Error (Error 1))
               // equal None (preview _Error (Ok 1))
               )
    #if !FABLE_COMPILER
    testCase "prism1" (fun () -> equal true (Option.isNone (preview _Some None)))
    testCase "prism4" (fun () -> equal true (Option.isNone (preview _None (Some 1))))
    #else
    (*
Const.run "Julio Cortázar"
Const.run First None
First.run First None
<null>
Const.run First None
First.run First None
<null>
Const.run First (Some 1)
First.run First (Some 1)
Const.run First (Some ())
First.run First (Some ())

Const.run []
First.run []
Const.run []
First.run []
Const.run First 1
First.run First 1
Const.run First undefined
First.run First undefined
    *)
    testCase "prism1" (fun () ->
        let v : int option = preview _Some None
        printfn "%A" v
        equal true (Option.isNone v))
    testCase "prism4" (fun () ->
        let v: unit option = (preview _None (Some 1))
        printfn "%A" v
        equal true (Option.isNone v))
    #endif
    testCase "prism2" (fun () -> equal (Some 1) (preview _Some (Some 1)))
    testCase "prism3" (fun () -> equal (Some ()) (preview _None None))
    #if !FABLE_COMPILER
    testCase "prism4" (fun () -> equal true (Option.isNone (preview _None (Some 1))))
    #endif
    testList "Result_Ok" [
        testCase "Ok" (fun () -> equal true (Option.isSome (preview _Ok (Ok 1))))
        testCase "Error" (fun () -> equal true (Option.isNone (preview _Ok (Error 1))))
    ]
    testList "Result_Error" [
        testCase "Ok" (fun () -> equal true (Option.isNone (preview _Error (Ok 1))))
        testCase "Error" (fun () -> equal true (Option.isSome (preview _Error (Error 1))))
    ]

#endif
]