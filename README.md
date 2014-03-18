# mForex API for F\# 
The goal of mForex API is to provide flexible, asynchronous programming model for .NET based clients connecting to mForex Trade Server. This implementation is a thin layer over C# one, however it's optimized for convenient use from F#. 

We are currently conducting beta tests, so our API is only available on demand for demo accounts only. If you would like to participate, please contact us on <api@mforex.pl>. 

## Quick start
mForex API for F# is available on NuGet

```
Install-Package mForex.API.FSharp
```

### Logging in 
Once you have your account ready, you can log in to our server using following code:

```fsharp 
//Firstly, you have to create an APIClient object, which will be used to communicate with the server.
let client = APIClient(ServerType.Demo)

//Every method returns Async<'T>, so they can be used to create an asynchronous workflow.
//When run, it will connect and login to the server.
let workflow = async {
    do! client.Connect()
    let! result = client.Login(login, password)
}
```

### Requesting for quotes
Once connection has been established, data stream has been setup and is ready to be registered for. ```APIClient``` provides events which can be subscribed to. For example, to receive and process every incoming tick one could:

```fsharp 
client.Ticks.Add(fun p -> 
        p |> Seq.ofArray
          |> Seq.filter (fun x -> x.Symbol = symbol)
          |> Seq.iter ( fun x -> [...] )
```

### Trade requests
```APIClient``` offers an easy way to manage your orders through an ```ITradeProvider``` interface. For example, sample code which closes all opened positions on EURUSD instrument could look like this:

```fsharp 
let trades = (client.RequestOpenTrades() |> Async.RunSynchronously).Trades
let workflow = trades 
               |> Seq.ofArray
               |> Seq.map(fun t -> client.Trade.CloseOrder(t.Order, t.Volume))
               |> Async.Parallel
```
Note, that you can schedule closing all orders without waiting for first response, which could significantly boost performance in your scenario.

## Asynchronous model
The protocol used to communicate with mForex Trade Server is fundamentally asynchronous. It is implemented in F#'s idiomatic way, using ```Async<'T>``` workflows.

## Problems?
If you encounter any bugs or would like to propose any new features or enhancements, please visit the [issue tracker](https://github.com/mForex/mForex.API.FSharp/issues) and report the issue. 

## Copyright and License
Copyright 2013 Dom Maklerski mBanku S.A.
Licensed under the [MIT License](https://raw.github.com/mForex/mForex.API.FSharp/master/LICENSE).
