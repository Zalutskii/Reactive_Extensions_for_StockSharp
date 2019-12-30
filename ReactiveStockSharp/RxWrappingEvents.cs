namespace ReactiveStockSharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;
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
        public static IObservable<(CandleSeries CandleSeries, Candle Candle)> RxCandleSeriesProcessing(
            this Connector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Action<CandleSeries, Candle>, (CandleSeries CandleSeries, Candle Candle)>(
                handler => (series, candle) => handler((series, candle)),
                handler => connector.CandleSeriesProcessing += handler,
                handler => connector.CandleSeriesProcessing -= handler);
        }


        /// <summary>
        /// The series processing end event.
        /// </summary>
        public static IObservable<CandleSeries> RxCandleSeriesStopped(this Connector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<CandleSeries>(
                handler => connector.CandleSeriesStopped += handler,
                handler => connector.CandleSeriesStopped -= handler);
        }

        /// <summary>
        /// Connected.
        /// </summary>
        public static IObservable<IConnector> RxConnected(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent(
                handler => connector.Connected += handler,
                handler => connector.Connected -= handler).Select(_ => connector);
        }

        /// <summary>
        /// Connected.
        /// </summary>
        public static IObservable<IMessageAdapter> RxConnectedEx(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IMessageAdapter>(
                handler => connector.ConnectedEx += handler,
                handler => connector.ConnectedEx -= handler);
        }

        /// <summary>
        /// Connection error (for example, the connection was aborted by server).
        /// </summary>
        public static IObservable<Exception> RxConnectionError(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Exception>(
                handler => connector.ConnectionError += handler,
                handler => connector.ConnectionError -= handler);
        }

        /// <summary>
        /// Disconnected.
        /// </summary>
        public static IObservable<IConnector> RxDisconnected(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent(
                handler => connector.Disconnected += handler,
                handler => connector.Disconnected -= handler).Select(_ => connector);
        }

        /// <summary>
        /// Disconnected.
        /// </summary>
        public static IObservable<IMessageAdapter> RxDisconnectedEx(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IMessageAdapter>(
                handler => connector.DisconnectedEx += handler,
                handler => connector.DisconnectedEx -= handler);
        }

        /// <summary>
        /// Data process error.
        /// </summary>
        public static IObservable<Exception> RxError(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Exception>(
                handler => connector.Error += handler,
                handler => connector.Error -= handler);
        }

        /// <summary>
        /// Lookup result <see cref="LookupPortfolios"/> received.
        /// </summary>
        public static
            IObservable<(PortfolioLookupMessage PortfolioLookupMessage, IEnumerable<Portfolio> Portfolios, Exception
                Exception)> RxLookupPortfoliosResult(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<PortfolioLookupMessage, IEnumerable<Portfolio>, Exception>, (PortfolioLookupMessage
                    PortfolioLookupMessage, IEnumerable<Portfolio> Portfolios, Exception Exception)>(
                    handler => (portfolioLookupMessage, portfolios, exception) =>
                        handler((portfolioLookupMessage, portfolios, exception)),
                    handler => connector.LookupPortfoliosResult += handler,
                    handler => connector.LookupPortfoliosResult -= handler);
        }

        /// <summary>
        /// Lookup result <see cref="LookupSecurities(Security)"/> received.
        /// </summary>
        public static
            IObservable<(SecurityLookupMessage SecurityLookupMessage, IEnumerable<Security> Securities, Exception
                Exception)> RxLookupSecuritiesResult(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<SecurityLookupMessage, IEnumerable<Security>, Exception>, (SecurityLookupMessage
                    SecurityLookupMessage, IEnumerable<Security> Securities, Exception Exception)>(
                    handler => (securityLookupMessage, security, exception) =>
                        handler((securityLookupMessage, security, exception)),
                    handler => connector.LookupSecuritiesResult += handler,
                    handler => connector.LookupSecuritiesResult -= handler);
        }

        /// <summary>
        /// Error subscription market-data.
        /// </summary>
        public static IObservable<(Security Security, MarketDataMessage MarketDataMessage, Exception Exception)>
            RxMarketDataSubscriptionFailed(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<Security, MarketDataMessage, Exception>, (Security Security, MarketDataMessage
                    MarketDataMessage, Exception Exception)>(
                    handler => (security, marketDataMessage, exception) =>
                        handler((security, marketDataMessage, exception)),
                    handler => connector.MarketDataSubscriptionFailed += handler,
                    handler => connector.MarketDataSubscriptionFailed -= handler);
        }

        /// <summary>
        /// Successful subscription market-data.
        /// </summary>
        public static IObservable<(Security Security, MarketDataMessage MarketDataMessage)>
            RxMarketDataSubscriptionSucceeded(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<Security, MarketDataMessage>, (Security Security, MarketDataMessage MarketDataMessage)
                >(
                    handler => (security, marketDataMessage) => handler((security, marketDataMessage)),
                    handler => connector.MarketDataSubscriptionSucceeded += handler,
                    handler => connector.MarketDataSubscriptionSucceeded -= handler);
        }

        /// <summary>
        /// Error unsubscription market-data.
        /// </summary>
        public static IObservable<(Security Security, MarketDataMessage MarketDataMessage, Exception Exception)>
            RxMarketDataUnSubscriptionFailed(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            return Observable
                .FromEvent<Action<Security, MarketDataMessage, Exception>, (Security Security, MarketDataMessage
                    MarketDataMessage, Exception Exception)>(
                    handler => (security, marketDataMessage, exception) =>
                        handler((security, marketDataMessage, exception)),
                    handler => connector.MarketDataUnSubscriptionFailed += handler,
                    handler => connector.MarketDataUnSubscriptionFailed -= handler);
        }

        /// <summary>
        /// Error unsubscription market-data.
        /// </summary>
        public static IObservable<(Security Security, MarketDataMessage MarketDataMessage)>
            RxMarketDataUnSubscriptionSucceeded(
                this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<Security, MarketDataMessage>, (Security Security, MarketDataMessage MarketDataMessage)
                >(
                    handler => (security, marketDataMessage) => handler((security, marketDataMessage)),
                    handler => connector.MarketDataUnSubscriptionSucceeded += handler,
                    handler => connector.MarketDataUnSubscriptionSucceeded -= handler);
        }

        /// <summary>
        /// Order book changed.
        /// </summary>
        public static IObservable<MarketDepth> RxMarketDepthChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<MarketDepth>(
                handler => connector.MarketDepthChanged += handler,
                handler => connector.MarketDepthChanged -= handler);
        }

        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<IEnumerable<MarketDepth>> RxMarketDepthsChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<MarketDepth>>(
                handler => connector.MarketDepthsChanged += handler,
                handler => connector.MarketDepthsChanged -= handler);
        }

        /// <summary>
        /// Server time changed <see cref="IConnector.ExchangeBoards"/>. It passed the time difference since the last call of the event. The first time the event passes the value <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public static IObservable<TimeSpan> RxMarketTimeChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<TimeSpan>(
                handler => connector.MarketTimeChanged += handler,
                handler => connector.MarketTimeChanged -= handler);
        }

        /// <summary>
        /// Mass order cancellation event.
        /// </summary>
        public static IObservable<long> RxMassOrderCanceled(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<long>(
                handler => connector.MassOrderCanceled += handler,
                handler => connector.MassOrderCanceled -= handler);
        }

        /// <summary>
        /// Mass order cancellation errors event.
        /// </summary>
        public static IObservable<(long Long, Exception Exception)> RxMassOrderCancelFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Action<long, Exception>, (long Long, Exception Exception)>(
                handler => (Long, exception) => handler((Long, exception)),
                handler => connector.MassOrderCancelFailed += handler,
                handler => connector.MassOrderCancelFailed -= handler);
        }

        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<MarketDepth> RxNewMarketDepth(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<MarketDepth>(
                handler => connector.NewMarketDepth += handler,
                handler => connector.NewMarketDepth -= handler);
        }

        /// <summary>
        /// Order books received.
        /// </summary>
        public static IObservable<IEnumerable<MarketDepth>> RxNewMarketDepths(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<MarketDepth>>(
                handler => connector.NewMarketDepths += handler,
                handler => connector.NewMarketDepths -= handler);
        }

        /// <summary>
        /// Message processed <see cref="Message"/>.
        /// </summary>
        public static IObservable<Message> RxNewMessage(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Message>(
                handler => connector.NewMessage += handler,
                handler => connector.NewMessage -= handler);
        }

        /// <summary>
        /// Own trade received.
        /// </summary>
        public static IObservable<MyTrade> RxNewMyTrade(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<MyTrade>(
                handler => connector.NewMyTrade += handler,
                handler => connector.NewMyTrade -= handler);
        }

        /// <summary>
        /// Own trades received.
        /// </summary>
        public static IObservable<IEnumerable<MyTrade>> RxNewMyTrades(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<MyTrade>>(
                handler => connector.NewMyTrades += handler,
                handler => connector.NewMyTrades -= handler);
        }

        /// <summary>
        /// News received.
        /// </summary>
        public static IObservable<News> RxNewNews(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<News>(
                handler => connector.NewNews += handler,
                handler => connector.NewNews -= handler);
        }

        /// <summary>
        /// Order received.
        /// </summary>
        public static IObservable<Order> RxNewOrder(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Order>(
                handler => connector.NewOrder += handler,
                handler => connector.NewOrder -= handler);
        }

        /// <summary>
        /// Order log received.
        /// </summary>
        public static IObservable<OrderLogItem> RxNewOrderLogItem(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<OrderLogItem>(
                handler => connector.NewOrderLogItem += handler,
                handler => connector.NewOrderLogItem -= handler);
        }

        /// <summary>
        /// Order log received.
        /// </summary>
        public static IObservable<IEnumerable<OrderLogItem>> RxNewOrderLogItems(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<OrderLogItem>>(
                handler => connector.NewOrderLogItems += handler,
                handler => connector.NewOrderLogItems -= handler);
        }

        /// <summary>
        /// Orders received.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxNewOrders(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Order>>(
                handler => connector.NewOrders += handler,
                handler => connector.NewOrders -= handler);
        }

        /// <summary>
        /// Portfolios received.
        /// </summary>
        public static IObservable<IEnumerable<Portfolio>> RxNewPortfolios(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Portfolio>>(
                handler => connector.NewPortfolios += handler,
                handler => connector.NewPortfolios -= handler);
        }

        ///// <summary>
        ///// Position received.
        ///// </summary>
        public static IObservable<Position> RxNewPosition(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Position>(
                handler => connector.NewPosition += handler,
                handler => connector.NewPosition -= handler);
        }

        /// <summary>
        /// Positions received.
        /// </summary>
        public static IObservable<IEnumerable<Position>> RxNewPositions(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Position>>(
                handler => connector.NewPositions += handler,
                handler => connector.NewPositions -= handler);
        }

        /// <summary>
        /// News updated (news body received <see cref="News.Story"/>).
        /// </summary>
        public static IObservable<News> RxNewsChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<News>(
                handler => connector.NewsChanged += handler,
                handler => connector.NewsChanged -= handler);
        }

        /// <summary>
        /// Securities received.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxNewSecurities(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Security>>(
                handler => connector.NewSecurities += handler,
                handler => connector.NewSecurities -= handler);
        }

        /// <summary>
        /// Security received.
        /// </summary>
        public static IObservable<Security> RxNewSecurity(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Security>(
                handler => connector.NewSecurity += handler,
                handler => connector.NewSecurity -= handler);
        }

        /// <summary>
        /// Stop-order received.
        /// </summary>
        public static IObservable<Order> RxNewStopOrder(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Order>(
                handler => connector.NewStopOrder += handler,
                handler => connector.NewStopOrder -= handler);
        }

        /// <summary>
        /// Stop-orders received.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxNewStopOrders(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Order>>(
                handler => connector.NewStopOrders += handler,
                handler => connector.NewStopOrders -= handler);
        }

        /// <summary>
        /// Tick trade received.
        /// </summary>
        public static IObservable<Trade> RxNewTrade(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Trade>(
                handler => connector.NewTrade += handler,
                handler => connector.NewTrade -= handler);
        }

        /// <summary>
        /// Tick trades received.
        /// </summary>
        public static IObservable<IEnumerable<Trade>> RxNewTrades(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Trade>>(
                handler => connector.NewTrades += handler,
                handler => connector.NewTrades -= handler);
        }

        /// <summary>
        /// Order cancellation error event.
        /// </summary>
        public static IObservable<OrderFail> RxOrderCancelFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<OrderFail>(
                handler => connector.OrderCancelFailed += handler,
                handler => connector.OrderCancelFailed -= handler);
        }

        /// <summary>
        /// Order changed (cancelled, matched).
        /// </summary>
        public static IObservable<Order> RxOrderChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Order>(
                handler => connector.OrderChanged += handler,
                handler => connector.OrderChanged -= handler);
        }

        /// <summary>
        /// Order registration error event.
        /// </summary>
        public static IObservable<OrderFail> RxOrderRegisterFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<OrderFail>(
                handler => connector.OrderRegisterFailed += handler,
                handler => connector.OrderRegisterFailed -= handler);
        }

        /// <summary>
        /// Order cancellation errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxOrdersCancelFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<OrderFail>>(
                handler => connector.OrdersCancelFailed += handler,
                handler => connector.OrdersCancelFailed -= handler);
        }

        /// <summary>
        /// Stop orders state change event.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxOrdersChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Order>>(
                handler => connector.OrdersChanged += handler,
                handler => connector.OrdersChanged -= handler);
        }

        /// <summary>
        /// Order registration errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxOrdersRegisterFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<OrderFail>>(
                handler => connector.OrdersRegisterFailed += handler,
                handler => connector.OrdersRegisterFailed -= handler);
        }

        /// <summary>
        /// Failed order status request event.
        /// </summary>
        public static IObservable<(long Long, Exception Exception)> RxOrderStatusFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            return Observable.FromEvent<Action<long, Exception>, (long Long, Exception Exception)>(
                handler => (Long, exception) => handler((Long, exception)),
                handler => connector.OrderStatusFailed += handler,
                handler => connector.OrderStatusFailed -= handler);
        }

        ///// <summary>
        ///// Portfolio changed.
        ///// </summary>
        public static IObservable<Portfolio> RxPortfolioChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Portfolio>(
                handler => connector.PortfolioChanged += handler,
                handler => connector.PortfolioChanged -= handler);
        }

        /// <summary>
        /// Portfolios changed.
        /// </summary>
        public static IObservable<IEnumerable<Portfolio>> RxPortfoliosChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Portfolio>>(
                handler => connector.PortfoliosChanged += handler,
                handler => connector.PortfoliosChanged -= handler);
        }

        ///// <summary>
        ///// Position changed.
        ///// </summary>
        public static IObservable<Position> RxPositionChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Position>(
                handler => connector.PositionChanged += handler,
                handler => connector.PositionChanged -= handler);
        }

        /// <summary>
        /// Positions changed.
        /// </summary>
        public static IObservable<IEnumerable<Position>> RxPositionsChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Position>>(
                handler => connector.PositionsChanged += handler,
                handler => connector.PositionsChanged -= handler);
        }

        /// <summary>
        /// Securities changed.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxSecuritiesChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Security>>(
                handler => connector.SecuritiesChanged += handler,
                handler => connector.SecuritiesChanged -= handler);
        }

        /// <summary>
        /// Security changed.
        /// </summary>
        public static IObservable<Security> RxSecurityChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Security>(
                handler => connector.SecurityChanged += handler,
                handler => connector.SecurityChanged -= handler);
        }

        /// <summary>
        /// Session changed.
        /// </summary>
        public static IObservable<(ExchangeBoard ExchangeBoard, SessionStates SessionStates)> RxSessionStateChanged(
            this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable
                .FromEvent<Action<ExchangeBoard, SessionStates>, (ExchangeBoard ExchangeBoard, SessionStates
                    SessionStates)>(
                    handler => (exchangeBoard, sessionStates) => handler((exchangeBoard, sessionStates)),
                    handler => connector.SessionStateChanged += handler,
                    handler => connector.SessionStateChanged -= handler);
        }

        /// <summary>
        /// Stop-order cancellation error event.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderCancelFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<OrderFail>(
                handler => connector.StopOrderCancelFailed += handler,
                handler => connector.StopOrderCancelFailed -= handler);
        }

        /// <summary>
        /// Stop order state change event.
        /// </summary>
        public static IObservable<Order> RxStopOrderChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<Order>(
                handler => connector.StopOrderChanged += handler,
                handler => connector.StopOrderChanged -= handler);
        }

        /// <summary>
        /// Stop-order registration error event.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderRegisterFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<OrderFail>(
                handler => connector.StopOrderRegisterFailed += handler,
                handler => connector.StopOrderRegisterFailed -= handler);
        }

        /// <summary>
        /// Stop-order cancellation errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxStopOrdersCancelFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<OrderFail>>(
                handler => connector.StopOrdersCancelFailed += handler,
                handler => connector.StopOrdersCancelFailed -= handler);
        }

        /// <summary>
        /// Stop orders state change event.
        /// </summary>
        public static IObservable<IEnumerable<Order>> RxStopOrdersChanged(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<Order>>(
                handler => connector.StopOrdersChanged += handler,
                handler => connector.StopOrdersChanged -= handler);
        }

        /// <summary>
        /// Stop-order registration errors event.
        /// </summary>
        public static IObservable<IEnumerable<OrderFail>> RxStopOrdersRegisterFailed(this IConnector connector)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            return Observable.FromEvent<IEnumerable<OrderFail>>(
                handler => connector.StopOrdersRegisterFailed += handler,
                handler => connector.StopOrdersRegisterFailed -= handler);
        }

        #endregion

        #region Stratagy

        /// <summary>
        /// <see cref="OEC.API.Commission"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxCommissionChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.CommissionChanged += handler,
                handler => strategy.CommissionChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// The event of strategy connection change.
        /// </summary>
        public static IObservable<Strategy> RxConnectorChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.ConnectorChanged += handler,
                handler => strategy.ConnectorChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// The event of error occurrence in the strategy.
        /// </summary>
        public static IObservable<(Strategy Strategy, Exception Exception)> RxError(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Action<Strategy, Exception>, (Strategy Strategy, Exception Exception)>(
                handler => (security, marketDataMessage) => handler((security, marketDataMessage)),
                handler => strategy.Error += handler,
                handler => strategy.Error -= handler);
        }

        /// <summary>
        /// <see cref="Latency"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxLatencyChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.LatencyChanged += handler,
                handler => strategy.LatencyChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// The event of new trade occurrence.
        /// </summary>
        public static IObservable<MyTrade> RxNewMyTrade(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<MyTrade>(
                handler => strategy.NewMyTrade += handler,
                handler => strategy.NewMyTrade -= handler);
        }

        /// <summary>
        /// The event of order cancelling order.
        /// </summary>
        public static IObservable<OrderFail> RxOrderCancelFailed(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<OrderFail>(
                handler => strategy.OrderCancelFailed += handler,
                handler => strategy.OrderCancelFailed -= handler);
        }

        /// <summary>
        /// The event of sending order for cancelling.
        /// </summary>
        public static IObservable<Order> RxOrderCanceling(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.OrderCanceling += handler,
                handler => strategy.OrderCanceling -= handler);
        }

        /// <summary>
        /// The event of order successful registration.
        /// </summary>
        public static IObservable<Order> RxOrderRegistered(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.OrderRegistered += handler,
                handler => strategy.OrderRegistered -= handler);
        }

        /// <summary>
        /// The event of order registration error.
        /// </summary>
        public static IObservable<OrderFail> RxOrderRegisterFailed(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<OrderFail>(
                handler => strategy.OrderRegisterFailed += handler,
                handler => strategy.OrderRegisterFailed -= handler);
        }

        /// <summary>
        /// The event of sending order for registration.
        /// </summary>
        public static IObservable<Order> RxOrderRegistering(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.OrderRegistering += handler,
                handler => strategy.OrderRegistering -= handler);
        }

        /// <summary>
        /// The event of sending order for re-registration.
        /// </summary>
        public static IObservable<(Order Order1, Order Order2)> RxOrderReRegistering(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Action<Order, Order>, (Order Order1, Order Order2)>(
                handler => (security, marketDataMessage) => handler((security, marketDataMessage)),
                handler => strategy.OrderReRegistering += handler,
                handler => strategy.OrderReRegistering -= handler);
        }

        /// <summary>
        /// <see cref="Parameters"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxParametersChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.ParametersChanged += handler,
                handler => strategy.ParametersChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// <see cref="Strategy.PnL"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxPnLChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.PnLChanged += handler,
                handler => strategy.PnLChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// <see cref="Position"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxPositionChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.PositionChanged += handler,
                handler => strategy.PositionChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// <see cref="Strategy.ProcessState"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxProcessStateChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Strategy>(
                handler => strategy.ProcessStateChanged += handler,
                handler => strategy.ProcessStateChanged -= handler);
        }

        /// <summary>
        /// NotImplemented
        /// The event of strategy parameters change. NotImplemented
        /// </summary>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> RxPropertyChanged(
            this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => strategy.PropertyChanged += h,
                h => strategy.PropertyChanged -= h);
        }

        /// <summary>
        /// The event of the strategy re-initialization.
        /// </summary>
        public static IObservable<Strategy> RxReseted(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.Reseted += handler,
                handler => strategy.Reseted -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// The event of strategy instrument change.
        /// </summary>
        public static IObservable<Strategy> RxSecurityChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.SecurityChanged += handler,
                handler => strategy.SecurityChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// <see cref="Strategy.Slippage"/> change event.
        /// </summary>
        public static IObservable<Strategy> RxSlippageChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent(
                handler => strategy.SlippageChanged += handler,
                handler => strategy.SlippageChanged -= handler).Select(_ => strategy);
        }

        /// <summary>
        /// The event of stop-order cancelling order.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderCancelFailed(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<OrderFail>(
                handler => strategy.StopOrderCancelFailed += handler,
                handler => strategy.StopOrderCancelFailed -= handler);
        }

        /// <summary>
        /// The event of sending stop-order for cancelling.
        /// </summary>
        public static IObservable<Order> RxStopOrderCanceling(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.StopOrderCanceling += handler,
                handler => strategy.StopOrderCanceling -= handler);
        }

        /// <summary>
        /// The event of stop-order change.
        /// </summary>
        public static IObservable<Order> RxStopOrderChanged(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.StopOrderChanged += handler,
                handler => strategy.StopOrderChanged -= handler);
        }

        /// <summary>
        /// The event of stop-order successful registration.
        /// </summary>
        public static IObservable<Order> RxStopOrderRegistered(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.StopOrderRegistered += handler,
                handler => strategy.StopOrderRegistered -= handler);
        }

        /// <summary>
        /// The event of stop-order registration error.
        /// </summary>
        public static IObservable<OrderFail> RxStopOrderRegisterFailed(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<OrderFail>(
                handler => strategy.StopOrderRegisterFailed += handler,
                handler => strategy.StopOrderRegisterFailed -= handler);
        }

        /// <summary>
        /// The event of sending stop-order for registration.
        /// </summary>
        public static IObservable<Order> RxStopOrderRegistering(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Order>(
                handler => strategy.StopOrderRegistering += handler,
                handler => strategy.StopOrderRegistering -= handler);
        }

        /// <summary>
        /// The event of sending stop-order for re-registration.
        /// </summary>
        public static IObservable<(Order Order1, Order Order2)> RxStopOrderReRegistering(this Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            return Observable.FromEvent<Action<Order, Order>, (Order Order1, Order Order2)>(
                handler => (security, marketDataMessage) => handler((security, marketDataMessage)),
                handler => strategy.StopOrderReRegistering += handler,
                handler => strategy.StopOrderReRegistering -= handler);
        }

        #endregion

        #region CandleManager

        /// <summary>
        /// A new value for processing occurrence event.
        /// </summary>
        public static IObservable<(CandleSeries Series, Candle Candle)> RxProcessing(
            this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null)
                throw new ArgumentNullException(nameof(candleManager));
            return Observable.FromEvent<Action<CandleSeries, Candle>, (CandleSeries CandleSeries, Candle Candle)>(
                handler => (candleSeries, candle) => handler((candleSeries, candle)),
                handler => candleManager.Processing += handler,
                handler => candleManager.Processing -= handler);
        }

        /// <summary>
        /// The series processing end event.
        /// </summary>
        public static IObservable<CandleSeries> RxStopped(this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null)
                throw new ArgumentNullException(nameof(candleManager));
            return Observable.FromEvent<CandleSeries>(
                handler => candleManager.Stopped += handler,
                handler => candleManager.Stopped -= handler);
        }

        /// <summary>
        /// The data transfer error event.
        /// </summary>
        public static IObservable<Exception> RxError(this ICandleSource<Candle> candleManager)
        {
            if (candleManager == null)
                throw new ArgumentNullException(nameof(candleManager));
            return Observable.FromEvent<Exception>(
                handler => candleManager.Error += handler,
                handler => candleManager.Error -= handler);
        }

        #endregion

        #region BaseLogSource

        public static IObservable<LogMessage> RxLog(this BaseLogSource baseLogSource)
        {
            if (baseLogSource == null)
                throw new ArgumentNullException(nameof(baseLogSource));
            return Observable.FromEvent<LogMessage>(
                handler => baseLogSource.Log += handler,
                handler => baseLogSource.Log -= handler);
        }

        #endregion

        #region MarketDepth

        /// <summary>
        /// Depth <see cref="MarketDepth.Depth"/> changed.
        /// </summary>
        public static IObservable<MarketDepth> RxDepthChanged(this MarketDepth marketDepth)
        {
            if (marketDepth == null)
                throw new ArgumentNullException(nameof(marketDepth));
            return Observable.FromEvent(
                handler => marketDepth.DepthChanged += handler,
                handler => marketDepth.DepthChanged -= handler).Select(_ => marketDepth);
        }

        /// <summary>
        /// To reduce the order book to the required depth.
        /// </summary>
        /// <param name="marketDepth">New order book depth.</param>
        public static IObservable<MarketDepth> RxQuotesChanged(this MarketDepth marketDepth)
        {
            if (marketDepth == null)
                throw new ArgumentNullException(nameof(marketDepth));
            return Observable.FromEvent(
                handler => marketDepth.QuotesChanged += handler,
                handler => marketDepth.QuotesChanged -= handler).Select(_ => marketDepth);
        }

        #endregion

        #region IMarketDataProvider

        /// <summary>
        /// Security changed.
        /// </summary>
        public static
            IObservable<(Security Security, IEnumerable<KeyValuePair<Level1Fields, object>> Level1Fields, DateTimeOffset
                DateTimeOffset1, DateTimeOffset DateTimeOffset2)> RxValuesChanged(
                this IMarketDataProvider marketDataProvider)
        {
            if (marketDataProvider == null)
                throw new ArgumentNullException(nameof(marketDataProvider));
            return Observable
                .FromEvent<Action<Security, IEnumerable<KeyValuePair<Level1Fields, object>>, DateTimeOffset,
                        DateTimeOffset>,
                    (Security Security, IEnumerable<KeyValuePair<Level1Fields, object>> Level1Fields, DateTimeOffset
                    DateTimeOffset1, DateTimeOffset DateTimeOffset2)>(
                    handler => (security, level1Fields, dateTimeOffset1, dateTimeOffset2) =>
                        handler((security, level1Fields, dateTimeOffset1, dateTimeOffset2)),
                    handler => marketDataProvider.ValuesChanged += handler,
                    handler => marketDataProvider.ValuesChanged -= handler);

        }

        #endregion

        #region ISecurityProvider

        /// <summary>
        /// New instruments added.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxAdded(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            return Observable.FromEvent<IEnumerable<Security>>(
                handler => securityProvider.Added += handler,
                handler => securityProvider.Added -= handler);
        }

        /// <summary>
        /// The storage was cleared.
        /// </summary>
        public static IObservable<ISecurityProvider> RxCleared(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            return Observable.FromEvent(
                handler => securityProvider.Cleared += handler,
                handler => securityProvider.Cleared -= handler).Select(_ => securityProvider);
        }

        /// <summary>
        /// Instruments removed.
        /// </summary>
        public static IObservable<IEnumerable<Security>> RxRemoved(this ISecurityProvider securityProvider)
        {
            if (securityProvider == null)
                throw new ArgumentNullException(nameof(securityProvider));
            return Observable.FromEvent<IEnumerable<Security>>(
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
            if (portfolioProvider == null)
                throw new ArgumentNullException(nameof(portfolioProvider));
            return Observable.FromEvent<Portfolio>(
                handler => portfolioProvider.NewPortfolio += handler,
                handler => portfolioProvider.NewPortfolio -= handler);
        }

        /// <summary>
        /// Portfolio changed event.
        /// </summary>
        public static IObservable<Portfolio> RxPortfolioChanged(this IPortfolioProvider portfolioProvider)
        {
            if (portfolioProvider == null)
                throw new ArgumentNullException(nameof(portfolioProvider));
            return Observable.FromEvent<Portfolio>(
                handler => portfolioProvider.PortfolioChanged += handler,
                handler => portfolioProvider.PortfolioChanged -= handler);
        }

        #endregion

    }
}