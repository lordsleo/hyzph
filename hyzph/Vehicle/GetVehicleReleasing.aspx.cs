//
//文件名：    GetVehicleReleasing.aspx.cs
//功能描述：  获取发布中车源信息
//创建时间：  2015/06/24
//作者：      
//修改时间：  暂无
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
    public partial class GetVehicleReleasing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];

            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (codeUser == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsGet", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "举例：http://218.92.115.55/M_Sph/Vehicle/GetVehicleReleasing.aspx?CodeUser=1DA60DDD8025725AE053A864016A725";
                    info.Add("参数CodeUser不能为空！", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取发布中车源
                string sql =
                    string.Format("select * from (select * from TB_SPH_EMPTY_VEHICLE where code_user='{0}' and mark_repeal='{1}' order by releasetime desc) where rownum=1", codeUser, "0");
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsGet", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "此车源暂未发布";
                    info.Add("Message", arry1);
                }
                else
                {
                    string[] ary = new string[7];
                    ary[0] = dt.Rows[0]["id"].ToString();
                    ary[1] = dt.Rows[0]["sfd_province"].ToString();
                    ary[2] = dt.Rows[0]["sfd_city"].ToString();
                    ary[3] = dt.Rows[0]["mdd_province"].ToString();
                    ary[4] = dt.Rows[0]["mdd_city"].ToString();
                    ary[5] = dt.Rows[0]["mark_back"].ToString();
                    ary[6] = dt.Rows[0]["mark_favorable"].ToString();

                    string[] arry0 = new string[1];
                    arry0[0] = "Yes";
                    info.Add("IsGet", arry0);
                    info.Add("VehicleReleasing", ary);
                }
 
                Json = JsonConvert.SerializeObject(info);


                //string[,] ary = new string[dt.Rows.Count, 9];
                //for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                //{
                //    ary[iRow, 0] = dt.Rows[iRow]["id"].ToString();
                //    ary[iRow, 1] = dt.Rows[iRow]["sfd_province"].ToString();
                //    ary[iRow, 2] = dt.Rows[iRow]["sfd_city"].ToString();
                //    ary[iRow, 4] = dt.Rows[iRow]["mdd_province"].ToString();
                //    ary[iRow, 5] = dt.Rows[iRow]["mdd_city"].ToString();
                //    ary[iRow, 7] = dt.Rows[iRow]["backmark"].ToString();
                //    ary[iRow, 8] = dt.Rows[iRow]["favorablemark"].ToString();
                //}

                //string[] arry2 = new string[1];
                //arry2[0] = "Yes";
                //info.Add("IsGet", arry2);
                //info.Add("GoodsReleasing", ary);
                //Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "NO";
                info.Add("IsGet", arry0);
                string[] arry1 = new string[1];
                arry1[0] = string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message);
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}