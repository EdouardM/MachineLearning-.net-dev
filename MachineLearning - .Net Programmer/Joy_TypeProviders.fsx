///R Type Provider
#I "C:\Users\Edouard\Documents\24. Machine Learning F#\MachineLearning - .Net Programmer\MachineLearning - .Net Programmer"
#load @"packages/FsLab/FsLab.fsx"

open RProvider
open FSharp.Charting
open System.Drawing
open FSharp.Data

type Questions = JsonProvider<"""https://api.stackexchange.com/2.2/questions?site=stackoverflow""">

let csQuestions =  """https://api.stackexchange.com/2.2/questions?site=stackoverflow&tagged=C%23"""

let q = Questions.Load(csQuestions).Items |> Seq.iter (fun q -> printfn "%A" q)


///Building Minimal DSL for queries

//Base query:
let questionQuery = """https://api.stackexchange.com/2.2/questions?site=stackoverflow"""

let tagged tags query =
  //join tags in a ; separated query
  let joinedTags = tags |> String.concat ";"
  sprintf "%s&tagged=%s" query joinedTags

let page p query = sprintf "%s&page=%i" query p

let pageSize s query = sprintf "%s&pagesize=%i" query s

let extractQuestions (query:string) = Questions.Load(query).Items

///UJsing DSL
let ``C#`` = "C%23"
let ``F#`` = "F%23"

let sample tags =
  questionQuery
  |> tagged tags
  |> pageSize 100
  |> extractQuestions

let csSample = sample [``C#``]
let fsSample = sample [``F#``]

///Comparing tags by language
let analyzeTags (qs: Questions.Item seq) =
  qs
  |> Seq.collect(fun question -> question.Tags)
  |> Seq.countBy id
  |> Seq.filter(fun (tag, count) -> count > 2)
  |> Seq.sortByDescending(fun (tag, count) -> count)
  |> Seq.iter(fun (tag, count) -> printfn "%s, %i" tag count)

analyzeTags csSample
analyzeTags fsSample

///All the Data in the World
let wb = WorldBankData.GetDataContext()
wb.Countries.Japan.CapitalCity // => Tokyo

//Retrieve population in all countries in the world between 2000 and 2010
let countries = wb.Countries
let pop2000 = [for c in countries -> c.Indicators.``Population, total``.[2000] ]
let pop2010 = [for c in countries -> c.Indicators.``Population, total``.[2010] ]


//Retrieve an F# list of countries surfaces
let surfaces = [for c in countries -> c.Indicators.``Surface area (sq. km)``.[2010] ]

//Produce summary statistics
let ch = Chart.Histogram surfaces
ch.ShowChart()
