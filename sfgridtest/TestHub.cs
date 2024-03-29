﻿using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;
using System;

namespace sfgridtest
{
    public enum PriceType
    {
        Price,
        Bid,
        Ask
    }
    public class TestInstrument : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TestInstrument(string ticker, decimal referencePrice)
        {
            _Ticker = ticker;
            ReferencePrice = referencePrice;
        }

        public decimal ReferencePrice { get; set; }
        private string? _Ticker {  get; set; }
        private decimal? _Price { get; set; }
        private decimal? _Bid { get; set; }
        private decimal? _Ask { get; set; }
        public string? Ticker
        {
            get => _Ticker;
            set
            {
                if (value != _Ticker)
                {
                    _Ticker = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal? Price
        {
            get => _Price;
            set
            {
                if (value != _Price)
                {
                    _Price = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal? Bid
        {
            get => _Bid;
            set
            {
                if (value != _Bid)
                {
                    _Bid = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal? Ask
        {
            get => _Ask;
            set
            {
                if (value != _Ask)
                {
                    _Ask = value;
                    OnPropertyChanged();
                }
            }
        }

    }

    public class PriceDistributor
    {
        private readonly IHubContext<TestHub> _context;
        public List<TestInstrument> defaultInstruments = new List<TestInstrument>()
        {
            new TestInstrument("MSFT",390),
            new TestInstrument("APPL", 180),
            new TestInstrument("NVDA", 450),
            new TestInstrument("QQQ", 400),
            new TestInstrument("OTP", 15000),
            new TestInstrument("Test1", 1000),
            new TestInstrument("Test2", 2000),
            new TestInstrument("Test3", 3000),
            new TestInstrument("Test4", 4000),
            new TestInstrument("Test5", 5000),
            new TestInstrument("Test6", 6000),
            new TestInstrument("Test7", 7000),
            new TestInstrument("Test8", 8000),
            new TestInstrument("Test9", 9000),
            new TestInstrument("Test10", 10000)
        };

        public PriceDistributor(IHubContext<TestHub> context)
        {
            _context = context;
            foreach (var instr in defaultInstruments)
            {
                _ = Task.Run(() => GenPrice(instr, PriceType.Price));
                _ = Task.Run(() => GenPrice(instr, PriceType.Bid));
                _ = Task.Run(() => GenPrice(instr, PriceType.Ask));
            }
        }

        public async Task SendInstrumentToClient(string connectionId,TestInstrument instr)
        {
            await _context.Clients.Clients(connectionId).SendAsync("Instrument",instr);
        }

        private async Task GenPrice(TestInstrument instr, PriceType type)
        {
            while (true)
            {
                int rnd = RandomNumberGenerator.GetInt32(-5, 5 + 1);
                decimal? price = null;
                switch (type)
                {
                    case PriceType.Price:
                        instr.Price = (instr.Price ?? instr.ReferencePrice) + rnd;
                        price = instr.Price;
                        break;
                    case PriceType.Bid:
                        instr.Bid = (instr.Bid ?? instr.ReferencePrice) + rnd;
                        price = instr.Bid;
                        break;
                    case PriceType.Ask:
                        instr.Ask = (instr.Ask ?? instr.ReferencePrice) + rnd;
                        price = instr.Ask;
                        break;
                    default:
                        break;
                }
                await _context.Clients.All.SendAsync("PriceChange", instr.Ticker!, type, price);
                int delay = RandomNumberGenerator.GetInt32(100, 500);
                await Task.Delay(delay);
            }
        }
    }

    public class TestHub : Hub
    {
        private string? _connectionId { get; set; }
        private readonly PriceDistributor _distributor;

        public TestHub(PriceDistributor distributor)
        {
            _distributor = distributor;
        }
        public override async Task OnConnectedAsync()
        {
            _connectionId = Context.ConnectionId;
            _ = SendInstruments();
            await base.OnConnectedAsync();
        }

        public async Task SendInstruments()
        {
            foreach (var instr in _distributor.defaultInstruments)
            {
                int delay = RandomNumberGenerator.GetInt32(100, 500);
                await Task.Delay(delay);
                await _distributor.SendInstrumentToClient(_connectionId, instr);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionId = null;
            await base.OnDisconnectedAsync(exception);
        }

    }
}
