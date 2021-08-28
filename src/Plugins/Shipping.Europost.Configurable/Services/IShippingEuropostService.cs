using Grand.Domain;
using Shipping.Europost.Configurable.Domain;
using System.Threading.Tasks;

namespace Shipping.Europost.Configurable.Services
{
    public partial interface IShippingEuropostService
    {
        Task DeleteShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord);

        Task<IPagedList<ShippingEuropostByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingEuropostByWeightRecord> FindShippingByWeightRecord(double weight);

        Task<ShippingEuropostByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId);

        Task InsertShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord);

        Task UpdateShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord);

        Task DeleteShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord);

        Task<IPagedList<ShippingEuropostByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingEuropostByTotalRecord> FindShippingByTotalRecord(double total);

        Task<ShippingEuropostByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId);

        Task InsertShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord);

        Task UpdateShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord);

        Task<IPagedList<EuropostShippingPointRecord>> GetAllStoreShippingPoint(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<EuropostShippingPointRecord> GetStoreShippingPointById(string id);

        Task InsertStoreShippingPoint(Domain.EuropostShippingPointRecord pickupPoint);

        Task UpdateStoreShippingPoint(Domain.EuropostShippingPointRecord pickupPoint);

        Task DeleteStoreShippingPoint(Domain.EuropostShippingPointRecord pickupPoint);

        Task SyncShippingPoints();
    }

}
