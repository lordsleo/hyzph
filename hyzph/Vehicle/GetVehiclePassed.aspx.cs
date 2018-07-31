//
//文件名：    GetVehiclePassed.aspx.cs
//功能描述：  获取车辆放行信息
//创建时间：  2016/03/08
//作者：      
//修改时间：  
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

namespace hyzph.Vehicle
{
    public partial class GetVehiclePassed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //数据起始行
            var strStartRow = Request.Params["StartRow"];
            //行数
            var strCount = Request.Params["Count"];
            //车牌号
            string strVehicleNum = Request.Params["VehicleNum"];

            try
            {
                if (strStartRow == null || strCount == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "获取车辆放行信息失败！").DicInfo());
                    return;
                }

                string strSql = string.Empty;
                if (string.IsNullOrWhiteSpace(strVehicleNum))
                {
                    strSql =
                        string.Format("select * from V_CONSIGN_VEHICLE_ONLY_QUICK order by audittime desc");
                }
                else
                {
                    strSql =
                         string.Format(@"select * 
                                         from V_CONSIGN_VEHICLE_ONLY_QUICK 
                                         where vehiclenet like '%{0}%' 
                                         order by audittime desc", 
                                         strVehicleNum);
                }

                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(strSql, Convert.ToInt32(strStartRow), Convert.ToInt32(strStartRow) + Convert.ToInt32(strCount) - 1);
                if (dt.Rows.Count <= 0)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "此车未放行！").DicInfo());
                    return;
                }

                string[,] strArray = new string[dt.Rows.Count, 4];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    strArray[iRow, 0] = dt.Rows[iRow]["vehiclenet"].ToString();
                    strArray[iRow, 1] = dt.Rows[iRow]["position"].ToString();
                    strArray[iRow, 2] = dt.Rows[iRow]["storage"].ToString();
                    strArray[iRow, 3] = dt.Rows[iRow]["audittime"].ToString();
                }

                Json = JsonConvert.SerializeObject(new DicPackage(true, strArray, null).DicInfo());
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
            }
        }
        protected string Json;
    }
}