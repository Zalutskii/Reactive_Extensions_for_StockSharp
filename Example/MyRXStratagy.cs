using System;

using StockSharp.Algo.Candles;
using StockSharp.Algo.Strategies;

namespace Example
{
	using System.Linq;
	using System.Reactive.Linq;

	using ReactiveStockSharp;

	public class MyRXStratagy : Strategy
    {
        private readonly CandleSeries _candleSeries;

        public MyRXStratagy(CandleSeries candleSeries)
        {
            _candleSeries = candleSeries;
        }

        private int _length = 3;

        protected override void OnStarted()
        {
            var bufferIsFull = this.GetCandleManager().RxWhenCandlesFinished(_candleSeries).Buffer(_length, 1);

            bufferIsFull
                .Where(bufer => bufer.All(c => c.OpenPrice <= c.ClosePrice)).Where(_ => Position >= 0)
                .Subscribe(_ => RegisterOrder(this.SellAtMarket(Volume + Math.Abs(Position))));

            bufferIsFull
                .Where(bufer => bufer.All(c => c.OpenPrice >= c.ClosePrice)).Where(_ => Position <= 0)
                .Subscribe(_ => RegisterOrder(this.BuyAtMarket(Volume + Math.Abs(Position))));

            base.OnStarted();
        }
    }
}