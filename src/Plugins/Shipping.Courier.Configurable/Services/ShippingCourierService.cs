using Grand.Domain;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Shipping.Courier.Configurable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Courier.Configurable.Services
{
    public partial class ShippingCourierService : IShippingCourierService
    {
        #region Constants
        private const string SHIPPINGBYWEIGHT_ALL_KEY = "Grand.couriershippingbyweight.all-{0}-{1}";
        private const string SHIPPINGBYWEIGHT_PATTERN_KEY = "Grand.couriershippingbyweight.";
        private const string SHIPPINGBYTOTAL_ALL_KEY = "Grand.couriershippingbytotal.all-{0}-{1}";
        private const string SHIPPINGBYTOTAL_PATTERN_KEY = "Grand.couriershippingbytotal.";
        #endregion

        #region Fields

        private readonly IRepository<ShippingCourierByWeightRecord> _sbwRepository;
        private readonly IRepository<ShippingCourierByTotalRecord> _sbtRepository;
        private readonly ICacheBase _cacheBase;

        #endregion

        #region Ctor

        public ShippingCourierService(ICacheBase cacheBase,
            IRepository<ShippingCourierByWeightRecord> sbwRepository,
            IRepository<ShippingCourierByTotalRecord> sbtRepository)
        {
            _cacheBase = cacheBase;
            _sbwRepository = sbwRepository;
            _sbtRepository = sbtRepository;
        }

        #endregion

        #region Methods by weight

        public virtual async Task DeleteShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.DeleteAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingCourierByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYWEIGHT_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            select sbw;

                return Task.FromResult(new PagedList<ShippingCourierByWeightRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingCourierByWeightRecord> FindShippingByWeightRecord(double weight)
        {
            var existingRates = (await GetAllShippingByWeightRecords())
                .Where(sbw => weight >= sbw.WeightFrom && weight <= sbw.WeightTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingCourierByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId)
        {
            return _sbwRepository.GetByIdAsync(shippingByWeightRecordId);
        }

        public virtual async Task InsertShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.InsertAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByWeightRecord(ShippingCourierByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.UpdateAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        #endregion

        #region Methods by total

        public virtual async Task DeleteShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.DeleteAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingCourierByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYTOTAL_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbt in _sbtRepository.Table
                            select sbt;

                return Task.FromResult(new PagedList<ShippingCourierByTotalRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingCourierByTotalRecord> FindShippingByTotalRecord(double total)
        {
            var existingRates = (await GetAllShippingByTotalRecords())
                .Where(sbt => total >= sbt.TotalFrom && total <= sbt.TotalTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingCourierByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId)
        {
            return _sbtRepository.GetByIdAsync(shippingByTotalRecordId);
        }

        public virtual async Task InsertShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.InsertAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByTotalRecord(ShippingCourierByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.UpdateAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        #endregion
    }

}
