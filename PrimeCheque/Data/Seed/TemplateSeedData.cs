using System;
using System.Collections.Generic;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Seed
{
    public static class TemplateSeedData
    {
        private static string CreateConfigJson(
            float dayX, float dayY, float monthX, float monthY, float yearX, float yearY,
            float payeeX, float payeeY, float payeeW,
            float words1X, float words1Y, float words1W,
            float words2X, float words2Y, float words2W,
            float figX, float figY, float figW,
            float crossX = 8, float crossY = 5, float memoX = 12, float memoY = 70)
        {
            return $@"{{
  ""dateD1"": {{ ""x"": {dayX}, ""y"": {dayY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateD2"": {{ ""x"": {dayX + 6}, ""y"": {dayY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateM1"": {{ ""x"": {monthX}, ""y"": {monthY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateM2"": {{ ""x"": {monthX + 6}, ""y"": {monthY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateY1"": {{ ""x"": {yearX}, ""y"": {yearY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateY2"": {{ ""x"": {yearX + 6}, ""y"": {yearY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateY3"": {{ ""x"": {yearX + 12}, ""y"": {yearY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""dateY4"": {{ ""x"": {yearX + 18}, ""y"": {yearY}, ""width"": 6, ""height"": 6, ""fontSize"": 11 }},
  ""payeeLine1"": {{ ""x"": {payeeX}, ""y"": {payeeY}, ""width"": {payeeW}, ""height"": 7, ""fontSize"": 12 }},
  ""payeeLine2"": {{ ""x"": {payeeX - 20}, ""y"": {payeeY + 8}, ""width"": {payeeW + 20}, ""height"": 7, ""fontSize"": 12 }},
  ""amountWordsLine1"": {{ ""x"": {words1X}, ""y"": {words1Y}, ""width"": {words1W}, ""height"": 7, ""fontSize"": 11 }},
  ""amountWordsLine2"": {{ ""x"": {words2X}, ""y"": {words2Y}, ""width"": {words2W}, ""height"": 7, ""fontSize"": 11 }},
  ""amountFigures"": {{ ""x"": {figX}, ""y"": {figY}, ""width"": {figW}, ""height"": 8, ""fontSize"": 12 }},
  ""crossingZone"": {{ ""x"": {crossX}, ""y"": {crossY}, ""width"": 35, ""height"": 18 }},
  ""signatureZone"": {{ ""x"": 130, ""y"": 65, ""width"": 60, ""height"": 15 }},
  ""memoLine"": {{ ""x"": {memoX}, ""y"": {memoY}, ""width"": 100, ""height"": 6, ""fontSize"": 9 }}
}}";
        }

        public static List<BankTemplate> GetInitialTemplates()
        {
            return new List<BankTemplate>
            {
                new BankTemplate
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    BankName = "Bank of Ceylon",
                    SeriesName = "Standard Current Account – Series A",
                    ChequeWidthMm = 200m,
                    ChequeHeightMm = 88m,
                    TemplateConfig = CreateConfigJson(152, 10, 164, 10, 176, 10, 32, 24, 150, 24, 38, 145, 12, 46, 150, 152, 40, 40),
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
                    TemplateConfig = CreateConfigJson(150, 11, 162, 11, 174, 11, 35, 25, 150, 26, 39, 140, 12, 47, 150, 155, 41, 38),
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
                    TemplateConfig = CreateConfigJson(154, 11, 166, 11, 178, 11, 33, 26, 150, 25, 40, 145, 12, 48, 150, 156, 42, 38),
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
                    TemplateConfig = CreateConfigJson(151, 11, 163, 11, 175, 11, 34, 25, 150, 27, 40, 140, 12, 48, 150, 154, 41, 38),
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
                    TemplateConfig = CreateConfigJson(153, 12, 165, 12, 177, 12, 36, 26, 150, 25, 40, 145, 12, 48, 150, 155, 42, 38),
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
                    TemplateConfig = CreateConfigJson(152, 11, 164, 11, 176, 11, 35, 25, 150, 26, 40, 142, 12, 48, 150, 154, 41, 38),
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
                    TemplateConfig = CreateConfigJson(150, 10, 162, 10, 174, 10, 33, 24, 150, 24, 39, 145, 12, 47, 150, 152, 40, 40),
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
                    TemplateConfig = CreateConfigJson(153, 11, 165, 11, 177, 11, 35, 25, 150, 26, 40, 140, 12, 48, 150, 155, 41, 38),
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
                    TemplateConfig = CreateConfigJson(153, 11, 165, 11, 177, 11, 35, 25, 150, 26, 40, 140, 12, 48, 150, 155, 41, 38),
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
                    TemplateConfig = CreateConfigJson(151, 10, 163, 10, 175, 10, 32, 24, 150, 25, 38, 145, 12, 46, 150, 153, 40, 40),
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
                    TemplateConfig = CreateConfigJson(152, 11, 164, 11, 176, 11, 35, 25, 150, 26, 39, 140, 12, 47, 150, 155, 41, 38),
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
                    TemplateConfig = CreateConfigJson(153, 11, 165, 11, 177, 11, 34, 25, 150, 25, 40, 145, 12, 48, 150, 154, 41, 38),
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
                    TemplateConfig = CreateConfigJson(150, 10, 162, 10, 174, 10, 33, 24, 150, 24, 38, 145, 12, 46, 150, 152, 40, 40),
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
                    TemplateConfig = CreateConfigJson(152, 11, 164, 11, 176, 11, 35, 25, 150, 26, 39, 140, 12, 47, 150, 154, 41, 38),
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
                    TemplateConfig = CreateConfigJson(154, 11, 166, 11, 178, 11, 36, 26, 150, 27, 40, 140, 12, 48, 150, 156, 42, 38),
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
                    TemplateConfig = CreateConfigJson(151, 10, 163, 10, 175, 10, 33, 24, 150, 24, 38, 145, 12, 46, 150, 153, 40, 40),
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
                    TemplateConfig = CreateConfigJson(152, 11, 164, 11, 176, 11, 35, 25, 150, 25, 40, 145, 12, 48, 150, 155, 41, 38),
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
                    TemplateConfig = CreateConfigJson(151, 10, 163, 10, 175, 10, 34, 25, 150, 26, 39, 140, 12, 47, 150, 153, 40, 40),
                    TemplateImagePath = "template_image/PublicBank_LK.jpg",
                    IsDefault = true,
                    BankId = Guid.Parse("f9999999-9999-9999-9999-999999999999")
                }
            };
        }
    }
}
