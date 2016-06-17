
#load "NaiveBayes.fs"

open System.IO
open System.Text.RegularExpressions
open MachineLearning.NaiveBayes.Classifier

///Reading Our Dataset

let trainingfilename = "SMSSpamCollection"
let trainingfilepath =  __SOURCE_DIRECTORY__ + @"..\..\data\" + trainingfilename

type DocType =
    | Spam
    | Ham

let parseDocType = function
    | "ham" -> Ham
    | "spam" -> Spam
    | _ -> failwith "Unknown label"

let parseLine (line: string) =
    let split = line.Split [| '\t' |]
    let label = split.[0] |> parseDocType
    let content = split.[1]
    (label, content)

let dataset =
    File.ReadAllLines trainingfilepath
    |> Array.map parseLine

///Deciding on a Single Word:

dataset |> Array.length
// => 5574

let spamWithFree =
    dataset
    |> Array.filter(fun (doctype, _) -> doctype = Spam)
    |> Array.filter(fun (_, msg) -> msg.Contains("FREE"))
    |> Array.length
// => 112

let hamWithFree =
    dataset
    |> Array.filter(fun (doctype, _) -> doctype = Ham)
    |> Array.filter(fun (_, msg) -> msg.Contains("FREE"))
    |> Array.length
// => 1

let primitiveClassifier (sms: string) =
    if sms.Contains("FREE") then Spam else Ham

///Implementing First tokenizer
let matchWords = new Regex(@"\w+")

let tokens (txt: string) =
    txt.ToLowerInvariant()
    |> matchWords.Matches
    |> Seq.cast<Match>
    |> Seq.map(fun m -> m.Value)
    |> Set.ofSeq

//Test
tokens "42 is the Answer to the question"
