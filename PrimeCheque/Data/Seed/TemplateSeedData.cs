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
                    BankName = "Bank of Ceylon",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/BOC_LK.png",
                    IsDefault = true,
                    BankId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    BankName = "Commercial Bank of Ceylon",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/CommercialBankOfCeylon_LK.png",
                    IsDefault = true,
                    BankId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    BankName = "Sampath Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/SampathBank_LK.png",
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
                    TemplateImagePath = "template_image/HattonNationalBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    BankName = "Nations Trust Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/NationsTrustBank_LK.png",
                    IsDefault = true,
                    BankId = Guid.Parse("55555555-5555-5555-5555-555555555555")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    BankName = "DFCC Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/DFCCBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("66666666-6666-6666-6666-666666666666")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("10101010-1010-1010-1010-101010101010"),
                    BankName = "Seylan Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/SeylanBank_LK.png",
                    IsDefault = true,
                    BankId = Guid.Parse("77777777-7777-7777-7777-777777777777")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("20202020-2020-2020-2020-202020202020"),
                    BankName = "Pan Asia Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/PanAsiaBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("88888888-8888-8888-8888-888888888888")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("30303030-3030-3030-3030-303030303030"),
                    BankName = "Pan Asia Bank - First Class",
                    SeriesName = "First Class Current Account",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/PanAsiaBank_FirstClass_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("99999999-9999-9999-9999-999999999999")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("40404040-4040-4040-4040-404040404040"),
                    BankName = "People's Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/PeoplesBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("a1111111-1111-1111-1111-111111111111")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("50505050-5050-5050-5050-505050505050"),
                    BankName = "NDB Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/NDB_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("b2222222-2222-2222-2222-222222222222")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("60606060-6060-6060-6060-606060606060"),
                    BankName = "Amana Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/AmanaBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("c3333333-3333-3333-3333-333333333333")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("70707070-7070-7070-7070-707070707070"),
                    BankName = "Cargills Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/CargillsBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("d4444444-4444-4444-4444-444444444444")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("80808080-8080-8080-8080-808080808080"),
                    BankName = "Union Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/UnionBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("e5555555-5555-5555-5555-555555555555")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("90909090-9090-9090-9090-909090909090"),
                    BankName = "HSBC Advance",
                    SeriesName = "Advance Current Account",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/HSBC_Advance_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("f6666666-6666-6666-6666-666666666666")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0"),
                    BankName = "Citibank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/Citibank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("f7777777-7777-7777-7777-777777777777")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"),
                    BankName = "Standard Chartered",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/StandardChartered_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("f8888888-8888-8888-8888-888888888888")
                },
                new BankTemplate
                {
                    Id = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"),
                    BankName = "Public Bank",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = defaultJsonConfig,
                    TemplateImagePath = "template_image/PublicBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("f9999999-9999-9999-9999-999999999999")
                }
            };
        }
    }
}
