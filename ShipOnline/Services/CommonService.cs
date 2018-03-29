using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models;
using ShipOnline.DataAccess;
using ShipOnline.Models.Entity;
using ShipOnline.BusinessServices;
using ShipOnline.Models.Define;

namespace ShipOnline.Service.Common
{
    public class CommonService:BaseServices
    {
        /// <summary>
        ///  GetCityList
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MstCity> GetCityList()
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstCity> results;

            results = dataAccess.GetCityList();

            return results;
        }

        public IEnumerable<MstDistrict> GetDistrictList()
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstDistrict> results;

            results = dataAccess.GetDistrictList();

            return results;
        }

        public IEnumerable<MstTown> GetTownList()
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstTown> results;

            results = dataAccess.GetTownList();

            return results;
        }

        public IEnumerable<MstDistrict> GetDistrictByCityCd(int cityCd)
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstDistrict> results;

            results = dataAccess.GetDistrictByCityCd(cityCd);

            return results;
        }

        public IEnumerable<MstTown> GetTownByDistrictCd(int cityCd, int districtCd)
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstTown> results;

            results = dataAccess.GetTownByDistrictCd(cityCd, districtCd);

            return results;
        }

        public DistrictModel getInfoDistrict(int cityCd, int districtCd)
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            DistrictModel results;

            results = dataAccess.getInfoDistrict(cityCd, districtCd);

            return results;
        }


        public IEnumerable<MstGroupArea> GetListGroupArea(int forUser)
        {
            // Declare new DataAccess object
            CommonDa dataAccess = new CommonDa();
            IEnumerable<MstGroupArea> results;

            results = dataAccess.GetListGroupArea(forUser);

            return results;
        }
    }
}