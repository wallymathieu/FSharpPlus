(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.

(**
Introduktion till FSharpPlus
============================

 - Ladda ner binärer från [Nuget](https://www.nuget.org/packages/FSharpPlus/).

 - Öppna en F# skriptfil eller interaktiv F# prompt, referera till biblioteket och öppna namnrymd

*)

#r @"../../../src/FSharpPlus/bin/Release/net45/FSharpPlus.dll"
open FSharpPlus

(**
Ignorera varningar om F# metadata om några.


Vi kan nu starta med en snabb tur av de features som finns i F#+.

### Generiska funktioner 

De finns automatiskt tillgängliga när du har öppnat FSharpPlus namnrymd

här är ett exempel med <code>map</code> ([fmap](https://wiki.haskell.org/Functor) för Haskelliter, [Select](http://www.dotnetperls.com/select) för Csharpare):

*)

map string [|2;3;4;5|]
// val it : string [] = [|"2"; "3"; "4"; "5"|]

map ((+) 9) (Some 3)
// val it : int option = Some 12

open FSharpPlus.Data

map string (NonEmptyList.create 2 [3;4;5])
// val it : NonEmptyList<string> = {Head = "2"; Tail = ["3"; "4"; "5"];}

(**
Dessa funktioer finns också tillgängliga för dina egna typer så länge de innehåller metoder med förväntade signaturer
*)


type Tree<'t> =
    | Tree of 't * Tree<'t> * Tree<'t>
    | Leaf of 't
    static member Map (x:Tree<'a>, f) = 
        let rec loop f = function
            | Leaf x -> Leaf (f x)
            | Tree (x, t1, t2) -> Tree (f x, loop f t1, loop f t2)
        loop f x

map ((*) 10) (Tree(6, Tree(2, Leaf 1, Leaf 3), Leaf 9))
// val it : Tree<int> = Tree (60,Tree (20,Leaf 10,Leaf 30),Leaf 90)

(**
Generiska funktioner ser kanske exotiska ut med F#-ögon, som enbart sparar några få tangenttryckningar (<code>map</code> istället för <code>List.map</code> eller <code>Array.map</code>). Det de däremot gör är att tillåta dig nå en högre abstraktionsnivå genom att använda ad-hoc polymorphism.

Ännu mer intressant är användningen av operatorer. Du kan inte prefixa dem med den modul de tillhör och förvänta dig att samma operator den fungerar för flera typer i samma utrymme. Ett exempel är att många F#-bibliotek definierar bind operator <code>(>>=)</code> men att dessa inte går att använda samtidigt utan att behöva lägga till ett prefix t.ex. <code>State.(>>=)</code> och <code>Reader.(>>=)</code> vilket gör att nyttan av operatorn försvinner.

Här har du en färdig att använda generisk bind operator: ``>>=``
*)

let x = ["hello";" ";"world"] >>= (fun x -> Seq.toList x)
// val x : char list = ['h'; 'e'; 'l'; 'l'; 'o'; ' '; 'w'; 'o'; 'r'; 'l'; 'd']


let tryParseInt : string -> int option = tryParse
let tryDivide x n = if n = 0 then None else Some (x / n)

let y = Some "20" >>= tryParseInt >>= tryDivide 100
// val y : int option = Some 5

(**
Du har också Kleislikompositionen familiärt kallad fiskoperatorn:  ``>=>``

Denna operator är populär inom F# tack vare [Railway Oriented Programming](https://www.google.ch/#q=railway+oriented+programming).
*)

let parseAndDivide100By = tryParseInt >=> tryDivide 100

let parsedAndDivide100By20 = parseAndDivide100By "20"   // Some 5
let parsedAndDivide100By0' = parseAndDivide100By "zero" // None
let parsedAndDivide100By0  = parseAndDivide100By "0"    // None

let parseElement n = List.tryItem n >=> tryParseInt
let parsedElement  = parseElement 2 ["0"; "1";"2"]

(**
Glöm inte bort att ovan använda operatorer är generiska, varför vi kan ändra på typen av våra funktioner och få annorlunda funktionalitet gratis:
*)

(*** hide ***)
module E2 =

let tryParseInt x : Choice<int, string> = 
    match tryParse x with 
    | Some x -> Choice1Of2 x
    | None   -> Choice2Of2 ("Failed to parse " + x)
        

let tryDivide x n = 
    if n = 0 then Choice2Of2 "Can't divide by zero"
    else Choice1Of2 (x / n)

(**
Testkoden är oförändrad, men vi får mer intressant funktionalitet
*)

let parseAndDivide100By = tryParseInt >=> tryDivide 100

let parsedAndDivide100By20 = parseAndDivide100By "20"   // Choice1Of2 5
let parsedAndDivide100By0' = parseAndDivide100By "zero" // Choice2Of2 "Failed to parse zero"
let parsedAndDivide100By0  = parseAndDivide100By "0"    // Choice2Of2 "Can't divide by zero"


(**

När vi arbetar med kombinatorer är den generiska applikativa functoroperatorn (space invaders) väldigt praktisk: ``<*>``
*)

let sumAllOptions = Some (+) <*> Some 2 <*> Some 10     // val sumAllOptions : int option = Some 12

let sumAllElemets = [(+)] <*> [10; 100] <*> [1; 2; 3]   // int list = [11; 12; 13; 101; 102; 103]

(**

För mer detaljer och funktionalitet, se [generiska operatorer och funktioner](generic-doc.html)

Här är alla [generiska operatorer och funktioner](reference\fsharpplus-operators.html)

Och [här är en kort förklaring](applicative-functors.html) av Functor, Applicativ och Monad-abstraktionerna med kodexempel.



### Lins



Från https://github.com/ekmett/lens/wiki/Examples


Först, öppna F#+ Lens
*)

open FSharpPlus.Lens

(** Nu kan du läsa från linser (``_2`` är en lins för den andra komponenten av en tupel) *)

let r1 = ("hello","world")^._2
// val it : string = "world"

(** och du kan skriva till linser. *)
let r2 = setl _2 42 ("hello","world")
// val it : string * int = ("hello", 42)

(**  Att knyta ihop linser för att läsa (eller skriva) går i den ordning som en imperativ programmerare skulle förvänta sig: använder sig av ``(<<)``. *)
let r3 = ("hello",("world","!!!"))^.(_2 << _1)
// val it : string = "world"

let r4 = setl (_2 << _1) 42 ("hello",("world","!!!"))
// val it : string * (int * string) = ("hello", (42, "!!!"))

(**  Du kan också göra en läsfunktion av en ren funktion genom att använda ``to'``. *)
let r5 = "hello"^.to' length
// val it : int = 5

(**  Du kan enkelt komponera läsfunktioner med linser genom att använda ``(<<)``. *)
let r6 = ("hello",("world","!!!"))^. (_2 << _2 << to' length)
// val it : int = 3

(**  Som vi såg ovan, kan du skriva till linser och dessa uppdateringar förändrar vilken typ behållaren har. ``(.->)`` är ett infix alias för ``set``. *)
let r7 = _1 .-> "hello" <| ((),"world")
// val it : string * string = ("hello", "world")

(**  Den kan avnändas tillsammans med ``(|>)`` för en bekant von Neumann-stil av tilldelningssyntax: *)
let r8 = ((), "world") |> _1 .-> "hello"
// val it : string * string = ("hello", "world")

(**  Omvänt kan view användas som ett prefix alias för ``(^.)``. *)
let r9 = view _2 (10,20)
// val it : int = 20

(**

För mer detaljer:

Här är en full genomgång av [liner och all annan optik](lens.html)

Ta dig en titt på alla [linsfunktioner](reference\fsharpplus-lens.html)
*)
