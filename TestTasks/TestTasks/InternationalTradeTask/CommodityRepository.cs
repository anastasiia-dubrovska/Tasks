using System;
using System.Collections.Generic;
using System.Linq;
using TestTasks.InternationalTradeTask.Models;

namespace TestTasks.InternationalTradeTask
{
    public class CommodityRepository
    {
        private readonly FullySpecifiedCommodityGroup[] _allCommodityGroups = new FullySpecifiedCommodityGroup[]
        {
            new FullySpecifiedCommodityGroup("06", "Sugar, sugar preparations and honey", 0.05, 0)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("061", "Sugar and honey")
                    {
                        SubGroups = new CommodityGroup[]
                        {
                            new CommodityGroup("0611", "Raw sugar,beet & cane"),
                            new CommodityGroup("0612", "Refined sugar & other prod.of refining,no syrup"),
                            new CommodityGroup("0615", "Molasses", 0, 0),
                            new CommodityGroup("0616", "Natural honey", 0, 0),
                            new CommodityGroup("0619", "Sugars & syrups nes incl.art.honey & caramel"),
                        }
                    },
                    new CommodityGroup("062", "Sugar confy, sugar preps. Ex chocolate confy", 0, 0)
                }
            },
            new FullySpecifiedCommodityGroup("282", "Iron and steel scrap", 0, 0.1)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("28201", "Iron/steel scrap not sorted or graded"),
                    new CommodityGroup("28202", "Iron/steel scrap sorted or graded/cast iron"),
                    new CommodityGroup("28203", "Iron/steel scrap sort.or graded/tinned iron"),
                    new CommodityGroup("28204", "Rest of 282.0")
                }
            }
        };

        public double GetImportTariff(string commodityName)
        {
            var commodity = FindCommodity(commodityName);
            if (commodity == null)
                throw new ArgumentException($"Commodity with name {commodityName} does not exist.");

            return FindTariff(commodity, c => c.ImportTarif);
        }

        public double GetExportTariff(string commodityName)
        {
            var commodity = FindCommodity(commodityName);
            if (commodity == null)
                throw new ArgumentException($"Commodity with name {commodityName} does not exist.");

            return FindTariff(commodity, c => c.ExportTarif);
        }

        private ICommodityGroup FindCommodity(string name)
        {
            return _allCommodityGroups.SelectMany(GetAllGroups).FirstOrDefault(c => c.Name == name);
        }

        private IEnumerable<ICommodityGroup> GetAllGroups(ICommodityGroup group)
        {
            yield return group;
            foreach (var subGroup in group.SubGroups ?? Array.Empty<ICommodityGroup>())
            {
                foreach (var sub in GetAllGroups(subGroup))
                    yield return sub;
            }
        }

        private double FindTariff(ICommodityGroup commodity, Func<ICommodityGroup, double?> tariffSelector)
        {
            while (commodity != null)
            {
                var tariff = tariffSelector(commodity);
                if (tariff.HasValue)
                    return tariff.Value;

                commodity = _allCommodityGroups.FirstOrDefault(g => g.SubGroups.Contains(commodity));
            }

            return 0; 
        }
    }
}
