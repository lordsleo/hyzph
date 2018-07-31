//
//文件名：    AuthForConsignor.aspx.cs
//功能描述：  货主会员认证
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

namespace hyzph.Auth
{
    public partial class AuthForConsignor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];
            //真实姓名
            var userName = Request.Params["UserName"];
            //身份证号
            var identityCard = Request.Params["IdentityCard"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || userName == null || identityCard == null)
                {
                    info.Add("参数CodeUser，UserName，IdentityCard不能为空！举例", "http://218.92.115.55/M_Sph/Auth/AuthForConsignor.aspx?CodeUser=1DA60DDD8025725AE053A864016A725A&UserName=张三&IdentityCard=111111111111111111");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //身份证验证
                if (!TokenTool.CheckIDCard(identityCard))
                {
                    info.Add("IsAuth", "No");
                    info.Add("Message", "身份证号码错误！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql =
                    string.Format("select authstate from TB_SPH_USER_AUTH where code_user='{0}' and roletype='{1}'", codeUser, 2);
                //验证此会员是否已认证
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    //添加认证数据
                    sql =
                        string.Format("insert into TB_SPH_USER_AUTH (code_user,username,identity_card,roletype,authstate) values('{0}','{1}','{2}','{3}','{4}')", codeUser, userName, identityCard, "2", "0");
                }
                else
                {
                    //更新认证数据
                    sql =
                        string.Format("update TB_SPH_USER_AUTH set username='{0}',identity_card='{1}',authstate='{2}',operatetime=to_date('{3}','YYYY-MM-DD HH24:MI:SS') where  code_user='{4}' and roletype={5}", userName, identityCard, "1", DateTime.Now, codeUser, 2);
                }

                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                //检查数据是否插入/更新成功  
                sql =
                    string.Format("select * from TB_SPH_USER_AUTH where  code_user='{0}' and roletype='{1}'", codeUser, "2");
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsAuth", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                ////添加初始账户资金信息(在首次认证时添加会员初始化账户资金信息)
                //sql =
                //    string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}'", account, "2");
                ////验证此会员是否已初始化账户信息
                //dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathHmw).ExecuteTable(sql);
                //if (dt.Rows.Count == 0)
                //{
                //    sql =
                //        string.Format("insert into TB_YGPH_FUND (account,roletype,income,balance,content,dealtime) values('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'))", account, "2", "0", "0", "开户", DateTime.Now);
                //    dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathHmw).ExecuteTable(sql);
                //}
                //sql =
                //    string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}'", account, "2");
                //dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathHmw).ExecuteTable(sql);
                //if (dt.Rows.Count == 0)
                //{
                //    info.Add("IsAuth", "No");
                //    info.Add("Message", "网络错误，请稍后再试！");
                //    Json = JsonConvert.SerializeObject(info);
                //    return;
                //}

                info.Add("IsAuth", "Yes");
                info.Add("Message", "审核中，请耐心等待！");
                Json = JsonConvert.SerializeObject(info);

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsAuth", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}