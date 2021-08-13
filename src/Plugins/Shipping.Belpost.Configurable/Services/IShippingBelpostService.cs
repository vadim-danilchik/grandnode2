using Grand.Domain;
using Shipping.Belpost.Configurable.Domain;
using System.Threading.Tasks;

namespace Shipping.Belpost.Configurable.Services
{
    public partial interface IShippingBelpostService
    {
        Task DeleteShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord);

        Task<IPagedList<ShippingBelpostByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingBelpostByWeightRecord> FindShippingByWeightRecord(double weight);

        Task<ShippingBelpostByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId);

        Task InsertShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord);

        Task UpdateShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord);

        Task DeleteShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord);

        Task<IPagedList<ShippingBelpostByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingBelpostByTotalRecord> FindShippingByTotalRecord(double total);

        Task<ShippingBelpostByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId);

        Task InsertShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord);

        Task UpdateShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord);
    }

}
