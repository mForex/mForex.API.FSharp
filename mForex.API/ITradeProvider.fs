namespace mForex.API.FSharp

open mForex.API
open mForex.API.Packets
open System

/// Provides interface for trade execution
type ITradeProvider = 
    /// Sends trade request to the server.
    abstract OpenOrder: symbol: string * tradeCommand: TradeCommand * price: float * stopLoss: float * takeProfit: float * volume: float -> Async<TradeTransResponsePacket>
    /// Sends trade request to the server.
    abstract OpenOrder: symbol: string * tradeCommand: TradeCommand * price: float * stopLoss: float * takeProfit: float * volume: float * comment: string -> Async<TradeTransResponsePacket>
    /// Sends order modification request to the server.
    abstract ModifyOrder: orderId: int * newPrice: float * newStopLoss: float * newTakeProfit: float * newVolume: float * newExpiration: DateTime -> Async<TradeTransResponsePacket>
    /// Sends delete pending order request to the server.
    abstract DeleteOrder: orderId: int -> Async<TradeTransResponsePacket>
    /// Sends close market order request to the server.
    abstract CloseOrder: orderId: int * volume: float -> Async<TradeTransResponsePacket>

