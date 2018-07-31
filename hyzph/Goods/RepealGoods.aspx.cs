//
//文件名：    RepealGoods.aspx.cs
//功能描述：  撤销发布中货源记录
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

namespace hyzph.Goods
{
    public partial class RepealGoods : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];
            //ID
            var id = Request.Params["ID"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || id == null)
                {
                    info.Add("IsRepeal", "No");
                    info.Add("参数Account，ID不能为空！举例", "http://218.92.115.55/M_Sph/Goods/RepealGoods.aspx?CodeUser=1DBAB54785BDB214E053A864016AB214&ID=931");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取用户信息
                string sql =
                    string.Format(@"select a.usertype, a.code_company, a.iport_code_user, b.authstate 
                                    from tb_sph_user a, tb_sph_user_auth b 
                                    where a.code_user=b.code_user and a.code_user='{0}' and b.roletype='{1}'"
                                    , codeUser, "2");
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0 || Convert.ToString(dt.Rows[0]["authstate"]) != "1")
                {
                    info.Add("IsGet", "No");
                    info.Add("Message", "用户未认证！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string SPHCodeUser = codeUser;
                //判断是否为Iport用户
                if ((Convert.ToString(dt.Rows[0]["usertype"]) == "1") && (Convert.ToString(dt.Rows[0]["code_company"]) != "null"))
                {
                    codeUser = Convert.ToString(dt.Rows[0]["iport_code_user"]);
                }

                //撤销货源记录
                Json = RepealGoodsRecord(codeUser, id);
                if (Json != string.Empty)
                {
                    return;
                }

                info.Add("IsRepeal", "Yes");
                info.Add("Message", "撤销成功！");
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsRepeal", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;

        #region 撤销货源记录
        /// <summary>
        /// 撤销货源记录
        /// </summary>
        /// <param name="codeUser">用户编码</param>
        /// <param name="id">货源编码</param>
        /// <returns>提示消息</returns>
        private string RepealGoodsRecord(string codeUser, string id)
        {
            string strJson = string.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();
            //撤销货源
            string sql =
                string.Format("update TB_DMT_CARGO set is_end='{0}' where code_user='{1}' and pkid='{2}'", "1", codeUser, id);
            var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            sql =
                string.Format("select * from TB_DMT_CARGO where code_user='{0}' and pkid='{1}'", codeUser, id);
            dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            if (dt.Rows.Count == 0)
            {
                info.Add("IsRepeal", "No");
                info.Add("Message", "网络错误，请稍后再试！");
                strJson = JsonConvert.SerializeObject(info);
                return strJson;
            }
            else
            {
                if (dt.Rows[0]["is_end"].ToString() != "1")
                {
                    info.Add("IsRepeal", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                    strJson = JsonConvert.SerializeObject(info);
                    return strJson;
                }
            }

            return strJson;
        }
        #endregion

        #region 撤销订单记录
        /// <summary>
        /// 撤销订单记录
        /// </summary>
        /// <param name="codeUser">用户编码</param>
        /// <param name="id">货源编码</param>
        /// <returns>提示消息</returns>
        private string RepealOrderRecord(string codeUser, string id)
        {
            string strJson = string.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();
            //撤销订单
            string sql =
                string.Format("update TB_SPH_ORDER set MARK_DEAL='{0}' where CODE_USER_FIRST='{1}' and CODE_CARGO_SOURCE='{2}'", "1", codeUser, id);
            var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            sql =
                string.Format("select * from TB_SPH_ORDER where CODE_USER_FIRST='{0}' and CODE_CARGO_SOURCE='{1}'", codeUser, id);
            dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
            if (dt.Rows.Count == 0)
            {
                info.Add("IsRepeal", "No");
                info.Add("Message", "网络错误，请稍后再试！");
                strJson = JsonConvert.SerializeObject(info);
                return strJson;
            }
            else
            {
                if (dt.Rows[0]["MARK_DEAL"].ToString() != "1")
                {
                    info.Add("IsRepeal", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                    strJson = JsonConvert.SerializeObject(info);
                    return strJson;
                }
            }

            return strJson;
        }
        #endregion


    }
}