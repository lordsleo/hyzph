//
//文件名：    BalanceForDriver.aspx.cs
//功能描述：  获取司机资金余额
//创建时间：  2015/06/26
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

namespace hyzph.Fund
{
    public partial class BalanceForDriver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //账号
            var account = Request.Params["Account"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (account == null)
                {
                    info.Add("IsBalance", "No");
                    info.Add("参数Account不能为空！举例", "http://218.92.115.55/M_Sph/Fund/BalanceForDriver.aspx?Account=18000000000");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取司机资金余额
                string sql =
                        string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}' order by dealtime desc", account, "1");
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsBalance", "No");
                    info.Add("Message", "此会员不存在！");
                }
                else
                {
                    info.Add("IsBalance", "Yes");
                    info.Add("Balance", dt.Rows[0]["balance"].ToString());
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsBalance", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}