# Reactive Extensions for StockSharp

To use, you need:
- [StockSharp][1]
- [Reactive Extensions][2]
### Strategy without Reactive Extensions
```C#
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
```
### Strategy with Reactive Extensions
```C#
public class MyRxStrategy : Strategy
{
	private readonly CandleSeries _candleSeries;

	public MyRxStrategy(CandleSeries candleSeries)
	{
		_candleSeries = candleSeries;
	}

	private int _length = 3;

	protected override void OnStarted()
	{
		var bufferIsFull = this.GetCandleManager().RxWhenCandlesFinished(_candleSeries).Buffer(_length, 1);

		bufferIsFull
			.Where(buffer => buffer.All(c => c.OpenPrice <= c.ClosePrice)).Where(_ => Position >= 0)
			.Subscribe(_ => RegisterOrder(this.SellAtMarket(Volume + Math.Abs(Position))));

		bufferIsFull
			.Where(buffer => buffer.All(c => c.OpenPrice >= c.ClosePrice)).Where(_ => Position <= 0)
			.Subscribe(_ => RegisterOrder(this.BuyAtMarket(Volume + Math.Abs(Position))));

		base.OnStarted();
	}
}
```
  [1]: https://github.com/StockSharp/StockSharp
  [2]: https://github.com/Reactive-Extensions/Rx.NET