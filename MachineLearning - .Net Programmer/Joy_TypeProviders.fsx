
//Add path to packages in look up places
#I "..\packages"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open FSharp.Data

type Questions = JsonProvider<"""https://api.stackexchange.com/2.2/questions?site=stackoverflow""">

let csQuestions =  """https://api.stackexchange.com/2.2/questions?site=stackoverflow&tagged=C%23"""

let q = Questions.Load(csQuestions).Items |> Seq.iter (fun q -> printfn "%s" q.Title)
