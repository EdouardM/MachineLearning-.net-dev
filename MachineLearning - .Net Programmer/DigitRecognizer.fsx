#load "DigitRecognizer.fs"

open MachineLearning.DigitRecognizer.Utils
open MachineLearning.DigitRecognizer.Model

let trainingfilename = "trainingsample.csv"
let trainingfilepath =  __SOURCE_DIRECTORY__ + @"..\..\data\" + trainingfilename

let trainingdata = reader trainingfilepath
let test = trainingdata.[100].Label


///Takes array of pixels and returns predicted label
let manathanClassifier = train manathanDistance trainingdata
let euclideanClassifier = train euclideanDistance trainingdata


let validationfilename = "validationsample.csv"
let validationfilepath = __SOURCE_DIRECTORY__ + @"..\..\data\" + validationfilename

let validationData = reader validationfilepath

printfn "Manathan:"
evaluate manathanClassifier validationData

printfn "Euclidean:"
evaluate euclideanClassifier validationData
