//
//文件名：    OnlineVehicles.aspx.cs
//功能描述：  网上车源（空车）记录
//创建时间：  2015/06/29
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
    public partial class OnlineVehicles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string sql =
                    string.Format(
                        "select  a.username,a.vehiclenum,a.vehiclelen,a.vehicletype,a.tons,b.mdd_province,b.mdd_city,to_char(b.releasetime,'YYYY-MM-DD HH24:MI:SS'),b.sfd_province,b.sfd_city from TB_SPH_USER_AUTH a,TB_SPH_EMPTY_VEHICLE b  where b.mark_repeal='{0}' and a.code_user=b.code_user and a.roletype='{1}' and rownum<={2} order by b.releasetime desc", "0", "1", "50");
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    var arry = 0;
                    Json = JsonConvert.SerializeObject(arry);
                    return;
                }

                var arrys = new Leo.Data.Table(dt).ToArray();
                Json = JsonConvert.SerializeObject(arrys);
            }
            catch (Exception ex)
            {
                LogTool.WriteLog(typeof(OnlineVehicles), ex);
            }
        }
        protected string Json;
    }
}