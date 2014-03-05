open mForex.API
open mForex.API.FSharp
open System.Threading
open System

type Agent<'a> = MailboxProcessor<'a>

type Message =
    | Start    

type Api() =    
    let client = APIClient(false)
    
    let connect() = async {
            do! client.Connect()
            let! loggedIn = client.Login(77000005, "Haslo123")
            return loggedIn
        }

    let agent = Agent.Start(fun a -> async {
            while true do
                let! msg = a.Receive()
                
                match msg with
                | Start -> let! r  = connect()
                           printfn "Logged in: %b" r.LoggedIn
        })

    
    do agent.Error.Add(fun e -> printfn "Fatal error occured: %O" e)

    member this.Connect() =
        agent.Post(Start) 

    member this.Stream symbol =
        client.Ticks.Add(fun p -> 
            p |> Array.toList 
              |> List.filter (fun x -> x.Symbol = symbol)
              |> List.iter (fun x -> printfn "%A %f/%f %A" x.Symbol x.Bid x.Ask x.Time))
        

[<EntryPoint>]
let main argv = 
    let api = Api()
    
    api.Connect()    
    api.Stream "EURUSD"

    Console.ReadLine() |> ignore
    0