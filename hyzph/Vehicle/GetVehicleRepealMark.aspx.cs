//
//文件名：    GetVehicleRepealMark.aspx.cs
//功能描述：  获取车源撤销标志
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
    public partial class GetVehicleRepealMark : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null)
                {
                    info.Add("IsRepeal", "No");
                    info.Add("参数CodeUser不能为空！举例", "http://218.92.115.55/M_Sph/Vehicle/GetVehicleRepealMark.aspx?CodeUser=1DA60DDD8025725AE053A864016A725");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取车源撤销标志
                string sql =
                    string.Format("select * from TB_SPH_EMPTY_VEHICLE where code_user='{0}' order by releasetime desc", codeUser);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsGet", "No");
                    info.Add("Message", "此车源未发布！");
                }
                else
                {
                    info.Add("IsGet", "Yes");
                    info.Add("RepealMark", dt.Rows[0]["mark_repeal"].ToString());
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsGet", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}