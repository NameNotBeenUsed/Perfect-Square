#time "on"
#r "nuget: Akka.FSharp"
#r "nuget: Akka.TestKit"

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

let system = ActorSystem.Create("PerfectSquare")

type MyMessage = {B:int; K:int}

type WorkerActor(name) =
    inherit Actor()

    override x.OnReceive message = 
        match box message with
        | :? MyMessage as msg ->
            let b = msg.B
            let sum = msg.K*b*b + msg.K*(msg.K-1)*b + (msg.K-1)*msg.K*(2*msg.K-1)/6
            let squareRoot =int(Math.Sqrt(float(sum)))
            if squareRoot*squareRoot = sum then 
                
                printfn "%i\n" b
            else 
                //printfn "not %i" msg.B
                ()


        | _ -> failwith "unknown message"

let workerActors = 
    [1 .. 100]
    |> List.map(fun id -> let properties = [| string(id) :> obj |]
                          system.ActorOf(Props(typedefof<WorkerActor>, properties)))

let rand = Random(1234)

let args : string array = fsi.CommandLineArgs
let msg1 = args.[1]|> int
let msg2 = args.[2]|> int



for i = 1 to msg1 do
    let myMessage = {MyMessage.B = i; MyMessage.K = msg2}
    workerActors.Item(rand.Next() % 100) <! myMessage


system.Terminate()
