//
//文件名：    Register.aspx.cs
//功能描述：  用户登陆
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
using YGSoft.IPort.Data;

namespace hyzph
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var account = Request.Params["Account"];
            var password = Request.Params["Password"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (account == null || password == null)
                {
                    info.Add("参数Account，Password不能为空！举例", "http://218.92.115.55/M_Sph/Login.aspx?Account=18000000000&Password=123456");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //手机登录&&会员名登录（只有Iport用户名首次登陆才会在sph用户表插入用户信息）
                if (TokenTool.VerifyMobile(account) == "ture")
                {
                    Json = GetInfoByMobileLogin(account, password);
                }//手机号登陆
                else
                {
                    Json = GetInfoByUserNameLogin(account, password);   
                }
            }
            catch (Exception ex)
            {
                info.Add("IsLogin", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }

        protected string Json;

        /// <summary>
        /// 获取通过手机登陆信息
        /// </summary>
        /// <param name="account"手机号></param>
        /// <param name="password">密码</param>
        /// <returns>返回结果</returns>
        private string GetInfoByMobileLogin(string account, string password)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            string Json = string.Empty;

            string sql =
                string.Format("select * from TB_SPH_USER where account='{0}'", account);
            //判断用户（手机号）是否存在
            var dt0 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            if (dt0.Rows.Count == 0)
            {
                info.Add("IsLogin", "No");
                info.Add("Message", "登陆账号错误！");
                Json = JsonConvert.SerializeObject(info);
                return Json;
            }

            //判断是否为Iport用户
            if (Convert.ToString(dt0.Rows[0]["usertype"]) == "1")
            {
                sql =
                    string.Format("select password from TB_SYS_USER  where upper(logogram) = '{0}'", Convert.ToString(dt0.Rows[0]["logogram"]).ToUpper());
                var dt1 = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathIport).ExecuteTable(sql);
                if (dt1.Rows.Count == 0)
                {
                    info.Add("IsLogin", "No");
                    info.Add("Message", "Iport账号可能变动，如有问题请联系客服！");
                    Json = JsonConvert.SerializeObject(info);
                    return Json;
                }
                else
                {
                    if (!Identity.VerifyText(Format.Trim(password), Convert.ToString(dt1.Rows[0]["password"])))
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "密码错误");
                        Json = JsonConvert.SerializeObject(info);
                        return Json;
                    }
                }
            }//sph用户表显示为Iport用户
            else
            {
                if (password != Convert.ToString(dt0.Rows[0]["password"]))
                {
                    info.Add("IsLogin", "No");
                    info.Add("Message", "密码错误");
                    Json = JsonConvert.SerializeObject(info);
                    return Json;
                }
            }//sph用户表显示为非Iport用户

            info.Add("IsLogin", "Yes");
            info.Add("CodeUser", Convert.ToString(dt0.Rows[0]["code_user"]));
            Json = JsonConvert.SerializeObject(info);
            return Json;
        }


        /// <summary>
        /// 获取通过用户名登陆信息
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回结果</returns>
        private string GetInfoByUserNameLogin(string account, string password)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            string Json = string.Empty;
            string codeUser = string.Empty;

            string sql =
                string.Format("select * from TB_SPH_USER where upper(logogram)='{0}'", account.ToUpper());
            //判断用户是否存在
            var dt0 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            if (dt0.Rows.Count == 0)
            {
                sql =
                    string.Format("select code_user,password,code_company from TB_SYS_USER  where upper(logogram) = '{0}'", account.ToUpper());
                var dt1 = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathIport).ExecuteTable(sql);
                if (dt1.Rows.Count == 0)
                {
                    info.Add("IsLogin", "No");
                    info.Add("Message", "登陆账号错误！");
                    Json = JsonConvert.SerializeObject(info);
                    return Json;
                }//Iport用户表不存在
                else
                {
                    if (!Identity.VerifyText(Format.Trim(password), dt1.Rows[0]["password"] as string))
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "密码错误");
                        Json = JsonConvert.SerializeObject(info);
                        return Json;
                    }

                    string mobile = string.Empty;
                    string codeCompany = string.Empty;
                    string iportCodeUser = string.Empty;
                    //获取Iport用户电话信息
                    sql =
                        string.Format("select mobile from TB_SYS_USERINFO  where code_user = '{0}'", Convert.ToString(dt1.Rows[0]["code_user"]));
                    var dt2 = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathIport).ExecuteTable(sql);
                    if (dt2.Rows.Count != 0)
                    {
                        mobile = Convert.ToString(dt2.Rows[0]["mobile"]);
                    }
                    codeCompany = Convert.ToString(dt1.Rows[0]["code_company"]);
                    iportCodeUser = Convert.ToString(dt1.Rows[0]["code_user"]);
                    


                    //查询此账号（手机号）是否已注册
                    sql =
                        string.Format("select * from TB_SPH_USER where account='{0}'", mobile);
                    //判断用户（手机号）是否存在
                    var dt3 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                    if (dt3.Rows.Count != 0)
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "您已用手机号进行注册，可以联系客服！");
                        Json = JsonConvert.SerializeObject(info);
                        return Json;
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

                    //sph用户表插入新用户信息
                    sql =
                        string.Format(@"insert into TB_SPH_USER (account, logogram, code_company, endtime, usertype, iport_code_user) 
                                        values('{0}','{1}','{2}',to_date('{3}','YYYY-MM-DD HH24:MI:SS'),'{4}','{5}')"
                                        , mobile, account, codeCompany, endTime, "1", iportCodeUser);
                    var dt4 = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                    //检查是否已插入
                    sql =
                        string.Format("select * from TB_SPH_USER where upper(logogram)='{0}'", account.ToUpper());
                    var dt5 = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                    if (dt5.Rows.Count == 0)
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "网络错误，请稍后再试！");
                        Json = JsonConvert.SerializeObject(info);
                        return Json;
                    }
                    else
                    {
                        if (Convert.ToString(dt5.Rows[0]["account"]) != mobile || Convert.ToString(dt5.Rows[0]["code_company"]) != codeCompany || Convert.ToString(dt5.Rows[0]["usertype"]) != "1" || Convert.ToString(dt5.Rows[0]["endtime"]) != endTime.ToString() || Convert.ToString(dt5.Rows[0]["iport_code_user"]) != iportCodeUser)
                        {
                            info.Add("IsLogin", "No");
                            info.Add("Message", "网络错误，请稍后再试！");
                            Json = JsonConvert.SerializeObject(info);
                            return Json;
                        }
                        {
                            codeUser = Convert.ToString(dt5.Rows[0]["code_user"]);
                        }
                    }
                }//Iport用户表存在
            }//sph用户表不存在
            else
            {
                sql =
                    string.Format("select password from TB_SYS_USER  where upper(logogram) = '{0}'", account.ToUpper());
                var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                if (dt1.Rows.Count == 0)
                {
                    info.Add("IsLogin", "No");
                    info.Add("Message", "Iport账号可能变动，请联系客服！");
                    Json = JsonConvert.SerializeObject(info);
                    return Json;
                }
                else
                {
                    if (!Identity.VerifyText(password, Convert.ToString(dt1.Rows[0]["password"])))
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "密码错误");
                        Json = JsonConvert.SerializeObject(info);
                        return Json;
                    }
                }

                    codeUser = Convert.ToString(dt0.Rows[0]["code_user"]);
                }//sph用户表存在

            info.Add("IsLogin", "Yes");
            info.Add("CodeUser", codeUser);
            Json = JsonConvert.SerializeObject(info);
            return Json;
        }
    }
}