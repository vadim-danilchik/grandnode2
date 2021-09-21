using Grand.Domain;
using Shipping.Courier.Configurable.Domain;
using System.Threading.Tasks;

namespace Shipping.Courier.Configurable.Services
{
    public partial interface IShippingCourierService
    {
        Task DeleteShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord);

        Task<IPagedList<ShippingCourierByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingCourierByWeightRecord> FindShippingByWeightRecord(double weight);

        Task<ShippingCourierByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId);

        Task InsertShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord);

        Task UpdateShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord);

        Task DeleteShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord);

        Task<IPagedList<ShippingCourierByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<ShippingCourierByTotalRecord> FindShippingByTotalRecord(double total);

        Task<ShippingCourierByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId);

        Task InsertShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord);

        Task UpdateShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord);
    }

}
