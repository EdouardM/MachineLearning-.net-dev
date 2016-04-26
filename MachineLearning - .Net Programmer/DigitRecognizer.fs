namespace MachineLearning.DigitRecognizer

///1-nearest neighbour
module Model =

    type Observation = { Label: string; Pixels: int[] }
    type Distance = int [] -> int [] -> int

    let toObservation (csvData: string) =
        let columns = csvData.Split [| ',' |]
        let label = columns.[0]
        let pixels = columns.[1..] |> Array.map int
        {Label = label; Pixels = pixels }

    let manathanDistance pixels1 pixels2 = 
        Array.zip pixels1 pixels2
        |> Array.map(fun (x,y) -> abs (x - y))
        |> Array.sum

    let euclideanDistance pixels1 pixels2 =
        Array.zip pixels1 pixels2
        |> Array.map(fun (x,y) -> pown (x - y) 2)
        |> Array.sum
        //|> sqrt We drop sqrt for faster calculation
    
    ///Search in the training set the element closest to the image to classify
    ///1-nearest neighbour
    let train (dist:Distance) (trainingSet: Observation []) =
        let classify (pixels: int []) =
            trainingSet
            |> Array.minBy (fun x -> dist x.Pixels pixels )
            |> (fun x -> x.Label)
        classify 

    ///Validate model based on accuracy
    let evaluate classifier data =
        data
        |> Array.averageBy(fun x -> if classifier x.Pixels = x.Label then 1. else 0.)
        |> printfn "Correct: %.3f"
    
module Utils = 
    open System.IO
    open Model

    let reader path =
        let data = File.ReadAllLines path
        data.[1..]
        |> Array.map toObservation 