//
//文件名：    FindGoods2.aspx.cs
//功能描述：  查找发布中货源记录（新版接口）
//创建时间：  2015/06/24
//作者：      
//修改时间：  2016/03/08
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Leo;
using ServiceInterface.Common;

namespace hyzph.Goods
{
    public partial class FindGoods2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //数据起始行
            var startRow = Request.Params["StartRow"];
            //行数
            var count = Request.Params["Count"];
            //始发地省
            var SFDProvince = Request.Params["SFDProvince"];
            //始发地市
            var SFDCity = Request.Params["SFDCity"];
            //始发地区
            var SFDCounty = Request.Params["SFDCounty"];
            //目的地省
            var MDDProvince = Request.Params["MDDProvince"];
            //目的地市
            var MDDCity = Request.Params["MDDCity"];
            //目的地区
            var MDDCount = Request.Params["MDDCounty"];
            //货物
            var cargo = Request.Params["Cargo"];
            //车型
            var vehicleType = Request.Params["VehicleType"];

            //SFDProvince = "江苏";
            ////SFDCity = "";
            //MDDProvince = "江苏";
            //MDDCity = "";
            //startRow = "1";
            //count = "15";
            //cargo = "铁矿砂";
          
            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (startRow == null || count == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsFind", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "举例：http://218.92.115.55/hyzph/Goods/FindGoods.aspx?StartRow=1&Count=15";
                    info.Add("参数StartRow，Count不能为null！", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string filter = string.Empty;
                //始发地省、市、区
                if (!string.IsNullOrWhiteSpace(SFDProvince) && SFDProvince != "全国")
                {
                    filter += string.Format("sfd_province like '%{0}%' and ", SFDProvince);

                    if (!string.IsNullOrWhiteSpace(SFDCity))
                    {
                        filter += string.Format("sfd_city like '%{0}%' and ", SFDCity);

                        if (!string.IsNullOrWhiteSpace(SFDCounty))
                        {
                            filter += string.Format("sfd_county like '%{0}%' and ", SFDCounty);
                        }
                    }
                }
                //目的地省、市、区
                if (!string.IsNullOrWhiteSpace(MDDProvince) && MDDProvince != "全国")
                {
                    filter += string.Format("mdd_province like '%{0}%' and ", MDDProvince);

                    if (!string.IsNullOrWhiteSpace(MDDCity))
                    {
                        filter += string.Format("mdd_city like '%{0}%' and ", MDDCity);

                        if (!string.IsNullOrWhiteSpace(MDDCount))
                        {
                            filter += string.Format("mdd_county like '%{0}%' and ", MDDCount);
                        }
                    }
                }
                //货物名称
                if (!string.IsNullOrWhiteSpace(cargo))
                {
                    filter += string.Format("cargoname='{0}' and ", cargo);
                }
                //车型
                if (!string.IsNullOrWhiteSpace(vehicleType))
                {
                    filter += string.Format("vehicletype='{0}' and ", vehicleType);
                }
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = " where " + filter + " is_end ='0'";
                }
                else
                {
                    filter = " where is_end ='0'";
                }

                string sql = 
                    string.Format(@"select * 
                                    from TB_DMT_CARGO {0} 
                                    order by opeartetime desc",
                                    filter);

                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql, Convert.ToInt32(startRow), Convert.ToInt32(startRow) + Convert.ToInt32(count) - 1);
                if (dt.Rows.Count <= 0)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsFind", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = startRow == "1" ? "暂无数据！" : "暂无更多数据！";
                    info.Add("Message", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string[,] ary = new string[dt.Rows.Count, 19];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    ary[iRow, 0] = dt.Rows[iRow]["pkid"].ToString();
                    ary[iRow, 1] = dt.Rows[iRow]["sfd_province"].ToString();
                    ary[iRow, 2] = dt.Rows[iRow]["sfd_city"].ToString();
                    ary[iRow, 3] = dt.Rows[iRow]["sfd_county"].ToString();
                    ary[iRow, 4] = dt.Rows[iRow]["mdd_province"].ToString();
                    ary[iRow, 5] = dt.Rows[iRow]["mdd_city"].ToString();
                    ary[iRow, 6] = dt.Rows[iRow]["mdd_county"].ToString();
                    ary[iRow, 7] = dt.Rows[iRow]["cargoname"].ToString();
                    ary[iRow, 8] = dt.Rows[iRow]["weight"].ToString();
                    ary[iRow, 9] = dt.Rows[iRow]["volume"].ToString();
                    ary[iRow, 10] = dt.Rows[iRow]["vehiclelen"].ToString();
                    ary[iRow, 11] = dt.Rows[iRow]["vehicletype"].ToString();
                    ary[iRow, 12] = dt.Rows[iRow]["mobile"].ToString();
                    ary[iRow, 13] = dt.Rows[iRow]["phone"].ToString();
                    ary[iRow, 14] = dt.Rows[iRow]["opeartetime"].ToString();
                    ary[iRow, 15] = dt.Rows[iRow]["diatance"].ToString();
                    ary[iRow, 16] = dt.Rows[iRow]["contact_man"].ToString();
                    ary[iRow, 17] = dt.Rows[iRow]["sfd_address"].ToString();
                    ary[iRow, 18] = dt.Rows[iRow]["mdd_address"].ToString();
                }

                string[] arry2 = new string[1];
                arry2[0] = "Yes";
                info.Add("IsFind", arry2);
                string[] arry3 = new string[1];
                info.Add("GoodsReleasing", ary);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "NO";
                info.Add("IsFind", arry0);
                string[] arry1 = new string[1];
                arry1[0] = string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message);
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}