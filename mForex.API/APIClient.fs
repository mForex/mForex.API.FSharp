namespace mForex.API.FSharp

open System
open System.Threading.Tasks

open mForex.API

type CApiClient = mForex.API.APIClient
type CApiConnection = mForex.API.APIConnection

type APIClient(connectToReal: bool) = 
    
    let connection = CApiConnection(ServerType.Demo)
    let client = new CApiClient(connection)

    let ConvTask (t: Task) = t.ContinueWith(
                                Func<Task, unit>(
                                    fun t -> 
                                        let x = if t.Exception <> null then raise t.Exception; (); else (); 
                                        x))
    let AwaitTask = ConvTask >> Async.AwaitTask

    let ticks = Event<_>()
    let margin = Event<_>()
    let disconnected = Event<_>()
    let tradeUpdate = Event<_>()

    do client.add_Ticks (fun t -> ticks.Trigger(t))
    do client.add_Margin (fun t -> margin.Trigger(t))
    do client.add_Disconnected (fun t -> disconnected.Trigger(t))
    do client.add_TradeUpdate (fun t -> tradeUpdate.Trigger(t))
  
    /// Provides interface for trade execution
    member this.Trade = this :> mForex.API.FSharp.ITradeProvider

    /// Occurs when new ticks are received from the server.
    member this.Ticks = ticks.Publish
    
    /// Occurs when margin level is updated. 
    /// However, this cannot occur more often than once per second.
    /// This event is fired only when packet with id 514 is received. 
    /// If margin level was requested explicitly, this even will not be fired.
    member this.Margin = margin.Publish
    
    /// Occurs when client is disconnected from server with exception.
    member this.Disconnected = disconnected.Publish
    
    /// Occurs when trade is updated.
    /// Trade is marked as updated if it has been opened, closed or modified.
    member this.TradeUpdate = tradeUpdate.Publish
    
    /// Initialises a connection to the server. 
    /// Returned task is signaled when connection is established.
    /// Connect method can be called only once. If client has been disconnected, 
    /// new instance must be created to reconnect.
    member this.Connect() = 
        client.Connect()
        |> AwaitTask
    
    /// Closes the client connection and releases any resources. 
    member this.Disconnect() = 
        client.Disconnect()

    /// Sends login request to the server.
    /// This is the only legal packet which can be sent after connection is established.
    member this.Login(login, password) = 
        client.Login(login, password)
        |> Async.AwaitTask
    
    /// Sends request to the server for candles from a given period. 
    member this.RequestCandles(symbol, period, fromTime, toTime) = 
        client.RequestCandles(symbol, period, fromTime, toTime)
        |> Async.AwaitTask

    /// Sends request to the server for settings for all available instruments.
    member this.RequestInstrumentSettings() = 
        client.RequestInstrumentSettings()
        |> Async.AwaitTask

     /// Sends request to the server for trading hours for particular instrument.
    member this.RequestSessions(symbol) = 
        client.RequestSessions(symbol)
        |> Async.AwaitTask
    
    /// Sends request to the server for current margin level.
    member this.RequestMarginLevel() = 
        client.RequestMarginLevel()
        |> Async.AwaitTask

    /// Sends request to the server for closed transactions from a given period. 
    member this.RequestTradeHistory(fromTime, toTime) = 
        client.RequestTradesHistory(fromTime, toTime)
        |> Async.AwaitTask

    /// Send request to the server for open transactions.
    member this.RequestOpenTrades() = 
        client.RequestOpenTrades()
        |> Async.AwaitTask

    interface mForex.API.FSharp.ITradeProvider with
        
        member this.OpenOrder(symbol, tradeCommand, price, stopLoss, takeProfit, volume) = 
            client.Trade.OpenOrder(symbol, tradeCommand, price, stopLoss, takeProfit, volume)
            |> Async.AwaitTask 

        member this.OpenOrder(symbol, tradeCommand, price, stopLoss, takeProfit, volume, comment) = 
            client.Trade.OpenOrder(symbol, tradeCommand, price, stopLoss, takeProfit, volume, comment)
            |> Async.AwaitTask 

        member this.ModifyOrder(orderId, newPrice, newStopLoss, newTakeProfit, newVolume, newExpiration) = 
            client.Trade.ModifyOrder(orderId, newPrice, newStopLoss, newTakeProfit, newVolume, newExpiration)
            |> Async.AwaitTask

        member this.DeleteOrder(orderId) = 
            client.Trade.DeleteOrder(orderId)
            |> Async.AwaitTask

        member this.CloseOrder(orderId, volume) = 
            client.Trade.CloseOrder(orderId, volume)
            |> Async.AwaitTask

