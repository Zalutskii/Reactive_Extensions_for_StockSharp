using System;
using StockSharp.Algo.Candles;
using StockSharp.Algo.Strategies;
using StockSharp.Messages;

namespace Example
{
	public class MyStrategy : Strategy
	{
		private readonly CandleSeries _candleSeries;

		public MyStrategy(CandleSeries candleSeries)
		{
			_candleSeries = candleSeries;
		}

		private int _bullLength;
		private int _bearLength;
		private int _length = 3;

		protected override void OnStarted()
		{
			this.GetCandleManager().Processing += _candleManager_Processing;

			base.OnStarted();
		}

		private void _candleManager_Processing(CandleSeries candleSeries, Candle candle)
		{
			if (candleSeries != _candleSeries)
				return;
			if (candle.State != CandleStates.Finished)
				return;

			if (candle.OpenPrice <= candle.ClosePrice)
			{
				_bullLength++;
				_bearLength = 0;
			}
			else if (candle.OpenPrice >= candle.ClosePrice)
			{
				_bullLength = 0;
				_bearLength++;
			}

            if (_bullLength >= _length && Position >= 0)
			{
				RegisterOrder(this.SellAtMarket(Volume + Math.Abs(Position)));
			}

			else if (_bearLength >= _length && Position <= 0)
			{
				RegisterOrder(this.BuyAtMarket(Volume + Math.Abs(Position)));
			}
		}
	}
}