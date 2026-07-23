using System;
using System.Collections.Generic;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Seed
{
    public static class TemplateSeedData
    {
        public static List<BankTemplate> GetInitialTemplates()
        {
            var defaultJsonConfig = @"{
  ""dateDay"": { ""x"": 152, ""y"": 12, ""width"": 12, ""height"": 6, ""fontSize"": 11 },
  ""dateMonth"": { ""x"": 164, ""y"": 12, ""width"": 12, ""height"": 6, ""fontSize"": 11 },
  ""dateYear"": { ""x"": 176, ""y"": 12, ""width"": 18, ""height"": 6, ""fontSize"": 11 },
  ""payeeLine1"": { ""x"": 35, ""y"": 25, ""width"": 150, ""height"": 7, ""fontSize"": 12 },
  ""payeeLine2"": { ""x"": 12, ""y"": 33, ""width"": 170, ""height"": 7, ""fontSize"": 12 },
  ""amountWordsLine1"": { ""x"": 12, ""y"": 42, ""width"": 165, ""height"": 7, ""fontSize"": 11 },
  ""amountWordsLine2"": { ""x"": 12, ""y"": 50, ""width"": 130, ""height"": 7, ""fontSize"": 11 },
  ""amountFigures"": { ""x"": 158, ""y"": 42, ""width"": 35, ""height"": 8, ""fontSize"": 12 },
  ""crossingZone"": { ""x"": 8, ""y"": 5, ""width"": 30, ""height"": 18 },
  ""signatureZone"": { ""x"": 130, ""y"": 65, ""width"": 60, ""height"": 15 },
  ""memoLine"": { ""x"": 12, ""y"": 70, ""width"": 100, ""height"": 6, ""fontSize"": 9 }
}";

            return new List<BankTemplate>
            {
                new BankTemplate
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BankName = "Commercial Bank of Ceylon",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    IsDefault = true,
                    BankId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    BankName = "Bank of Ceylon",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    IsDefault = true,
                    BankId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    BankName = "Sampath Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    IsDefault = true,
                    BankId = Guid.Parse("33333333-3333-3333-3333-333333333333")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    BankName = "Hatton National Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    IsDefault = true,
                    BankId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                }
            };
        }
    }
}
