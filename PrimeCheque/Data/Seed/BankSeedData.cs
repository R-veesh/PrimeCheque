using System;
using System.Collections.Generic;
using PrimeCheque.Models;

namespace PrimeCheque.Data.Seed
{
    public static class BankSeedData
    {
        public static List<Bank> GetInitialBanks()
        {
            return new List<Bank>
            {
                new Bank { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Bank of Ceylon", ShortName = "BOC", SwiftCode = "BCEYIKLX" },
                new Bank { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Commercial Bank of Ceylon", ShortName = "COMBANK", SwiftCode = "CCEYIKLX" },
                new Bank { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Sampath Bank", ShortName = "SAMPATH", SwiftCode = "BSAMIKLX" },
                new Bank { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Hatton National Bank", ShortName = "HNB", SwiftCode = "HNBKIKLX" },
                new Bank { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Nations Trust Bank", ShortName = "NTB", SwiftCode = "NTBKLX" },
                new Bank { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "DFCC Bank", ShortName = "DFCC", SwiftCode = "DFCCIKLX" },
                new Bank { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Seylan Bank", ShortName = "SEYLAN", SwiftCode = "SEYLIKLX" },
                new Bank { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Pan Asia Bank", ShortName = "PABC", SwiftCode = "PABKIKLX" }
            };
        }
    }
}
