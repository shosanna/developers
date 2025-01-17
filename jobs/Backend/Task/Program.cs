﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NLog;

namespace ExchangeRateUpdater;

public static class Program
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private static readonly IEnumerable<Currency> currencies = new[]
    {
        new Currency("USD"),
        new Currency("EUR"),
        new Currency("CZK"),
        new Currency("JPY"),
        new Currency("KES"),
        new Currency("RUB"),
        new Currency("THB"),
        new Currency("TRY"),
        new Currency("XYZ")
    };

    public static void Main(string[] args)
    {
        try
        {
            var client = new HttpClient();
            var provider = new ExchangeRateProvider(client, _logger);
            // Starting from C# 7, the Main method can be async and return a Task
            // Awaiting the task here as we are working with an older version of C#.
            var rates = provider.GetExchangeRates(currencies).GetAwaiter().GetResult();

            Console.WriteLine($"Successfully retrieved {rates.Count()} exchange rates:");
            foreach (var rate in rates)
            {
                _logger.Info(rate.ToString());
                Console.WriteLine(rate.ToString());
            }
        }
        catch (HttpRequestException e)
        {
            // In case of an API failure, an alternative data source may be used, for example:
            // https://www.cnb.cz/en/financial-markets/foreign-exchange-market/central-bank-exchange-rate-fixing/central-bank-exchange-rate-fixing/daily.txt?date=07.02.2024
            // That would require custom parsing logic
            _logger.Error($"Network error while retrieving exchange rates: {e.Message}");
        }
        catch (FormatException e)
        {
            _logger.Error($"Data format error: {e.Message}");
        }
        catch (Exception e)
        {
            _logger.Error($"Could not retrieve exchange rates: '{e.Message}'.");
        }

        Console.ReadLine();
    }
}