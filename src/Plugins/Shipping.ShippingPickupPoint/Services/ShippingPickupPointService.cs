using Grand.Domain;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Shipping.ShippingPickupPoint.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint.Services
{
    public class ShippingPickupPointService : IShippingPickupPointService
    {
        #region Constants

        private const string PICKUP_POINT_PATTERN_KEY = "Grand.ShippingPickupPoint.";

        #endregion

        #region Fields

        private readonly ICacheBase _cacheBase;
        private readonly IRepository<ShippingPickupPoints> _ShippingPickupPointRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheBase">Cache base</param>
        /// <param name="ShippingPickupPointRepository">Store pickup point repository</param>
        public ShippingPickupPointService(ICacheBase cacheBase,
            IRepository<ShippingPickupPoints> ShippingPickupPointRepository)
        {
            _cacheBase = cacheBase;
            _ShippingPickupPointRepository = ShippingPickupPointRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass "" to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        public virtual async Task<IPagedList<ShippingPickupPoints>> GetAllStoreShippingPickupPoint(string storeId = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from gp in _ShippingPickupPointRepository.Table
                        where (gp.StoreId == storeId || string.IsNullOrEmpty(gp.StoreId)) || storeId == ""
                        select gp;

            var records = query.ToList();

            //paging
            return await Task.FromResult(new PagedList<ShippingPickupPoints>(records, pageIndex, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickupPointId"></param>
        /// <returns></returns>
        public virtual Task<ShippingPickupPoints> GetStoreShippingPickupPointByPointName(string pointName)
        {
            return Task.FromResult((from shippingOoint in _ShippingPickupPointRepository.Table
                    where shippingOoint.ShippingPickupPointName == pointName
                    select shippingOoint).FirstOrDefault());
        }

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        public virtual Task<ShippingPickupPoints> GetStoreShippingPickupPointById(string pickupPointId)
        {
            return _ShippingPickupPointRepository.GetByIdAsync(pickupPointId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task InsertStoreShippingPickupPoint(ShippingPickupPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _ShippingPickupPointRepository.InsertAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task UpdateStoreShippingPickupPoint(ShippingPickupPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _ShippingPickupPointRepository.UpdateAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task DeleteStoreShippingPickupPoint(ShippingPickupPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _ShippingPickupPointRepository.DeleteAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }
        #endregion
    }
}
