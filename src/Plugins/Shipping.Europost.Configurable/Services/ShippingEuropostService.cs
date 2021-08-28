using Grand.Domain;
using Grand.Domain.Data;
using Grand.Infrastructure.Caching;
using Shipping.Europost.Configurable.Domain;
using Shipping.Europost.Configurable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shipping.Europost.Configurable.Services
{
    public partial class ShippingEuropostService : IShippingEuropostService
    {
        #region Constants
        private const string SHIPPINGBYWEIGHT_ALL_KEY = "Grand.Europostshippingbyweight.all-{0}-{1}";
        private const string SHIPPINGBYWEIGHT_PATTERN_KEY = "Grand.Europostshippingbyweight.";
        private const string SHIPPINGBYTOTAL_ALL_KEY = "Grand.Europostshippingbytotal.all-{0}-{1}";
        private const string SHIPPINGBYTOTAL_PATTERN_KEY = "Grand.Europostshippingbytotal.";
        private const string PICKUP_POINT_PATTERN_KEY = "Grand.EuropostShippingPoint.";
        #endregion

        #region Fields

        private readonly IRepository<ShippingEuropostByWeightRecord> _sbwRepository;
        private readonly IRepository<ShippingEuropostByTotalRecord> _sbtRepository;
        private readonly IRepository<EuropostShippingPointRecord> _shippingPointRepository;
        private readonly ICacheBase _cacheBase;
        private readonly HttpClient _client;
        private readonly EuropostShippingSettings _europostShippingSettings;

        #endregion

        #region Ctor

        public ShippingEuropostService(ICacheBase cacheBase,
            IRepository<ShippingEuropostByWeightRecord> sbwRepository,
            IRepository<ShippingEuropostByTotalRecord> sbtRepository,
            IRepository<EuropostShippingPointRecord> shippingPointRepository, 
            HttpClient client,
            EuropostShippingSettings europostShippingSettings)
        {
            _cacheBase = cacheBase;
            _sbwRepository = sbwRepository;
            _sbtRepository = sbtRepository;
            _shippingPointRepository = shippingPointRepository;
            _client = client;
            _europostShippingSettings = europostShippingSettings;
        }

        #endregion

        #region Methods by weight

        public virtual async Task DeleteShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.DeleteAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingEuropostByWeightRecord>> GetAllShippingByWeightRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYWEIGHT_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            select sbw;

                return Task.FromResult(new PagedList<ShippingEuropostByWeightRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingEuropostByWeightRecord> FindShippingByWeightRecord(double weight)
        {
            var existingRates = (await GetAllShippingByWeightRecords())
                .Where(sbw => weight >= sbw.WeightFrom && weight <= sbw.WeightTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingEuropostByWeightRecord> GetShippingByWeightRecordById(string shippingByWeightRecordId)
        {
            return _sbwRepository.GetByIdAsync(shippingByWeightRecordId);
        }

        public virtual async Task InsertShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.InsertAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByWeightRecord(ShippingEuropostByWeightRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            await _sbwRepository.UpdateAsync(shippingByWeightRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYWEIGHT_PATTERN_KEY);
        }

        #endregion

        #region Methods by total

        public virtual async Task DeleteShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.DeleteAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingEuropostByTotalRecord>> GetAllShippingByTotalRecords(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYTOTAL_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbt in _sbtRepository.Table
                            select sbt;

                return Task.FromResult(new PagedList<ShippingEuropostByTotalRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingEuropostByTotalRecord> FindShippingByTotalRecord(double total)
        {
            var existingRates = (await GetAllShippingByTotalRecords())
                .Where(sbt => total >= sbt.TotalFrom && total <= sbt.TotalTo)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        public virtual Task<ShippingEuropostByTotalRecord> GetShippingByTotalRecordById(string shippingByTotalRecordId)
        {
            return _sbtRepository.GetByIdAsync(shippingByTotalRecordId);
        }

        public virtual async Task InsertShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.InsertAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingByTotalRecord(ShippingEuropostByTotalRecord shippingByTotalRecord)
        {
            if (shippingByTotalRecord == null)
                throw new ArgumentNullException(nameof(shippingByTotalRecord));

            await _sbtRepository.UpdateAsync(shippingByTotalRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGBYTOTAL_PATTERN_KEY);
        }

        #endregion

        #region ShippingPoints
        public virtual async Task<IPagedList<EuropostShippingPointRecord>> GetAllStoreShippingPoint(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from gp in _shippingPointRepository.Table
                        select gp;

            var records = query.ToList();

            //paging
            return await Task.FromResult(new PagedList<EuropostShippingPointRecord>(records, pageIndex, pageSize));
        }

        public virtual Task<EuropostShippingPointRecord> GetStoreShippingPointById(string id)
        {
            return _shippingPointRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertStoreShippingPoint(EuropostShippingPointRecord pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _shippingPointRepository.InsertAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        public virtual async Task UpdateStoreShippingPoint(EuropostShippingPointRecord pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _shippingPointRepository.UpdateAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        public virtual async Task DeleteStoreShippingPoint(EuropostShippingPointRecord pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _shippingPointRepository.DeleteAsync(pickupPoint);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }

        public async Task SyncShippingPoints()
        {
            //var url = "https://rest.eurotorg.by/10301/Json?What=Postal.OfficesOut";
            //var formContent = new StringContent(
            //    "{\"CRC\": \"\", \"Packet\": { \"MethodName\": \"Postal.OfficesOut\", \"JWT\": null, \"ServiceNumber\": \"E811AE79-DFDE-4F85-8715-DD3A8308707E\", \"Data\": { }}}",
            //    Encoding.UTF8, 
            //    "text/plain");
            var url = _europostShippingSettings.UpdateRequestUrl;
            var requestBody = _europostShippingSettings.UpdateRequestBody;
            var formContent = new StringContent(requestBody, Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync(url, formContent);

            var content = await response.Content.ReadAsStringAsync();
            var shippingPoints = JsonSerializer.Deserialize<EuropostResponse>(content).Table;

            await _shippingPointRepository.DeleteManyAsync(e => e.Id != null);
            await _shippingPointRepository.InsertManyAsync(shippingPoints);
            await _cacheBase.RemoveByPrefix(PICKUP_POINT_PATTERN_KEY);
        }
        #endregion
    }

}
