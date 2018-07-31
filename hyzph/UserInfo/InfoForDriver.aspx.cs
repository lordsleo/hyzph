//
//文件名：    InfoForDriver.aspx.cs
//功能描述：  司机信息
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

namespace hyzph.UserInfo
{
    public partial class InfoForDriver : System.Web.UI.Page
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
                    info.Add("IsInfo", "No");
                    info.Add("参数CodeUser不能为null！", "举例：http://218.92.115.55/M_Sph/UserInfo/InfoForDriver.aspx?CodeUser=1DBAB54785BDB214E053A864016AB214");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql =
                    string.Format("select * from TB_SPH_USER_AUTH where code_user='{0}' and roletype='{1}'", codeUser, "1");
                //验证此会员是否已认证
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsInfo", "No");
                    info.Add("Message", "用户未认证！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //查询司机信息
                sql =
                    string.Format(@"select b.username,b.identity_card,b.vehiclenum,b.vehiclelen,b.vehicletype,b.tons,b.auditmark_identity,b.auditmark_driving,b.auditmark_vehicle,a.account,a.logogram
                                    from tb_sph_user a, tb_sph_user_auth b 
                                    where a.code_user=b.code_user and a.code_user='{0}' and b.roletype='{1}'"
                                    , codeUser, "1");
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsInfo", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else
                {
                    info.Add("IsInfo", "Yes");
                    info.Add("UserName", dt.Rows[0]["username"].ToString());
                    info.Add("IdentityCard", TokenTool.HideIDCard(dt.Rows[0]["identity_card"].ToString()));
                    info.Add("VehicleNum", dt.Rows[0]["vehiclenum"].ToString());
                    info.Add("VehicleLen", dt.Rows[0]["vehiclelen"].ToString());
                    info.Add("VehicleType", dt.Rows[0]["vehicletype"].ToString());
                    info.Add("Tons", dt.Rows[0]["tons"].ToString());
                    info.Add("AuditMarkIdentity", dt.Rows[0]["auditmark_identity"].ToString());
                    info.Add("AuditMarkDriving", dt.Rows[0]["auditmark_driving"].ToString());
                    info.Add("AuditMarkVehicle", dt.Rows[0]["auditmark_vehicle"].ToString());
                    info.Add("Mobile", dt.Rows[0]["account"].ToString());
                    info.Add("Logogram", dt.Rows[0]["logogram"].ToString());
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsInfo", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}