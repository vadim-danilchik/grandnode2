using Grand.Domain;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Shipping.Belpost.Configurable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Belpost.Configurable.Services
{
    public partial class ShippingBelpostService : IShippingBelpostService
    {
        #region Constants
        private const string SHIPPINGBYWEIGHT_ALL_KEY = "Grand.shippingbyweight.all-{0}-{1}";
        private const string SHIPPINGBYWEIGHT_PATTERN_KEY = "Grand.shippingbyweight.";
        private const string SHIPPINGBYTOTAL_ALL_KEY = "Grand.shippingbytotal.all-{0}-{1}";
        private const string SHIPPINGBYTOTAL_PATTERN_KEY = "Grand.shippingbytotal.";
        #endregion

        #region Fields

        private readonly IRepository<ShippingBelpostByWeightRecord> _sbwRepository;
        private readonly IRepository<ShippingBelpostByTotalRecord> _sbtRepository;
        private readonly ICacheBase _cacheBase;

        #endregion

        #region Ctor

        public ShippingBelpostService(ICacheBase cacheBase,
            IRepository<ShippingBelpostByWeightRecord> sbwRepository,
            IRepository<ShippingBelpostByTotalRecord> sbtRepository)
        {
            _cacheBase = cacheBase;
            _sbwRepository = sbwRepository;
            _sbtRepository = sbtRepository;
        }

        #endregion

        #region Methods by weight

        public virtual async Task DeleteShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.DeleteAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingBelpostByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYWEIGHT_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            select sbw;

                return Task.FromResult(new PagedList<ShippingBelpostByWeightRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingBelpostByWeightRecord> FindShippingByWeightRecord(double weight)
        {
            var existingRates = (await GetAllShippingByWeightRecords())
                .Where(sbw => weight >= sbw.WeightFrom && weight <= sbw.WeightTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingBelpostByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId)
        {
            return _sbwRepository.GetByIdAsync(shippingByWeightRecordId);
        }

        public virtual async Task InsertShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.InsertAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByWeightRecord(ShippingBelpostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.UpdateAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        #endregion

        #region Methods by total

        public virtual async Task DeleteShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.DeleteAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingBelpostByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYTOTAL_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbt in _sbtRepository.Table
                            select sbt;

                return Task.FromResult(new PagedList<ShippingBelpostByTotalRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingBelpostByTotalRecord> FindShippingByTotalRecord(double total)
        {
            var existingRates = (await GetAllShippingByTotalRecords())
                .Where(sbt => total >= sbt.TotalFrom && total <= sbt.TotalTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingBelpostByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId)
        {
            return _sbtRepository.GetByIdAsync(shippingByTotalRecordId);
        }

        public virtual async Task InsertShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.InsertAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByTotalRecord(ShippingBelpostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.UpdateAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        #endregion
    }

}
