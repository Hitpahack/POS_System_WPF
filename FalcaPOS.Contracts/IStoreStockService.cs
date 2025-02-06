using FalcaPOS.Entites.Stock;
using System.Data;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IStoreStockService
    {
        Task<DataSet> GetStoreStockSearch(StockSearchModel stockModelRequest);

        Task<StoreStockModelResponse> GetStockDetails(long ProductID);

    }
}
