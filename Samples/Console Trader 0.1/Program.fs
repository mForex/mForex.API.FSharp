open mForex.API
open mForex.API.FSharp
open System.Threading
open System

type Agent<'a> = MailboxProcessor<'a>
type Message = | Start of Credentials: int * string

type Api() =        
    
    let client = APIClient(false)
    
    let connect login password = async {
            do! client.Connect()
            let! loggedIn = client.Login(login, password)
            return loggedIn
        }

    let agent = Agent.Start(fun a -> async {
            while true do
                let! msg = a.Receive()
                
                match msg with
                | Start (login,password)  -> 
                    let! r  = (connect login password)
                    printfn "Logged in: %b" r.LoggedIn
        })

    
    do agent.Error.Add(fun e -> printfn "Fatal error occured: %O" e)

    member this.Connect(login, password) =
        agent.Post(Start (login, password)) 

    member this.Stream symbol =
        client.Ticks.Add(fun p -> 
            p |> Array.toList 
              |> List.filter (fun x -> x.Symbol = symbol)
              |> List.iter (fun x -> printfn "%A %.5f/%.5f %A" x.Symbol x.Bid x.Ask x.Time))
        

[<EntryPoint>]
let main argv =     
    let login = 0               // Enter your login here    
    let password = "password"   // Enter your password here
    
    let api = Api()
    
    api.Connect(login, password)    
    api.Stream "EURUSD"

    Console.ReadKey() |> ignore
    0