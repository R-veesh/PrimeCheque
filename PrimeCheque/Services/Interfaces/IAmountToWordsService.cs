using System;

namespace PrimeCheque.Services.Interfaces
{
    public class AmountToWordsOptions
    {
        public string Prefix { get; set; } = "Sri Lanka Rupees";
        public string Suffix { get; set; } = "Only";
        public string CentsWord { get; set; } = "Cents";
        public bool UseAnd { get; set; } = true;
        public bool Uppercase { get; set; } = false;
        public string StartSymbol { get; set; } = "**";
        public string EndSymbol { get; set; } = "**";
        public int MaxLineLength { get; set; } = 80;
        public bool TwoLineFormat { get; set; } = false;
    }

    public interface IAmountToWordsService
    {
        string Convert(decimal amount, AmountToWordsOptions? options = null);
    }
}
