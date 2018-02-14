using System;
using System.Windows;
using StockSharp.BusinessEntities;
using StockSharp.Algo.Candles;
using StockSharp.Algo.Strategies;
using StockSharp.Messages;
using StockSharp.Xaml.Charting;
using Ecng.Common;

using StockSharp.Algo.Storages;
using StockSharp.Algo.Testing;
using ReactiveStockSharp;
using System.Reactive.Linq;

namespace Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HistoryEmulationConnector _connector;
        private ChartCandleElement _candleElement;
        private ChartTradeElement _tradesElem;
        private CandleSeries _candleSeries;
        private Security _security;
        private Portfolio _portfolio;
        private CandleManager _candleManager;

        private Strategy _strategy;
        private readonly string _hystoryPath = @"..\..\..\History\".ToFullPath();

        public MainWindow()
        {
            InitializeComponent();

            CandleSettingsEditor.Settings = new CandleSeries
            {
                CandleType = typeof(TimeFrameCandle),
                Arg = TimeSpan.FromMinutes(5),
            };
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

            _security = new Security
            {
                Id = "SBER@TQBR",
                Code = "SBER",
                PriceStep = 0.01m,
                Board = ExchangeBoard.Micex,
            };
            _portfolio = new Portfolio { Name = "test account", BeginValue = 1000000 };
            var storageRegistry = new StorageRegistry
            {
                DefaultDrive = new LocalMarketDataDrive(_hystoryPath),
            };

            _connector = new HistoryEmulationConnector(new[] { _security }, new[] { _portfolio })
            {
                HistoryMessageAdapter =
                {
                    StorageRegistry = storageRegistry,
                    StorageFormat = StorageFormats.Csv,
                    StartDate = new DateTime(2017, 10, 01).ChangeKind(DateTimeKind.Utc),
                    StopDate = new DateTime(2017, 10, 31).ChangeKind(DateTimeKind.Utc),

                },
            };

            _candleSeries = new CandleSeries(CandleSettingsEditor.Settings.CandleType, _security,
                CandleSettingsEditor.Settings.Arg)
            {
                BuildCandlesMode = BuildCandlesModes.Load,
                BuildCandlesFrom = MarketDataTypes.Trades,
            };

            InitChart();

            _connector.RxNewSecurity().Where(sec => sec == _security).Subscribe(Connector_RxNewSecurity);

            _connector.Connect();
        }

        private void InitChart()
        {
            Chart.ClearAreas();

            var area = new ChartArea();
            _candleElement = new ChartCandleElement();
            _tradesElem = new ChartTradeElement { FullTitle = "Trade" };

            Chart.AddArea(area);
            Chart.AddElement(area, _candleElement);
            Chart.AddElement(area, _tradesElem);
        }

        private void Connector_RxNewSecurity(Security security)
        {
            _connector.RegisterTrades(security);
            _candleManager = new CandleManager(_connector);
            _candleManager.RxWhenCandlesChanged(_candleSeries).Subscribe(CandleManager_RxWhenCandlesChanged);

            _strategy = new MyStratagy(_candleSeries)
            {
                Security = _security,
                Connector = _connector,
                Portfolio = _portfolio,
            };
            _strategy.SetCandleManager(_candleManager);

            _strategy.RxNewMyTrade().Subscribe(MyTradeGrid.Trades.Add);
            _strategy.RxNewMyTrade().Subscribe(FirstStrategy_NewMyTrade);

            _strategy.Start();

            _candleManager.Start(_candleSeries);
            _connector.Start();
        }

        private void FirstStrategy_NewMyTrade(MyTrade myTrade)
        {
            var data = new ChartDrawData();
            data.Group(myTrade.Trade.Time)
                .Add(_tradesElem, myTrade);
            Chart.Draw(data);
        }

        private void CandleManager_RxWhenCandlesChanged(Candle candle)
        {
            Chart.Draw(_candleElement, candle);
        }
    }
}
