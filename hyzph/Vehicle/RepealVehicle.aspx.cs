//
//文件名：    RepealVehicle.aspx.cs
//功能描述：  撤销发布中车源记录
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
    public partial class RepealVehicle : System.Web.UI.Page
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
                    info.Add("参数Account不能为空！举例", "http://218.92.115.55/M_Sph/Vehicle/RepealVehicle.aspx?CodeUser=1DA60DDD8025725AE053A864016A725");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //撤销车源
                string sql =
                    string.Format("select * from TB_SPH_EMPTY_VEHICLE where code_user='{0}' order by releasetime desc", codeUser);
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsRepeal", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else
                {
                    if (dt.Rows[0]["mark_repeal"].ToString() == "0")
                    {
                        sql =
                            string.Format("update TB_SPH_EMPTY_VEHICLE set mark_repeal='{0}' where code_user='{1}' and mark_repeal='{2}'", "1", codeUser, "0");
                        dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                        sql =
                            string.Format("select * from TB_SPH_EMPTY_VEHICLE where code_user='{0}'", codeUser);
                        dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            info.Add("IsRepeal", "No");
                            info.Add("Message", "网络错误，请稍后再试！");
                        }
                        else
                        {
                            if (Convert.ToString(dt.Rows[0]["mark_repeal"]) == "1")
                            {
                                info.Add("IsRepeal", "Yes");
                                info.Add("Message", "撤销成功！");
                            }
                            else
                            {
                                info.Add("IsRepeal", "No");
                                info.Add("Message", "网络错误，请稍后再试！");
                            }                                     
                        }
                    }
                    else if (dt.Rows[0]["mark_repeal"].ToString() == "1")
                    {
                        info.Add("IsRepeal", "No");
                        info.Add("Message", "此车源已被撤销！");
                    }
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsRepeal", "No");
                info.Add("Message", string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}