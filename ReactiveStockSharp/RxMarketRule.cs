namespace ReactiveStockSharp
{
	using System;
	using System.Collections.Generic;
	using System.Reactive.Linq;

	using Ecng.Configuration;

	using StockSharp.Algo;
	using StockSharp.Algo.Candles;
	using StockSharp.BusinessEntities;
	using StockSharp.Messages;

	public static class RxMarketRule
	{
		#region Connector

		/// <summary>
		/// To create a Reactive Extension for the event of candles occurrence, change and end.
		/// </summary>
		/// <param name="connector">The connector.</param>
		/// <param name="series">Candles series to be traced for candles.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<(CandleSeries Series,Candle Candle)> RxWhenCandles(this Connector connector, CandleSeries series)
		{
			if (series == null)
				throw new ArgumentNullException(nameof(series));
			return connector.RxCandleSeriesProcessing().Where(arg => arg.CandleSeries == series);
		}

		/// <summary>
		///  NotImplemented
		/// To create a Reactive Extension for the event <see cref="IConnector.MarketTimeChanged"/>, activated after expiration of <paramref name="interval" />.
		/// </summary>
		/// <param name="connector">Connection to the trading system.</param>
		/// <param name="interval">Interval.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<(CandleSeries Series,Candle Candle)> RxWhenIntervalElapsed(this IConnector connector,
			TimeSpan interval)
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

		#region Order

		/// <summary>
		/// To create a for the event of order unsuccessful registration on exchange.
		/// </summary>
		/// <param name="order">The order to be traced for unsuccessful registration event.</param>
		/// <param name="connector">The connection of interaction with trade systems.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<OrderFail> RxWhenRegisterFailed(this Order order, IConnector connector)
		{
			if (order == null)
				throw new ArgumentNullException(nameof(order));

			return order.Type == OrderTypes.Conditional
				? connector.RxStopOrderRegisterFailed().Where(f => f.Order == order)
				: connector.RxOrderRegisterFailed().Where(f => f.Order == order);
		}

		/// <summary>
		/// To create a Reactive Extension for the event of unsuccessful order cancelling on exchange.
		/// </summary>
		/// <param name="order">The order to be traced for unsuccessful cancelling event.</param>
		/// <param name="connector">The connection of interaction with trade systems.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<OrderFail> RxWhenCancelFailed(this Order order, IConnector connector)
		{
			if (order == null)
				throw new ArgumentNullException(nameof(order));

			return order.Type == OrderTypes.Conditional
				? connector.RxStopOrderCancelFailed().Where(f => f.Order == order)
				: connector.RxOrderCancelFailed().Where(f => f.Order == order);
		}

		/// <summary>
		/// To create a Reactive Extension for the order cancelling event.
		/// </summary>
		/// <param name="order">The order to be traced for cancelling event.</param>
		/// <param name="connector">The connection of interaction with trade systems.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<MyTrade> RxWhenCanceled(this Order order, IConnector connector)
		{
			if (order == null)
				throw new ArgumentNullException(nameof(order));
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
			if (order == null)
				throw new ArgumentNullException(nameof(order));
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
			if (order == null)
				throw new ArgumentNullException(nameof(order));

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
			if (order == null)
				throw new ArgumentNullException(nameof(order));
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
			if (portfolio == null)
				throw new ArgumentNullException(nameof(portfolio));
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
			if (portfolio == null)
				throw new ArgumentNullException(nameof(portfolio));
			if (money == null)
				throw new ArgumentNullException(nameof(money));
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
			if (portfolio == null)
				throw new ArgumentNullException(nameof(portfolio));
			if (money == null)
				throw new ArgumentNullException(nameof(money));
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
			if (position == null)
				throw new ArgumentNullException(nameof(position));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
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
			if (position == null)
				throw new ArgumentNullException(nameof(position));
			if (value == null)
				throw new ArgumentNullException(nameof(value));
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
			if (position == null)
				throw new ArgumentNullException(nameof(position));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			return connector.RxNewOrderLogItem().Where(t =>
			{
				var basket = security as BasketSecurity;
				return basket?.Contains(ConfigManager.GetService<ISecurityProvider>(), t.Order.Security) ??
				       t.Order.Security == security;
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
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
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
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
		/// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<Trade> RxWhenLastTradePriceMore(this Security security, IConnector connector, Unit price)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
			return security.RxWhenNewTrade(connector).Where(t => t.Price > price);
		}

		/// <summary>
		/// To create a Reactive Extension for the event of reduction of the last trade price below the specific level.
		/// </summary>
		/// <param name="security">The instrument to be traced for the event of reduction of the last trade price below the specific level.</param>
		/// <param name="connector">Connection to the trading system.</param>
		/// <param name="price">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<Trade> RxWhenLastTradePriceLess(this Security security, IConnector connector, Unit price)
		{
			if (security == null)
				throw new ArgumentNullException(nameof(security));
			if (price == null)
				throw new ArgumentNullException(nameof(price));
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
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
			var pair = marketDepth.BestPair;
			var firstPrice = pair?.SpreadPrice ?? 0;
			return marketDepth.RxQuotesChanged().Where(_ =>
				marketDepth.BestPair != null && marketDepth.BestPair.SpreadPrice > (firstPrice + price));
		}

		/// <summary>
		/// To create a Reactive Extension for the event of order book spread size decrease on a specific value.
		/// </summary>
		/// <param name="marketDepth">The order book to be traced for the spread change event.</param>
		/// <param name="price">The shift value.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<object> RxWhenSpreadLess(this MarketDepth marketDepth, Unit price)
		{
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
			var pair = marketDepth.BestPair;
			var firstPrice = pair?.SpreadPrice ?? 0;
			return marketDepth.RxQuotesChanged().Where(_ =>
				marketDepth.BestPair != null && marketDepth.BestPair.SpreadPrice < (firstPrice - price));
		}

		/// <summary>
		/// To create a Reactive Extension for the event of the best bid increase on a specific value.
		/// </summary>
		/// <param name="marketDepth">The order book to be traced for the event of the best bid increase on a specific value.</param>
		/// <param name="price">The shift value.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<object> RxWhenBestBidPriceMore(this MarketDepth marketDepth, Unit price)
		{
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
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
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
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
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
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
			if (marketDepth == null)
				throw new ArgumentNullException(nameof(marketDepth));
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
			if (candle == null)
				throw new ArgumentNullException(nameof(candle));
			if (price == null)
				throw new ArgumentNullException(nameof(price));

			return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.ClosePrice < price)
				.Select(arg => arg.Candle);
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
			if (candle == null)
				throw new ArgumentNullException(nameof(candle));
			if (price == null)
				throw new ArgumentNullException(nameof(price));

			return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.ClosePrice > price)
				.Select(arg => arg.Candle);
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
			if (candle == null)
				throw new ArgumentNullException(nameof(candle));
			if (volume == null)
				throw new ArgumentNullException(nameof(volume));
			var finishVolume = volume.Type == UnitTypes.Limit ? volume : candle.TotalVolume + volume;
			return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.TotalVolume > finishVolume)
				.Select(arg => arg.Candle);
		}

		/// <summary>
		/// To create a Reactive Extension for the event of candle total volume excess above a specific level.
		/// </summary>
		/// <param name="candleManager">The candles manager.</param>
		/// <param name="series">Candles series, from which a candle will be taken.</param>
		/// <param name="volume">The level. If the <see cref="Unit.Type"/> type equals to <see cref="UnitTypes.Limit"/>, specified price is set. Otherwise, shift value is specified.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<Candle> RxWhenCurrentCandleTotalVolumeMore(this ICandleManager candleManager,
			CandleSeries series, Unit volume)
		{
			if (series == null)
				throw new ArgumentNullException(nameof(series));
			if (volume == null)
				throw new ArgumentNullException(nameof(volume));
			var finishVolume = volume;

			if (volume.Type != UnitTypes.Limit)
			{
				var curCandle = candleManager.GetCurrentCandle<Candle>(series);

				if (curCandle == null)
					throw new ArgumentException(nameof(series));

				finishVolume = curCandle.TotalVolume + volume;
			}
			return candleManager.RxProcessing().Where(arg => arg.Series == series && arg.Candle.TotalVolume > finishVolume)
				.Select(arg => arg.Candle);
		}

		/// <summary>
		/// To create a Reactive Extension for the event of new candles occurrence.
		/// </summary>
		/// <param name="candleManager">The candles manager.</param>
		/// <param name="series">Candles series to be traced for new candles.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<Candle> RxWhenCandlesStarted(this ICandleManager candleManager, CandleSeries series)
		{
			if (series == null)
				throw new ArgumentNullException(nameof(series));
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
			if (series == null)
				throw new ArgumentNullException(nameof(series));

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
			if (series == null)
				throw new ArgumentNullException(nameof(series));
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
			if (series == null)
				throw new ArgumentNullException(nameof(series));
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
			if (candle == null)
				throw new ArgumentNullException(nameof(candle));
			return candleManager.RxProcessing().Where(arg => arg.Candle == candle).Select(arg => arg.Candle);
		}

		/// <summary>
		/// To create a Reactive Extension for candle end event.
		/// </summary>
		/// <param name="candleManager">The candles manager.</param>
		/// <param name="candle">The candle to be traced for end.</param>
		/// <returns>Reactive Extension.</returns>
		public static IObservable<Candle> RxWhenFinished(this ICandleManager candleManager, Candle candle)
		{
			if (candle == null)
				throw new ArgumentNullException(nameof(candle));
			return candleManager.RxProcessing().Where(arg => arg.Candle == candle && arg.Candle.State == CandleStates.Finished)
				.Select(arg => arg.Candle);
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
		public static IObservable<Candle> RxWhenPartiallyFinished(this ICandleManager candleManager, Candle candle,
			IConnector connector, decimal percent)
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
		public static IObservable<Candle> RxWhenPartiallyFinishedCandles(this ICandleManager candleManager,
			CandleSeries series, IConnector connector, decimal percent)
		{
			//TODO: RxWhenPartiallyFinishedCandles
			throw new NotImplementedException();
		}

		#endregion
	}
}