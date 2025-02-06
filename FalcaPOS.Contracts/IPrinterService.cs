namespace FalcaPOS.Contracts
{
    public interface IPrinterService
    {
        bool IsPrinterAvailable { get; }

        void PrintBarcode(string barocode, int barCodeCount, double price);

        void RefreshPrinter(bool isManualRefresh = false);

        string GetPrinterName();
    }
}
