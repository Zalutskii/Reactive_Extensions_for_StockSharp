namespace ReactiveStockSharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using StockSharp.Algo;

    using StockSharp.Algo.Candles;
    using StockSharp.Algo.Strategies;
    using StockSharp.BusinessEntities;
    using StockSharp.Logging;
    using StockSharp.Messages;

	public static class RxWrappingEvents
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
                    observer.OnNext(new ExeptionAndEnumerablePortfoliosArg() {Exception = exception, Portfolios = portfolios});
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
                    observer.OnNext(new ExeptionAndEnumerableSecuritiesArg() {Exception = exception, Securities = securities});
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
        public static IObservable<SecurityAndMarketDataMessageAndExceptionArg> RxMarketDataSubscriptionFailed(
            this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageAndExceptionArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage, Exception exception)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageAndExceptionArg()
                    {
                        Exception = exception,
                        Security = security,
                        MarketDataMessage = marketDataMessage
                    });
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
        public static IObservable<SecurityAndMarketDataMessageArg> RxMarketDataSubscriptionSucceeded(
            this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage)
                {
                    observer.OnNext(
                        new SecurityAndMarketDataMessageArg() {Security = security, MarketDataMessage = marketDataMessage});
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
        public static IObservable<SecurityAndMarketDataMessageAndExceptionArg> RxMarketDataUnSubscriptionFailed(
            this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageAndExceptionArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage, Exception exception)
                {
                    observer.OnNext(new SecurityAndMarketDataMessageAndExceptionArg()
                    {
                        Exception = exception,
                        Security = security,
                        MarketDataMessage = marketDataMessage
                    });
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
        public static IObservable<SecurityAndMarketDataMessageArg> RxMarketDataUnSubscriptionSucceeded(
            this IConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            var observable = Observable.Create<SecurityAndMarketDataMessageArg>(observer =>
            {
                void OnNext(Security security, MarketDataMessage marketDataMessage)
                {
                    observer.OnNext(
                        new SecurityAndMarketDataMessageArg() {Security = security, MarketDataMessage = marketDataMessage});
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
                    observer.OnNext(new LongAndExceptionArg() {Exception = exception, Long = l1});
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
                    observer.OnNext(new LongAndExceptionArg() {Exception = exception, Long = l1});
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
                    observer.OnNext(
                        new ExchangeBoardAndSessionStatesnArg() {ExchangeBoard = exchangeBoard, SessionStates = sessionStates});
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

        #endregion

        #region Stratagy

        /// <summary>
        /// <see cref="OEC.API.Commission"/> change event.
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
                    observer.OnNext(new OrderAndOrderArg() {Order1 = order1, Order2 = order2});
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
                    observer.OnNext(new OrderAndOrderArg() {Order1 = order1, Order2 = order2});
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
                void OnNext(Security security, IEnumerable<KeyValuePair<Level1Fields, object>> keyValuePairs, DateTimeOffset arg3,
                    DateTimeOffset arg4)
                {
                    observer.OnNext(new ValuesChangedArg()
                    {
                        Security = security,
                        Level1Fields = keyValuePairs,
                        DateTimeOffset1 = arg3,
                        DateTimeOffset2 = arg4
                    });
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

        /// <summary>
        /// Portfolio changed event.
        /// </summary>
        public static IObservable<Portfolio> RxPortfolioChanged(this IPortfolioProvider portfolioProvider)
        {
            if (portfolioProvider == null) throw new ArgumentNullException(nameof(portfolioProvider));
            return GetNewObservable<Portfolio>(
                handler => portfolioProvider.PortfolioChanged += handler,
                handler => portfolioProvider.PortfolioChanged -= handler);
        }

        #endregion
        private static IObservable<T> GetNewObservable<T>(Action<Action<T>> actionOnNextAdd,
            Action<Action<T>> actionOnNextRemove)
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

        private static IObservable<T> GetNewObservable<T>(Action<Action<T>> actionOnNextAdd,
            Action<Action<T>> actionOnNextRemove, Action<Action<T>> actionOnCompletedAdd,
            Action<Action<T>> actionOnCompletedRemove)
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