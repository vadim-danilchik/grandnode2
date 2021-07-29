using Grand.Domain;
using Shipping.ShippingPickupPoint.Domain;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public interface IShippingPickupPointService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass "" to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        Task<IPagedList<ShippingPickupPoints>> GetAllStoreShippingPickupPoint(string storeId = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointName"></param>
        /// <returns></returns>
        Task<ShippingPickupPoints> GetStoreShippingPickupPointByPointName(string pointName);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="ShippingPickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        Task<ShippingPickupPoints> GetStoreShippingPickupPointById(string ShippingPickupPointId);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="ShippingPickupPoint">Pickup point</param>
        Task InsertStoreShippingPickupPoint(Domain.ShippingPickupPoints ShippingPickupPoint);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="ShippingPickupPoint">Pickup point</param>
        Task UpdateStoreShippingPickupPoint(Domain.ShippingPickupPoints ShippingPickupPoint);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="ShippingPickupPoint">Pickup point</param>
        Task DeleteStoreShippingPickupPoint(Domain.ShippingPickupPoints ShippingPickupPoint);
    }
}
