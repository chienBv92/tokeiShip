
using ShipOnline.Models;
using ShipOnline.Models.Entity;
using System;
using System.Web;

namespace ShipOnline.BusinessServices
{
	public class BaseServices : IDisposable
	{
		private CmnEntityModel cmnEntityModel = null;

		public CmnEntityModel CmnEntityModel
		{
			get
			{
				if ( cmnEntityModel == null )
				{
					if ( HttpContext.Current.Session[ "CmnEntityModel" ] == null )
					{
						HttpContext.Current.Session[ "CmnEntityModel" ] = new CmnEntityModel();
					}
					cmnEntityModel = (CmnEntityModel)HttpContext.Current.Session[ "CmnEntityModel" ];
				}
				return cmnEntityModel;
			}
		}

		public void Dispose()
		{
		}

	}
}