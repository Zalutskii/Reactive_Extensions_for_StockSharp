using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using Ecng.Configuration;
using StockSharp.Algo;
using StockSharp.Algo.Candles;
using StockSharp.Algo.Strategies;
using StockSharp.BusinessEntities;
using StockSharp.Logging;
using StockSharp.Messages;

namespace Reactive.StockSharp
{
    #region Args
    public class CandleSeriesAndCandleArg
    {
        public CandleSeries Series { get; set; }
        public Candle Candle { get; set; }
    }
    public class ExeptionAndEnumerablePortfoliosArg
    {
        public Exception Exception { get; set; }
        public IEnumerable<Portfolio> Portfolios { get; set; }
    }
    public class ExeptionAndEnumerableSecuritiesArg
    {
        public Exception Exception { get; set; }
        public IEnumerable<Security> Securities { get; set; }
    }
    public class SecurityAndMarketDataMessageAndExceptionArg
    {
        public Security Security { get; set; }
        public MarketDataMessage MarketDataMessage { get; set; }
        public Exception Exception { get; set; }
    }
    public class SecurityAndMarketDataMessageArg
    {
        public Security Security { get; set; }
        public MarketDataMessage MarketDataMessage { get; set; }
    }
    public class LongAndExceptionArg
    {
        public long Long { get; set; }
        public Exception Exception { get; set; }

    }
    public class ExchangeBoardAndSessionStatesnArg
    {
        public ExchangeBoard ExchangeBoard { get; set; }
        public SessionStates SessionStates { get; set; }

    }
    public class ValuesChangedArg
    {
        public Security Security { get; set; }
        public IEnumerable<KeyValuePair<Level1Fields, object>> Level1Fields { get; set; }
        public DateTimeOffset DateTimeOffset1 { get; set; }
        public DateTimeOffset DateTimeOffset2 { get; set; }
    }
    public class OrderAndOrderArg
    {
        public Order Order1 { get; set; }
        public Order Order2 { get; set; }
    }
    #endregion  
    public static class RxMarketRuleHelper
    {
        #region Connector
        /// <summary>
        /// A new value for processing occurrence event.
        /// </summary>
        public static IObservable<CandleSeriesAndCandleArg> RxCandleSeriesProcessing(this Connector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<CandleSeriesAndCandleArg>(observer =>
            {
                void OnNext(CandleSeries s, Candle c)
                {
                    observer.OnNext(new CandleSeriesAndCandleArg() {Series = s, Candle = c});
                }

                void OnCompleted(CandleSeries series)
                {
                    observer.OnCompleted();
                }

                connector.CandleSeriesProcessing += OnNext;
                connector.CandleSeriesStopped += (_) => observer.OnCompleted();
                return () =>
                {
                    connector.CandleSeriesProcessing -= OnNext;
                    connector.CandleSeriesStopped -= OnCompleted;
                };
            });

            return observable;
        }
        /// <summary>
        /// The series processing end event.
        /// </summary>
        public static IObservable<CandleSeries> RxCandleSeriesStopped(this Connector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<CandleSeries>(
                handler => connector.CandleSeriesStopped += handler,
                handler => connector.CandleSeriesStopped -= handler);
        }
        /// <summary>
        /// Connected.
        /// </summary>
        public static IObservable<object> RxConnected(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable(
                handler => connector.Connected += handler,
                handler => connector.Connected -= handler);
        }
        /// <summary>
        /// Connected.
        /// </summary>
        public static IObservable<IMessageAdapter> RxConnectedEx(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IMessageAdapter>(
                handler => connector.ConnectedEx += handler,
                handler => connector.ConnectedEx -= handler,
                handler => connector.DisconnectedEx += handler,
                handler => connector.DisconnectedEx -= handler);
        }
        /// <summary>
        /// Connection error (for example, the connection was aborted by server).
        /// </summary>
        public static IObservable<Exception> RxConnectionError(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Exception>(
                handler => connector.ConnectionError += handler,
                handler => connector.ConnectionError -= handler);
        }
        /// <summary>
        /// Disconnected.
        /// </summary>
        public static IObservable<object> RxDisconnected(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable(
                handler => connector.Disconnected += handler,
                handler => connector.Disconnected -= handler);
        }
        /// <summary>
        /// Disconnected.
        /// </summary>
        public static IObservable<IMessageAdapter> RxDisconnectedEx(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IMessageAdapter>(
                handler => connector.DisconnectedEx += handler,
                handler => connector.DisconnectedEx -= handler);
        }
        /// <summary>
        /// Data process error.
        /// </summary>
        public static IObservable<Exception> RxError(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Exception>(
                handler => connector.Error += handler,
                handler => connector.Error -= handler);
        }
        /// <summary>
        /// Lookup result <see cref="LookupPortfolios"/> received.
        /// </summary>
        public static IObservable<ExeptionAndEnumerablePortfoliosArg> RxLookupPortfoliosResult(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<ExeptionAndEnumerablePortfoliosArg>(observer =>
            {
                void OnNext(Exception exception, IEnumerable<Portfolio> portfolios)
                {
                    observer.OnNext(new ExeptionAndEnumerablePortfoliosArg() { Exception = exception, Portfolios = portfolios });
                }
                connector.LookupPortfoliosResult += OnNext;

                return () =>
                {
                    connector.LookupPortfoliosResult -= OnNext;
                };
            });

            return observable;
        }
        /// <summary>
        /// Lookup result <see cref="LookupSecurities(Security)"/> received.
        /// </summary>
        public static IObservable<ExeptionAndEnumerableSecuritiesArg> RxLookupSecuritiesResult(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<ExeptionAndEnumerableSecuritiesArg>(observer =>
            {
                void OnNext(Exception exception, IEnumerable<Security> securities)
                {
                    observer.OnNext(new ExeptionAndEnumerableSecuritiesArg() { Exception = exception, Securities = securities });
                }

                connector.LookupSecuritiesResult += OnNext;

                return () =>
                {
                    connector.LookupSecuritiesResult += OnNext;
                };
            });

            return observable;
        }
        /// <summary>
        /// Error subscription market-data.
        /// </summary>
        public static IObservable<SecurityAndMarketDataMessageAndExceptionArg> RxMarketDataSubscriptionFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageAndExceptionArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage, Exception exception)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageAndExceptionArg() { Exception = exception, Security = security, MarketDataMessage = marketDataMessage });
                }

                connector.MarketDataSubscriptionFailed += OnNext;

                return () =>
                {
                    connector.MarketDataSubscriptionFailed -= OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Successful subscription market-data.
        /// </summary>
        public static IObservable<SecurityAndMarketDataMessageArg> RxMarketDataSubscriptionSucceeded(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageArg() { Security = security, MarketDataMessage = marketDataMessage });
                }
                connector.MarketDataSubscriptionSucceeded += OnNext;

                return () =>
                {
                    connector.MarketDataSubscriptionSucceeded += OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Error unsubscription market-data.
        /// </summary>
        public static IObservable<SecurityAndMarketDataMessageAndExceptionArg> RxMarketDataUnSubscriptionFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageAndExceptionArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage, Exception exception)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageAndExceptionArg() { Exception = exception, Security = security, MarketDataMessage = marketDataMessage });
                }
                connector.MarketDataUnSubscriptionFailed += OnNext;

                return () =>
                {
                    connector.MarketDataUnSubscriptionFailed -= OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Error unsubscription market-data.
        /// </summary>
        public static IObservable<SecurityAndMarketDataMessageArg> RxMarketDataUnSubscriptionSucceeded(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageArg() { Security = security, MarketDataMessage = marketDataMessage });
                }
                connector.MarketDataUnSubscriptionSucceeded += OnNext;

                return () =>
                {
                    connector.MarketDataUnSubscriptionSucceeded += OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Order book changed.
        /// </summary>
        public static IObservable<MarketDepth> RxMarketDepthChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<MarketDepth>(
                handler => connector.MarketDepthChanged += handler,
                handler => connector.MarketDepthChanged -= handler);
        }
        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<IEnumerable<MarketDepth>> RxMarketDepthsChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<MarketDepth>>(
                handler => connector.MarketDepthsChanged += handler,
                handler => connector.MarketDepthsChanged -= handler);
        }
        /// <summary>
        /// Server time changed <see cref="IConnector.ExchangeBoards"/>. It passed the time difference since the last call of the event. The first time the event passes the value <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public static IObservable<TimeSpan> RxMarketTimeChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<TimeSpan>(
                handler => connector.MarketTimeChanged += handler,
                handler => connector.MarketTimeChanged -= handler);
        }
        /// <summary>
        /// Mass order cancellation event.
        /// </summary>
        public static IObservable<long> RxMassOrderCanceled(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<long>(
                handler => connector.MassOrderCanceled += handler,
                handler => connector.MassOrderCanceled -= handler);
        }
        /// <summary>
        /// Mass order cancellation errors event.
        /// </summary>
        public static IObservable<LongAndExceptionArg> RxMassOrderCancelFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<LongAndExceptionArg>(observer =>
            {
                
                void OnNext(long l1, Exception exception)
                {
                    observer.OnNext(new LongAndExceptionArg() { Exception = exception, Long = l1 });
                }
                connector.MassOrderCancelFailed += OnNext;

                return () =>
                {
                    connector.MassOrderCancelFailed -= OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<MarketDepth> RxNewMarketDepth(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<MarketDepth>(
                handler => connector.NewMarketDepth += handler,
                handler => connector.NewMarketDepth -= handler);
        }
        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<IEnumerable<MarketDepth>> RxNewMarketDepths(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<MarketDepth>>(
                handler => connector.NewMarketDepths += handler,
                handler => connector.NewMarketDepths -= handler);
        }
        /// <summary>
        /// Message processed <see cref="Message"/>.
        /// </summary>
        public static IObservable<Message> RxNewMessage(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Message>(
                handler => connector.NewMessage += handler,
                handler => connector.NewMessage -= handler);
        }
        /// <summary>
        /// Own trade received.
        /// </summary>
        public static IObservable<MyTrade> RxNewMyTrade(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<MyTrade>(
                handler => connector.NewMyTrade += handler,
                handler => connector.NewMyTrade -= handler);
        }
        /// <summary>
        /// Own trades received.
        /// </summary>
        public static IObservable<IEnumerable<MyTrade>> RxNewMyTrades(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<MyTrade>>(
                handler => connector.NewMyTrades += handler,
                handler => connector.NewMyTrades -= handler);
        }
        /// <summary>
        /// News received.
        /// </summary>
        public static IObservable<News> RxNewNews(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<News>(
                handler => connector.NewNews += handler,
                handler => connector.NewNews -= handler);
        }
        /// <summary>
        /// Order received.
        /// </summary>
        public static IObservable<Order> RxNewOrder(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Order>(
                handler => connector.NewOrder += handler,
                handler => connector.NewOrder -= handler);
        }
        /// <summary>
        /// Order log received.
        /// </summary>
        public static IObservable<OrderLogItem> RxNewOrderLogItem(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<OrderLogItem>(
                handler => connector.NewOrderLogItem += handler,
                handler => connector.NewOrderLogItem -= handler);
        }
        /// <summary>
        /// Order log received.
        /// </summary>
        public static IObservable<IEnumerable<OrderLogItem>> RxNewOrderLogItems(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<OrderLogItem>>(
                handler => connector.NewOrderLogItems += handler,
                handler => connector.NewOrderLogItems -= handler);
        }
        /// <summary>
        /// Orders received.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxNewOrders(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Order>>(
                handler => connector.NewOrders += handler,
                handler => connector.NewOrders -= handler);
        }
        /// <summary>
        /// Portfolios received.
        /// </summary>
        public static IObservable<IEnumerable<Portfolio>> RxNewPortfolios(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Portfolio>>(
                handler => connector.NewPortfolios += handler,
                handler => connector.NewPortfolios -= handler);
        }
        ///// <summary>
        ///// Position received.
        ///// </summary>
        public static IObservable<Position> RxNewPosition(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Position>(
                handler => connector.NewPosition += handler,
                handler => connector.NewPosition -= handler);
        }
        /// <summary>
        /// Positions received.
        /// </summary>
        public static IObservable<IEnumerable<Position>> RxNewPositions(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Position>>(
                handler => connector.NewPositions += handler,
                handler => connector.NewPositions -= handler);
        }
        /// <summary>
        /// News updated (news body received <see cref="News.Story"/>).
        /// </summary>
        public static IObservable<News> RxNewsChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<News>(
                handler => connector.NewsChanged += handler,
                handler => connector.NewsChanged -= handler);
        }
        /// <summary>
        /// Securities received.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxNewSecurities(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Security>>(
                handler => connector.NewSecurities += handler,
                handler => connector.NewSecurities -= handler);
        }
        /// <summary>
        /// Security received.
        /// </summary>
        public static IObservable<Security> RxNewSecurity(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Security>(
                handler => connector.NewSecurity += handler,
                handler => connector.NewSecurity -= handler);
        }
        /// <summary>
        /// Stop-order received.
        /// </summary>
        public static IObservable<Order> RxNewStopOrder(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Order>(
                handler => connector.NewStopOrder += handler,
                handler => connector.NewStopOrder -= handler);
        }
        /// <summary>
        /// Stop-orders received.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxNewStopOrders(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Order>>(
                handler => connector.NewStopOrders += handler,
                handler => connector.NewStopOrders -= handler);
        }
        /// <summary>
        /// Tick trade received.
        /// </summary>
        public static IObservable<Trade> RxNewTrade(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Trade>(
                handler => connector.NewTrade += handler,
                handler => connector.NewTrade -= handler);
        }
        /// <summary>
        /// Tick trades received.
        /// </summary>
        public static IObservable<IEnumerable<Trade>> RxNewTrades(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Trade>>(
                handler => connector.NewTrades += handler,
                handler => connector.NewTrades -= handler);
        }
        /// <summary>
        /// Order cancellation error event.
        /// </summary>
        public static IObservable<OrderFail> RxOrderCancelFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<OrderFail>(
                handler => connector.OrderCancelFailed += handler,
                handler => connector.OrderCancelFailed -= handler);
        }
        /// <summary>
        /// Order changed (cancelled, matched).
        /// </summary>
        public static IObservable<Order> RxOrderChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Order>(
                handler => connector.OrderChanged += handler,
                handler => connector.OrderChanged -= handler);
        }
        /// <summary>
        /// Order registration error event.
        /// </summary>
        public static IObservable<OrderFail> RxOrderRegisterFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<OrderFail>(
                handler => connector.OrderRegisterFailed += handler,
                handler => connector.OrderRegisterFailed -= handler);
        }
        /// <summary>
        /// Order cancellation errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxOrdersCancelFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<OrderFail>>(
                handler => connector.OrdersCancelFailed += handler,
                handler => connector.OrdersCancelFailed -= handler);
        }
        /// <summary>
        /// Stop orders state change event.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxOrdersChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Order>>(
                handler => connector.OrdersChanged += handler,
                handler => connector.OrdersChanged -= handler);
        }
        /// <summary>
        /// Order registration errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxOrdersRegisterFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<OrderFail>>(
                handler => connector.OrdersRegisterFailed += handler,
                handler => connector.OrdersRegisterFailed -= handler);
        }
        /// <summary>
        /// Failed order status request event.
        /// </summary>
        public static IObservable<LongAndExceptionArg> RxOrderStatusFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<LongAndExceptionArg>(observer =>
            {
                void OnNext(long l1, Exception exception)
                {
                    observer.OnNext(new LongAndExceptionArg() { Exception = exception, Long = l1 });
                }
                connector.OrderStatusFailed += OnNext;

                return () =>
                {
                    connector.OrderStatusFailed -= OnNext;
                };
            });
            return observable;
        }
        ///// <summary>
        ///// Portfolio changed.
        ///// </summary>
        public static IObservable<Portfolio> RxPortfolioChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Portfolio>(
                handler => connector.PortfolioChanged += handler,
                handler => connector.PortfolioChanged -= handler);
        }
        /// <summary>
        /// Portfolios changed.
        /// </summary>
        public static IObservable<IEnumerable<Portfolio>> RxPortfoliosChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Portfolio>>(
                handler => connector.PortfoliosChanged += handler,
                handler => connector.PortfoliosChanged -= handler);
        }
        ///// <summary>
        ///// Position changed.
        ///// </summary>
        public static IObservable<Position> RxPositionChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Position>(
                handler => connector.PositionChanged += handler,
                handler => connector.PositionChanged -= handler);
        }
        /// <summary>
        /// Positions changed.
        /// </summary>
        public static IObservable<IEnumerable<Position>> RxPositionsChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Position>>(
                handler => connector.PositionsChanged += handler,
                handler => connector.PositionsChanged -= handler);
        }
        /// <summary>
        /// Connection restored.
        /// </summary>
        public static IObservable<object> RxRestored(this Connector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable(
                handler => connector.Restored += handler,
                handler => connector.Restored -= handler);
        }
        /// <summary>
        /// Securities changed.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxSecuritiesChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Security>>(
                handler => connector.SecuritiesChanged += handler,
                handler => connector.SecuritiesChanged -= handler);
        }

        /// <summary>
        /// Security changed.
        /// </summary>
        public static IObservable<Security> RxSecurityChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Security>(
                handler => connector.SecurityChanged += handler,
                handler => connector.SecurityChanged -= handler);
        }
        /// <summary>
        /// Session changed.
        /// </summary>
        public static IObservable<ExchangeBoardAndSessionStatesnArg> RxSessionStateChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<ExchangeBoardAndSessionStatesnArg>(observer =>
            {
                void OnNext(ExchangeBoard exchangeBoard, SessionStates sessionStates)
                {
                    observer.OnNext(new ExchangeBoardAndSessionStatesnArg() { ExchangeBoard = exchangeBoard, SessionStates = sessionStates });
                }
                connector.SessionStateChanged += OnNext;

                return () =>
                {
                    connector.SessionStateChanged += OnNext;
                };
            });
            return observable;
        }
        /// <summary>
        /// Stop-order cancellation error event.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderCancelFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<OrderFail>(
                handler => connector.StopOrderCancelFailed += handler,
                handler => connector.StopOrderCancelFailed -= handler);
        }
        /// <summary>
        /// Stop order state change event.
        /// </summary>
        public static IObservable<Order> RxStopOrderChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<Order>(
                handler => connector.StopOrderChanged += handler,
                handler => connector.StopOrderChanged -= handler);
        }
        /// <summary>
        /// Stop-order registration error event.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderRegisterFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<OrderFail>(
                handler => connector.StopOrderRegisterFailed += handler,
                handler => connector.StopOrderRegisterFailed -= handler);
        }
        /// <summary>
        /// Stop-order cancellation errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxStopOrdersCancelFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<OrderFail>>(
                handler => connector.StopOrdersCancelFailed += handler,
                handler => connector.StopOrdersCancelFailed -= handler);
        }
        /// <summary>
        /// Stop orders state change event.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxStopOrdersChanged(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<Order>>(
                handler => connector.StopOrdersChanged += handler,
                handler => connector.StopOrdersChanged -= handler);
        }
        /// <summary>
        /// Stop-order registration errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxStopOrdersRegisterFailed(this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable<IEnumerable<OrderFail>>(
                handler => connector.StopOrdersRegisterFailed += handler,
                handler => connector.StopOrdersRegisterFailed -= handler);
        }
        /// <summary>
        /// Connection timed-out.
        /// </summary>
        public static IObservable<object> RxTimeOut(this Connector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            return GetNewObservable(
                handler => connector.TimeOut += handler,
                handler => connector.TimeOut -= handler);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of candles occurrence, change and end.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <param name="series">Candles series to be traced for candles.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<CandleSeriesAndCandleArg> RxWhenCandles(this Connector connector, CandleSeries series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            return connector.RxCandleSeriesProcessing().Where(arg => arg.Series == series);
        }
        /// <summary>
        ///  NotImplemented
        /// To create a Reactive Extension for the event <see cref="IConnector.MarketTimeChanged"/>, activated after expiration of <paramref name="interval" />.
        /// </summary>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="interval">Interval.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<CandleSeriesAndCandleArg> RxWhenIntervalElapsed(this IConnector connector, TimeSpan interval)
        {
            //TODO: RxWhenIntervalElapsed
            throw new NotImplementedException();
        }
        /// <summary>
        /// To create a Reactive Extension for the event of new trade occurrences.
        /// </summary>
        /// <param name="connector">The connection to be traced for trades occurrences.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<MyTrade> RxWhenNewMyTrade(this IConnector connector)
        {
            return connector.RxNewMyTrade();
        }
        /// <summary>
        /// To create a Reactive Extension for the event of new orders occurrences.
        /// </summary>
        /// <param name="connector">The connection to be traced for orders occurrences.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Order> RxWhenNewOrder(this IConnector connector)
        {
            return connector.RxNewOrder();
        }
        #endregion
        #region Stratagy
        /// <summary>
        /// <see cref="Commission"/> change event.
        /// </summary>
        public static IObservable<object> RxCommissionChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.CommissionChanged += handler,
                handler => strategy.CommissionChanged -= handler);
        }
        /// <summary>
        /// The event of strategy connection change.
        /// </summary>
        public static IObservable<object> RxConnectorChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.ConnectorChanged += handler,
                handler => strategy.ConnectorChanged -= handler);
        }
        /// <summary>
        /// The event of error occurrence in the strategy.
        /// </summary>
        public static IObservable<Exception> RxError(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Exception>(
                handler => strategy.Error += handler,
                handler => strategy.Error -= handler);
        }
        /// <summary>
        /// <see cref="Latency"/> change event.
        /// </summary>
        public static IObservable<object> RxLatencyChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.LatencyChanged += handler,
                handler => strategy.LatencyChanged -= handler);
        }
        /// <summary>
        /// The event of new trade occurrence.
        /// </summary>
        public static IObservable<MyTrade> RxNewMyTrade(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<MyTrade>(
                handler => strategy.NewMyTrade += handler,
                handler => strategy.NewMyTrade -= handler);
        }
        /// <summary>
        /// The event of order cancelling order.
        /// </summary>
        public static IObservable<OrderFail> RxOrderCancelFailed(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<OrderFail>(
                handler => strategy.OrderCancelFailed += handler,
                handler => strategy.OrderCancelFailed -= handler);
        }
        /// <summary>
        /// The event of sending order for cancelling.
        /// </summary>
        public static IObservable<Order> RxOrderCanceling(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.OrderCanceling += handler,
                handler => strategy.OrderCanceling -= handler);
        }
        /// <summary>
        /// The event of order successful registration.
        /// </summary>
        public static IObservable<Order> RxOrderRegistered(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.OrderRegistered += handler,
                handler => strategy.OrderRegistered -= handler);
        }
        /// <summary>
        /// The event of order registration error.
        /// </summary>
        public static IObservable<OrderFail> RxOrderRegisterFailed(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<OrderFail>(
                handler => strategy.OrderRegisterFailed += handler,
                handler => strategy.OrderRegisterFailed -= handler);
        }
        /// <summary>
        /// The event of sending order for registration.
        /// </summary>
        public static IObservable<Order> RxOrderRegistering(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.OrderRegistering += handler,
                handler => strategy.OrderRegistering -= handler);
        }
        /// <summary>
        /// The event of sending order for re-registration.
        /// </summary>
        public static IObservable<OrderAndOrderArg> RxOrderReRegistering(this Strategy strategy)
        {

            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            var observable = Observable.Create<OrderAndOrderArg>(observer =>
            {
                void OnNext(Order order1, Order order2)
                {
                    observer.OnNext(new OrderAndOrderArg() { Order1 = order1, Order2 = order2 });
                }

                strategy.OrderReRegistering += OnNext;

                return () =>
                {
                    strategy.OrderReRegistering -= OnNext;
                };
            });

            return observable;
        }
        /// <summary>
        /// <see cref="Parameters"/> change event.
        /// </summary>
        public static IObservable<object> RxParametersChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.ParametersChanged += handler,
                handler => strategy.ParametersChanged -= handler);
        }
        /// <summary>
        /// <see cref="Strategy.PnL"/> change event.
        /// </summary>
        public static IObservable<object> RxPnLChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.PnLChanged += handler,
                handler => strategy.PnLChanged -= handler);
        }
        /// <summary>
        /// The event of strategy portfolio change.
        /// </summary>
        public static IObservable<object> RxPortfolioChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.PortfolioChanged += handler,
                handler => strategy.PortfolioChanged -= handler);
        }
        /// <summary>
        /// <see cref="Position"/> change event.
        /// </summary>
        public static IObservable<object> RxPositionChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.PositionChanged += handler,
                handler => strategy.PositionChanged -= handler);
        }
        /// <summary>
        /// The event of strategy position change.
        /// </summary>
        public static IObservable<Position> RxPositionChanged2(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Position>(
                handler => strategy.PositionChanged2 += handler,
                handler => strategy.PositionChanged2 -= handler);
        }
        /// <summary>
        /// <see cref="Strategy.ProcessState"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxProcessStateChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Strategy>(
                handler => strategy.ProcessStateChanged += handler,
                handler => strategy.ProcessStateChanged -= handler);
        }
        /// <summary>
        /// NotImplemented
        /// The event of strategy parameters change. NotImplemented
        /// </summary>
        public static IObservable<PropertyChangedEventHandler> RxPropertyChanged(this Strategy strategy)
        {
            //TODO: Strategy.RxPropertyChanged
            throw new NotImplementedException();
        }
        /// <summary>
        /// The event of the strategy re-initialization.
        /// </summary>
        public static IObservable<object> RxReseted(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.Reseted += handler,
                handler => strategy.Reseted -= handler);
        }
        /// <summary>
        /// The event of strategy instrument change.
        /// </summary>
        public static IObservable<object> RxSecurityChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.SecurityChanged += handler,
                handler => strategy.SecurityChanged -= handler);
        }
        /// <summary>
        /// <see cref="Strategy.Slippage"/> change event.
        /// </summary>
        public static IObservable<object> RxSlippageChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable(
                handler => strategy.SlippageChanged += handler,
                handler => strategy.SlippageChanged -= handler);
        }
        /// <summary>
        /// The event of stop-order cancelling order.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderCancelFailed(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<OrderFail>(
                handler => strategy.StopOrderCancelFailed += handler,
                handler => strategy.StopOrderCancelFailed -= handler);
        }
        /// <summary>
        /// The event of sending stop-order for cancelling.
        /// </summary>
        public static IObservable<Order> RxStopOrderCanceling(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.StopOrderCanceling += handler,
                handler => strategy.StopOrderCanceling -= handler);
        }
        /// <summary>
        /// The event of stop-order change.
        /// </summary>
        public static IObservable<Order> RxStopOrderChanged(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.StopOrderChanged += handler,
                handler => strategy.StopOrderChanged -= handler);
        }
        /// <summary>
        /// The event of stop-order successful registration.
        /// </summary>
        public static IObservable<Order> RxStopOrderRegistered(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.StopOrderRegistered += handler,
                handler => strategy.StopOrderRegistered -= handler);
        }
        /// <summary>
        /// The event of stop-order registration error.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderRegisterFailed(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<OrderFail>(
                handler => strategy.StopOrderRegisterFailed += handler,
                handler => strategy.StopOrderRegisterFailed -= handler);
        }
        /// <summary>
        /// The event of sending stop-order for registration.
        /// </summary>
        public static IObservable<Order> RxStopOrderRegistering(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return GetNewObservable<Order>(
                handler => strategy.StopOrderRegistering += handler,
                handler => strategy.StopOrderRegistering -= handler);
        }
        /// <summary>
        /// The event of sending stop-order for re-registration.
        /// </summary>
        public static IObservable<OrderAndOrderArg> RxStopOrderReRegistering(this Strategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            var observable = Observable.Create<OrderAndOrderArg>(observer =>
            {
                void OnNext(Order order1, Order order2)
                {
                    observer.OnNext(new OrderAndOrderArg() { Order1 = order1, Order2 = order2 });
                }

                strategy.StopOrderReRegistering += OnNext;

                return () =>
                {
                    strategy.StopOrderReRegistering -= OnNext;
                };
            });

            return observable;
        }
        #endregion
        #region CandleManager

        /// <summary>
        /// A new value for processing occurrence event.
        /// </summary>
        public static IObservable<CandleSeriesAndCandleArg> RxProcessing(this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null) throw new ArgumentNullException(nameof(candleManager));
            var observable = Observable.Create<CandleSeriesAndCandleArg>(observer =>
            {
                void OnNext(CandleSeries s, Candle c)
                {
                    observer.OnNext(new CandleSeriesAndCandleArg() {Series = s, Candle = c});
                }

                void OnCompleted(CandleSeries series)
                {
                    observer.OnCompleted();
                }
                candleManager.Processing += OnNext;
                candleManager.Error += observer.OnError;
                candleManager.Stopped += OnCompleted;
                return () =>
                {
                    candleManager.Processing -= OnNext;
                    candleManager.Error -= observer.OnError;
                    candleManager.Stopped -= OnCompleted;
                };
            });
            return observable;
        }
        /// <summary>
        /// The series processing end event.
        /// </summary>
        public static IObservable<CandleSeries> RxStopped(this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null) throw new ArgumentNullException(nameof(candleManager));
            return GetNewObservable<CandleSeries>(
                handler => candleManager.Stopped += handler,
                handler => candleManager.Stopped -= handler);
        }
        /// <summary>
        /// The data transfer error event.
        /// </summary>
        public static IObservable<Exception> RxError(this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null) throw new ArgumentNullException(nameof(candleManager));
            return GetNewObservable<Exception>(
                handler => candleManager.Error += handler,
                handler => candleManager.Error -= handler);
        }
        #endregion
 
        #region BaseLogSource
        public static IObservable<LogMessage> RxLog(this BaseLogSource baseLogSource)
        {
            if (baseLogSource == null) throw new ArgumentNullException(nameof(baseLogSource));
            return GetNewObservable<LogMessage>(
                handler => baseLogSource.Log += handler,
                handler => baseLogSource.Log -= handler);
        }
        #endregion
        #region Order
        /// <summary>
        /// To create a for the event of order unsuccessful registration on exchange.
        /// </summary>
        /// <param name="order">The order to be traced for unsuccessful registration event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<OrderFail> RxWhenRegisterFailed(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return order.Type == OrderTypes.Conditional ? 
                connector.RxStopOrderRegisterFailed().Where(f => f.Order == order) : 
                connector.RxOrderRegisterFailed().Where(f => f.Order == order);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of unsuccessful order cancelling on exchange.
        /// </summary>
        /// <param name="order">The order to be traced for unsuccessful cancelling event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<OrderFail> RxWhenCancelFailed(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return order.Type == OrderTypes.Conditional ? 
                connector.RxStopOrderCancelFailed().Where(f => f.Order == order) : 
                connector.RxOrderCancelFailed().Where(f => f.Order == order);
        }
        /// <summary>
        /// To create a Reactive Extension for the order cancelling event.
        /// </summary>
        /// <param name="order">The order to be traced for cancelling event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<MyTrade> RxWhenCanceled(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            return connector.RxNewMyTrade().Where(f => f.Order == order && f.Order.IsCanceled());
        }
        /// <summary>
        /// To create a Reactive Extension for the event of order fully matching.
        /// </summary>
        /// <param name="order">The order to be traced for the fully matching event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<MyTrade> RxWhenMatched(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            return connector.RxNewMyTrade().Where(f => f.Order == order && f.Order.IsMatched());
        }
        /// <summary>
        /// To create a Reactive Extension for the order change event.
        /// </summary>
        /// <param name="order">The order to be traced for the change event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Order> RxWhenChanged(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            if (order.Type == OrderTypes.Conditional)
            {
                var s1 = connector.RxStopOrderChanged().Where(o => o == order);
                var s2 = connector.RxNewStopOrder().Where(o => o == order);
                return s1.Merge(s2);
            }
            else
            {
                var s1 = connector.RxOrderChanged().Where(o => o == order);
                var s2 = connector.RxNewOrder().Where(o => o == order);
                return s1.Merge(s2);
            }
        }
        /// <summary>
        /// To create a Reactive Extension for the event of trade occurrence for the order.
        /// </summary>
        /// <param name="order">The order to be traced for trades occurrence events.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<MyTrade> RxWhenNewTrade(this Order order, IConnector connector)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            return connector.RxNewMyTrade().Where(f => f.Order == order);
        }
        #endregion
        #region Portfolio
        /// <summary>
        /// To create a Reactive Extension for the event of change portfolio .
        /// </summary>
        /// <param name="portfolio">The portfolio to be traced for the event of change.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Portfolio> RxWhenChanged(this Portfolio portfolio, IConnector connector)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));
            return connector.RxPortfolioChanged().Where(p => p == portfolio);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of money decrease in portfolio below the specific level.
        /// </summary>
        /// <param name="portfolio">The portfolio to be traced for the event of money decrease below the specific level.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <param name="money">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Portfolio> RxWhenMoneyLess(this Portfolio portfolio, IConnector connector, Unit money)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));
            if (money == null) throw new ArgumentNullException(nameof(money));
            var finishMoney = money.Type == UnitTypes.Limit ? money : portfolio.CurrentValue - money;

            return connector.RxPortfolioChanged().Where(p => p == portfolio && p.CurrentValue < finishMoney);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of money increase in portfolio above the specific level.
        /// </summary>
        /// <param name="portfolio">The portfolio to be traced for the event of money increase above the specific level.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <param name="money">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Portfolio> RxWhenMoneyMore(this Portfolio portfolio, IConnector connector, Unit money)
        {
            if (portfolio == null) throw new ArgumentNullException(nameof(portfolio));
            if (money == null) throw new ArgumentNullException(nameof(money));
            var finishMoney = money.Type == UnitTypes.Limit ? money : portfolio.CurrentValue + money;

            return connector.RxPortfolioChanged().Where(p => p == portfolio && p.CurrentValue > finishMoney);
        }
        #endregion
        #region Position
        /// <summary>
        /// To create a Reactive Extension for the event of position decrease below the specific level.
        /// </summary>
        /// <param name="position">The position to be traced for the event of decrease below the specific level.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <param name="value">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Position> RxWhenLess(this Position position, IConnector connector, Unit value)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var finishPosition = value.Type == UnitTypes.Limit ? value : position.CurrentValue - value;

            return connector.RxPositionChanged().Where(p => p == position && p.CurrentValue < finishPosition);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of position increase above the specific level.
        /// </summary>
        /// <param name="position">The position to be traced of the event of increase above the specific level.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <param name="value">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Position> RxWhenMore(this Position position, IConnector connector, Unit value)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var finishPosition = value.Type == UnitTypes.Limit ? value : position.CurrentValue + value;

            return connector.RxPositionChanged().Where(p => p == position && p.CurrentValue > finishPosition);
        }
        /// <summary>
        /// To create a Reactive Extension for the position change event.
        /// </summary>
        /// <param name="position">The position to be traced for the change event.</param>
        /// <param name="connector">The connection of interaction with trade systems.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Position> RxWhenChanged(this Position position, IConnector connector)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));
            return connector.RxPositionChanged().Where(p => p == position);
        }
        #endregion
        #region Security
        /// <summary>
        /// To create a Reactive Extension for the instrument change event.
        /// </summary>
        /// <param name="security">The instrument to be traced for changes.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Security> RxWhenChanged(this Security security, IConnector connector)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            return connector.RxSecurityChanged().Where(t =>
            {
                var basket = security as BasketSecurity;
                return basket?.Contains(ConfigManager.GetService<ISecurityProvider>(), t) ?? t == security;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of new trade occurrence for the instrument.
        /// </summary>
        /// <param name="security">The instrument to be traced for new trade occurrence event.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Trade> RxWhenNewTrade(this Security security, IConnector connector)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            return connector.RxNewTrade().Where(t =>
            {
                var basket = security as BasketSecurity;
                return basket?.Contains(ConfigManager.GetService<ISecurityProvider>(), t.Security) ?? t.Security == security;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of new notes occurrence in the orders log for instrument.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of new notes occurrence in the orders log.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<OrderLogItem> RxWhenNewOrderLogItem(this Security security, IConnector connector)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            return connector.RxNewOrderLogItem().Where(t =>
            {
                var basket = security as BasketSecurity;
                return basket?.Contains(ConfigManager.GetService<ISecurityProvider>(), t.Order.Security) ?? t.Order.Security == security;
            });
        }

        /// <summary>
        /// To create a Reactive Extension for the event of order book change by instruments basket.
        /// </summary>
        /// <param name="security">Instruments basket to be traced for the event of order books change by internal instruments.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<MarketDepth> RxWhenMarketDepthChanged(this Security security, IConnector connector)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            return connector.RxMarketDepthChanged().Where(t => 
            {
                var basket = security as BasketSecurity;
                return basket?.Contains(ConfigManager.GetService<ISecurityProvider>(), t.Security) ?? t.Security == security;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of excess of the best bid of specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of excess of the best bid of specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Security> RxWhenBestBidPriceMore(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            var finishPrice = (decimal)(price.Type == UnitTypes.Limit ? price : price - price);
            return security.RxWhenChanged(connector).Where((s) =>
            {
                var quote = (decimal?)connector.GetSecurityValue(s, Level1Fields.BuyBackPrice);
                return quote != null && quote > finishPrice;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of excess of the best offer of the specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of excess of the best offer of the specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Security> RxWhenBestAskPriceMore(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            var finishPrice = (decimal)(price.Type == UnitTypes.Limit ? price : price - price);
            return security.RxWhenChanged(connector).Where((s) =>
            {
                var quote = (decimal?)connector.GetSecurityValue(s, Level1Fields.BestAskPrice);
                return quote != null && quote > finishPrice;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of dropping the best bid below the specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of dropping the best bid below the specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Security> RxWhenBestBidPriceLess(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            var finishPrice = (decimal)(price.Type == UnitTypes.Limit ? price : price - price);
            return security.RxWhenChanged(connector).Where((s) =>
            {
                var quote = (decimal?)connector.GetSecurityValue(s, Level1Fields.BuyBackPrice);
                return quote != null && quote < finishPrice;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of dropping the best offer below the specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of dropping the best offer below the specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Security> RxWhenBestAskPricLess(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            var finishPrice = (decimal)(price.Type == UnitTypes.Limit ? price : price - price);
            return security.RxWhenChanged(connector).Where((s) =>
            {
                var quote = (decimal?)connector.GetSecurityValue(s, Level1Fields.BestAskPrice);
                return quote != null && quote < finishPrice;
            });
        }
        /// <summary>
        /// To create a Reactive Extension for the event of increase of the last trade price above the specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of increase of the last trade price above the specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="provider">The market data provider.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Trade> RxWhenLastTradePriceMore(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            return security.RxWhenNewTrade(connector).Where(t => t.Price > price);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of reduction of the last trade price below the specific level.
        /// </summary>
        /// <param name="security">The instrument to be traced for the event of reduction of the last trade price below the specific level.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="provider">The market data provider.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Trade> RxWhenLastTradePriceLess(this Security security, IConnector connector, Unit price)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (price == null) throw new ArgumentNullException(nameof(price));
            return security.RxWhenNewTrade(connector).Where(t => t.Price < price);
        }
        /// <summary>
        /// NotImplemented
        /// To create a Reactive Extension, activated at the exact time, specified through <paramref name="times" />.
        /// </summary>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="times">The exact time. Several values may be sent.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Trade> RxWhenTimeCome(this IConnector connector, params DateTimeOffset[] times)
        {
            //TODO: RxWhenTimeCome
            throw new NotImplementedException();
        }
        /// <summary>
        /// NotImplemented
        /// To create a Reactive Extension, activated at the exact time, specified through <paramref name="times" />.
        /// </summary>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="times">The exact time. Several values may be sent.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Trade> RxWhenTimeCome(this IConnector connector, IEnumerable<DateTimeOffset> times)
        {
            //TODO: RxWhenTimeCome
            throw new NotImplementedException();
        }

        #endregion
        #region MarketDepth
        /// <summary>
        /// Depth <see cref="MarketDepth.Depth"/> changed.
        /// </summary>
        public static IObservable<object> RxDepthChanged(this MarketDepth marketDepth)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return GetNewObservable(
                handler => marketDepth.DepthChanged += handler,
                handler => marketDepth.DepthChanged -= handler);
        }
        /// <summary>
        /// Event on exceeding the maximum allowable depth of quotes.
        /// </summary>
        public static IObservable<Quote> RxQuoteOutOfDepth(this MarketDepth marketDepth)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return GetNewObservable<Quote>(
                handler => marketDepth.QuoteOutOfDepth += handler,
                handler => marketDepth.QuoteOutOfDepth -= handler);
        }
        /// <summary>
        /// To reduce the order book to the required depth.
        /// </summary>
        /// <param name="marketDepth">New order book depth.</param>
        public static IObservable<object> RxQuotesChanged(this MarketDepth marketDepth)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return GetNewObservable(
                handler => marketDepth.QuotesChanged += handler,
                handler => marketDepth.QuotesChanged -= handler);
        }

        /// <summary>
        /// To create a Reactive Extension for the order book change event.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for change event.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenDepthChanged(this MarketDepth marketDepth)
        {
            return marketDepth.RxDepthChanged();
        }
        /// <summary>
        /// To create a Reactive Extension for the event of order book spread size increase on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the spread change event.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenSpreadMore(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            var pair = marketDepth.BestPair;
            var firstPrice = pair?.SpreadPrice ?? 0;
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestPair != null && marketDepth.BestPair.SpreadPrice > (firstPrice + price));
        }
        /// <summary>
        /// To create a Reactive Extension for the event of order book spread size decrease on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the spread change event.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenSpreadLess(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            var pair = marketDepth.BestPair;
            var firstPrice = pair?.SpreadPrice ?? 0;
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestPair != null && marketDepth.BestPair.SpreadPrice < (firstPrice - price));
        }
        /// <summary>
        /// To create a Reactive Extension for the event of the best bid increase on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the event of the best bid increase on a specific value.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenBestBidPriceMore(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestBid != null && marketDepth.BestBid.Price > price);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of the best offer increase on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the event of the best offer increase on a specific value.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenBestAskPriceMore(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestAsk != null && marketDepth.BestAsk.Price > price);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of the best bid decrease on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the event of the best bid decrease on a specific value.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenBestBidPriceLess(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestBid != null && marketDepth.BestBid.Price < price);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of the best offer decrease on a specific value.
        /// </summary>
        /// <param name="marketDepth">The order book to be traced for the event of the best offer decrease on a specific value.</param>
        /// <param name="price">The shift value.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<object> RxWhenBestAskPriceLess(this MarketDepth marketDepth, Unit price)
        {
            if (marketDepth == null) throw new ArgumentNullException(nameof(marketDepth));
            return marketDepth.RxQuotesChanged().Where(_ => marketDepth.BestAsk != null && marketDepth.BestAsk.Price < price);
        }
        #endregion
        #region Candle
        /// <summary>
        /// To create a Reactive Extension for the event of candle closing price reduction below a specific level.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for the event of candle closing price reduction below a specific level.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenClosePriceLess(this ICandleManager candleManager, Candle candle, Unit price)
        {
            if (candle == null) throw new ArgumentNullException(nameof(candle));
            if (price == null) throw new ArgumentNullException(nameof(price));

            return candleManager.RxProcessing().Where(arg=>arg.Candle==candle&& arg.Candle.ClosePrice< price).Select(arg => arg.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of candle closing price excess above a specific level.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for the event of candle closing price excess above a specific level.</param>
        /// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenClosePriceMore(this ICandleManager candleManager, Candle candle, Unit price)
        {
            if (candle == null) throw new ArgumentNullException(nameof(candle));
            if (price == null) throw new ArgumentNullException(nameof(price));

            return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.ClosePrice > price).Select(arg => arg.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of candle total volume excess above a specific level.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for the event of candle total volume excess above a specific level.</param>
        /// <param name="volume">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenTotalVolumeMore(this ICandleManager candleManager, Candle candle, Unit volume)
        {
            if (candle == null) throw new ArgumentNullException(nameof(candle));
            if (volume == null) throw new ArgumentNullException(nameof(volume));
            var finishVolume = volume.Type == UnitTypes.Limit ? volume : candle.TotalVolume + volume;
            return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.TotalVolume > finishVolume).Select(arg => arg.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of candle total volume excess above a specific level.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">Candles series, from which a candle will be taken.</param>
        /// <param name="volume">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenCurrentCandleTotalVolumeMore(this ICandleManager candleManager, CandleSeries series, Unit volume)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            if (volume == null) throw new ArgumentNullException(nameof(volume));
            var finishVolume = volume;

            if (volume.Type != UnitTypes.Limit)
            {
                var curCandle = candleManager.GetCurrentCandle<Candle>(series);

                if (curCandle == null)
                    throw new ArgumentException(nameof(series));

                finishVolume = curCandle.TotalVolume + volume;
            }
            return candleManager.RxProcessing().Where(arg => arg.Series == series && arg.Candle.TotalVolume > finishVolume).Select(arg => arg.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of new candles occurrence.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">Candles series to be traced for new candles.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenCandlesStarted(this ICandleManager candleManager, CandleSeries series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            return candleManager.RxWhenCandlesChanged(series).DistinctUntilChanged();
        }
        /// <summary>
        /// To create a Reactive Extension for candle change event.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">Candles series to be traced for changed candles.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenCandlesChanged(this ICandleManager candleManager, CandleSeries series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));

            return candleManager.RxProcessing().Where(arg => arg.Series == series).Select(b => b.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for the event of candles occurrence, change and end.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">Candles series to be traced for candles.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenCandles(this ICandleManager candleManager, CandleSeries series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            return candleManager.RxWhenCandlesChanged(series);
        }
        /// <summary>
        /// To create a Reactive Extension for candles end event.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">Candles series to be traced for end of candle.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenCandlesFinished(this ICandleManager candleManager, CandleSeries series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            return candleManager.RxWhenCandles(series).Where(c => c.State == CandleStates.Finished);
        }
        /// <summary>
        /// To create a Reactive Extension for candle change event.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for change.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenChanged(this ICandleManager candleManager, Candle candle)
        {
            if (candle == null) throw new ArgumentNullException(nameof(candle));
            return candleManager.RxProcessing().Where(arg=>arg.Candle== candle).Select(arg=>arg.Candle);
        }
        /// <summary>
        /// To create a Reactive Extension for candle end event.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for end.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenFinished(this ICandleManager candleManager, Candle candle)
        {
            if (candle == null) throw new ArgumentNullException(nameof(candle));
            return candleManager.RxProcessing().Where(arg => arg.Candle == candle&&arg.Candle.State== CandleStates.Finished).Select(arg => arg.Candle);
        }
        /// <summary>
        /// NotImplemented
        /// To create a Reactive Extension for the event of candle partial end.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="candle">The candle to be traced for partial end.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="percent">The percentage of candle completion.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenPartiallyFinished(this ICandleManager candleManager, Candle candle, IConnector connector, decimal percent)
        {
            //TODO: RxWhenPartiallyFinished
            throw new NotImplementedException();
        }
        /// <summary>
        /// NotImplemented
        /// To create a Reactive Extension for the event of candle partial end.
        /// </summary>
        /// <param name="candleManager">The candles manager.</param>
        /// <param name="series">The candle series to be traced for candle partial end.</param>
        /// <param name="connector">Connection to the trading system.</param>
        /// <param name="percent">The percentage of candle completion.</param>
        /// <returns>Reactive Extension.</returns>
        public static IObservable<Candle> RxWhenPartiallyFinishedCandles(this ICandleManager candleManager, CandleSeries series, IConnector connector, decimal percent)
        {
            //TODO: RxWhenPartiallyFinishedCandles
            throw new NotImplementedException();
        }

        #endregion
        #region IMarketDataProvider
        /// <summary>
        /// Security changed.
        /// </summary>
        public static IObservable<ValuesChangedArg> RxValuesChanged(this IMarketDataProvider marketDataProvider)
        {
            if (marketDataProvider == null) throw new ArgumentNullException(nameof(marketDataProvider));
            var observable = Observable.Create<ValuesChangedArg>(observer =>
            {
                void OnNext(Security security, IEnumerable<KeyValuePair<Level1Fields, object>> keyValuePairs, DateTimeOffset arg3, DateTimeOffset arg4)
                {
                    observer.OnNext(new ValuesChangedArg() { Security = security, Level1Fields = keyValuePairs, DateTimeOffset1 = arg3, DateTimeOffset2 = arg4 });
                }

                marketDataProvider.ValuesChanged += OnNext;

                return () =>
                {
                    marketDataProvider.ValuesChanged -= OnNext;
                };
            });
            return observable;
        }
        #endregion
        #region ISecurityProvider
        /// <summary>
        /// New instruments added.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxAdded(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null) throw new ArgumentNullException(nameof(securityProvider));
            return GetNewObservable<IEnumerable<Security>>(
                handler => securityProvider.Added += handler,
                handler => securityProvider.Added -= handler);
        }
        /// <summary>
        /// The storage was cleared.
        /// </summary>
        public static IObservable<object> RxCleared(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null) throw new ArgumentNullException(nameof(securityProvider));
            return GetNewObservable(
                handler => securityProvider.Cleared += handler,
                handler => securityProvider.Cleared -= handler);
        }
        /// <summary>
        /// Instruments removed.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxRemoved(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null) throw new ArgumentNullException(nameof(securityProvider));
            return GetNewObservable<IEnumerable<Security>>(
                handler => securityProvider.Removed += handler,
                handler => securityProvider.Removed -= handler);
        }
        #endregion
        #region IPortfolioProvider
        /// <summary>
        /// New portfolio received.
        /// </summary>
        public static IObservable<Portfolio> RxNewPortfolio(this IPortfolioProvider portfolioProvider)
        {
            if (portfolioProvider == null) throw new ArgumentNullException(nameof(portfolioProvider));
            return GetNewObservable<Portfolio>(
                handler => portfolioProvider.NewPortfolio += handler,
                handler => portfolioProvider.NewPortfolio -= handler);
        }

        #endregion

        private static IObservable<T> GetNewObservable<T>(Action<Action<T>> actionOnNextAdd, Action<Action<T>> actionOnNextRemove)
        {
            var observable = Observable.Create<T>(observer =>
            {
                void OnNext(T t)
                {
                    observer.OnNext(t);
                }
                actionOnNextAdd(OnNext);
                return () =>
                {
                    actionOnNextRemove(OnNext);
                };
            });
            return observable;
        }
        private static IObservable<object> GetNewObservable(Action<Action> actionOnNextAdd, Action<Action> actionOnNextRemove)
        {
            var observable = Observable.Create<object>(observer =>
            {
                void OnNext()
                {
                    observer.OnNext(null);
                }
                actionOnNextAdd(OnNext);
                return () =>
                {
                    actionOnNextRemove(OnNext);
                };
            });
            return observable;
        }
        private static IObservable<T> GetNewObservable<T>(Action<Action<T>> actionOnNextAdd, Action<Action<T>> actionOnNextRemove, Action<Action<T>> actionOnCompletedAdd, Action<Action<T>> actionOnCompletedRemove)
        {
            var observable = Observable.Create<T>(observer =>
            {
                void OnNext(T t)
                {
                    observer.OnNext(t);
                }
                void OnCompleted(T t)
                {
                    observer.OnCompleted();
                }
                actionOnNextAdd(OnNext);
                actionOnCompletedAdd(OnCompleted);
                return () =>
                {
                    actionOnNextRemove(OnNext);
                    actionOnCompletedRemove(OnCompleted);
                };
            });
            return observable;
        }
    }
}
