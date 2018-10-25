namespace MintCyclingService.Cycling
{
    public class BaiduModel
    {

        /// <summary>
        /// 省市区编号
        /// </summary>
        public class AddressIDList
        {
            /// <summary>
            /// 省
            /// </summary>
            public int ProvinceID { get; set; }

            /// <summary>
            /// 市
            /// </summary>
            public int cityID { get; set; }

            /// <summary>
            /// 区
            /// </summary>
            public int districtID { get; set; }

            /// <summary>
            /// 详细地址
            /// </summary>
            public string Address { get; set; }
        }

        /// <summary>
        /// 省市区
        /// </summary>
        public class AddressModel
        {
            public string provinceName { get; set; }

            public string city { get; set; }

            public string district { get; set; }

            public string Address { get; set; }
        }

        public class BaiduApiModel
        {
            /// <summary>
            /// 经度
            /// </summary>
            public string Longitude { get; set; }

            /// <summary>
            /// 纬度
            /// </summary>
            public string Latitude { get; set; }

        }

        public class LatLng
        {

            public string Lat { get; set; }

            public string Lng { get; set; }


        }




    }
}