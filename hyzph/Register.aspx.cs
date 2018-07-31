//
//文件名：    Register.aspx.cs
//功能描述：  用户注册
//创建时间：  2015/06/23
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

namespace hyzph
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var mobile = Request.Params["Mobile"];
            var password1 = Request.Params["Password1"];
            var password2 = Request.Params["Password2"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (mobile == null || password1 == null || password2 == null)
                {
                    info.Add("参数Mobile，Password1，Password2不能为空！举例", "http://218.92.115.55/M_Sph/Register.aspx?Mobile=18000000000&Password1=123456&Password2=123456");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //判断是否已注册
                string sql =
                    string.Format("select * from TB_SPH_USER  where account='{0}'", mobile);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count != 0)
                {
                    info.Add("IsReg", "No");
                    info.Add("Message", "手机号已注册！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //手机号码验证
                string message = TokenTool.VerifyMobile(mobile);
                if (message != "ture")
                {
                    info.Add("IsReg", "No");
                    info.Add("Message", message);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //判断密码是的合理
                if (password1 != password2)
                {
                    info.Add("IsReg", "No");
                    info.Add("Message", "密码不一致");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //App使用截止时间
                string timeType = "MONTH";
                int timeInterval = 3;
                DateTime endTime;
                switch (timeType)
                {
                    case "DAY":
                        endTime = DateTime.Now.AddDays(timeInterval);
                        break;
                    case "MONTH":
                        endTime = DateTime.Now.AddMonths(timeInterval);
                        break;
                    case "YEAR":
                        endTime = DateTime.Now.AddYears(timeInterval);
                        break;
                    default:
                        throw new Exception("错误的对象索引");
                }

                //注册
                sql =
                    string.Format(@"insert into TB_SPH_USER (account, password, endtime) 
                                    values ('{0}', '{1}',  to_date('{2}','YYYY-MM-DD HH24:MI:SS'))"
                                    , mobile,  password1, endTime);
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);                        
                //检查是否已插入
                sql =
                    string.Format(@"select * 
                                    from TB_SPH_USER 
                                    where account='{0}' and password='{1}' and endtime=to_date('{2}','YYYY-MM-DD HH24:MI:SS')"
                                    , mobile, password1, endTime);
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsReg", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else
                {

                    if (Convert.ToString(dt.Rows[0]["account"]) != mobile || Convert.ToString(dt.Rows[0]["password"]) != password1 || Convert.ToString(dt.Rows[0]["endtime"]) != endTime.ToString())
                    {
                        info.Add("IsReg", "No");
                        info.Add("Message", "网络错误，请稍后再试！");
                    }
                    else
                    {
                        info.Add("IsReg", "Yes");
                        info.Add("Message", "注册成功！");
                    }
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsReg", "No");
                info.Add("Message", string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}