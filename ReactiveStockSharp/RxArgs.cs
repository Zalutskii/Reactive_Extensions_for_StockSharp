using System;
using System.Collections.Generic;

using StockSharp.Algo.Candles;
using StockSharp.BusinessEntities;
using StockSharp.Messages;

namespace ReactiveStockSharp
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
}
