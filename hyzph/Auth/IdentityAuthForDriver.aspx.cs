//
//文件名：    IdentityAuthForDriver.aspx.cs
//功能描述：  司机身份认证
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

namespace hyzph.Auth
{
    public partial class IdentityAuthForDriver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //手机号码
            var mobile = Request.Params["Mobile"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (mobile == null)
                {
                    info.Add("IsAuth", "No");
                    info.Add("参数Mobile不能为null！", "举例：http://218.92.115.55/M_Sph/Auth/IdentityAuthForDriver.aspx?mobile=18000000000");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //手机号码验证
                string message = TokenTool.VerifyMobile(mobile);
                if (message != "ture")
                {
                    info.Add("IsAuth", "No");
                    info.Add("Message", message);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql =
                    string.Format(@"select b.username,b.identity_card,b.vehiclenum,b.vehiclelen,b.vehicletype,b.tons,b.auditmark_identity,b.auditmark_driving,b.auditmark_vehicle,a.account,a.logogram,a.code_user 
                                    from TB_SPH_USER a, TB_SPH_USER_AUTH b  
                                    where a.code_user=b.code_user and b.roletype='1' and a.account='{0}' "
                                    , mobile);
                //验证此会员是否已认证
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsInfo", "No");
                    info.Add("Message", "用户未认证！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                info.Add("IsAuth", "Yes");
                info.Add("UserName", dt.Rows[0]["username"].ToString());
                info.Add("IdentityCard", TokenTool.HideIDCard(dt.Rows[0]["identity_card"].ToString()));
                info.Add("VehicleNum", dt.Rows[0]["vehiclenum"].ToString());
                info.Add("VehicleLen", dt.Rows[0]["vehiclelen"].ToString());
                info.Add("VehicleType", dt.Rows[0]["vehicletype"].ToString());
                info.Add("Tons", dt.Rows[0]["tons"].ToString());
                info.Add("AuditMarkIdentity", dt.Rows[0]["auditmark_identity"].ToString());
                info.Add("AuditMarkDriving", dt.Rows[0]["auditmark_driving"].ToString());
                info.Add("AuditMarkVehicle", dt.Rows[0]["auditmark_vehicle"].ToString());
                info.Add("CodeUser", dt.Rows[0]["code_user"].ToString());

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