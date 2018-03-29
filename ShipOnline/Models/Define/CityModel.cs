using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipOnline.Models.Entity;
using ShipOnline.Resources;

namespace ShipOnline.Models.Define
{
    public class CityModel:MstCity
    {
        public int CITY_CD_HIDDEN { get; set; }

        public List<MstCity>  ListCity = new List<MstCity>();

        public CityModel()
        {
        }
    }
}