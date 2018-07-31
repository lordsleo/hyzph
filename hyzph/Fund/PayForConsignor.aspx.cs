//
//文件名：    PayForConsignor.aspx.cs
//功能描述：  货主充值
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
    public partial class PayForConsignor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //账号
            var account = Request.Params["Account"];
            //充值金额
            var payAmount = Request.Params["PayAmount"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (account == null)
                {
                    info.Add("IsBalance", "No");
                    info.Add("参数Account，PayAmount不能为空！举例", "http://218.92.115.55/M_Sph/Fund/PayForConsignor.aspx?Account=18000000000&PayAmount=130");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取当前会员资金余额
                string sql =
                        string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}' order by dealtime desc", account, "2");
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsBalance", "No");
                    info.Add("Message", "此会员不存在！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                double balance = Convert.ToDouble(dt.Rows[0]["balance"]) + Convert.ToDouble(payAmount);
                
                //插入新余额
                sql =
                    string.Format("insert into TB_YGPH_FUND (account,income,balance,dealtime,roletype,content) values('{0}','{1}','{2}',to_date('{3}','YYYY-MM-DD HH24:MI:SS'),'{4}','{5}')", account, payAmount, balance, DateTime.Now, "2", "充值");
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                sql =
                    string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}' order by dealtime desc", account, "2");
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsPay", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else
                {
                    if (dt.Rows[0]["balance"].ToString() != balance.ToString())
                    {
                        info.Add("IsPay", "No");
                        info.Add("Message", "网络错误，请稍后再试！");
                    }
                    else
                    {
                        info.Add("IsPay", "Yes");
                        info.Add("Message", "充值成功！");
                    }
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsPay", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}