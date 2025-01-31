using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TestTasks.InternationalTradeTask;
using TestTasks.VowelCounting;
using TestTasks.WeatherFromAPI;

namespace TestTasks
{
    class Program
    {
        static async Task Main()
        {
          
                                            
            var stringProcessor = new StringProcessor();

            string str = File.ReadAllText("C:\\Users\\nastunja\\Desktop\\TestTaskForPolytechSoftware\\TestTasks\\TestTasks\\CharCounting\\StringExample.txt");
            var charCount = stringProcessor.GetCharCount(str, new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'g', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' });
            foreach (var (symbol, count) in charCount)
            {
                Console.WriteLine($"Symbol '{symbol}' is found {count} times.");
            }

            
            var commodityRepository = new CommodityRepository();
            
            try
            {
                Console.WriteLine("Import Tariff for 'Natural honey': " + commodityRepository.GetImportTariff("Natural honey"));
                Console.WriteLine("Export Tariff for 'Natural honey': " + commodityRepository.GetExportTariff("Natural honey"));
                Console.WriteLine("Import Tariff for 'Iron and steel scrap': " + commodityRepository.GetImportTariff("Iron and steel scrap"));
                Console.WriteLine("Export Tariff for 'Iron and steel scrap': " + commodityRepository.GetExportTariff("Iron and steel scrap"));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }



            var httpClient = new HttpClient();
            var weatherManager = new WeatherManager(httpClient);
            var comparisonResult = await weatherManager.CompareWeather("kyiv,ua", "lviv,ua", 4);
        }
    }
}
