//
//文件名：    ReleaseVehicle.aspx.cs
//功能描述：  发布车源
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
    public partial class ReleaseVehicle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];
            //始发地省，市，区县
            var SFDProvince = Request.Params["SFDProvince"];
            var SFDCity = Request.Params["SFDCity"];
            //目的地区省，市，区县  
            var MDDProvince = Request.Params["MDDProvince"];
            var MDDCity = Request.Params["MDDCity"];
            //回程车标志
            var backMark = Request.Params["BackMark"];
            //优惠标志
            var favorableMark = Request.Params["FavorableMark"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || SFDProvince == null || SFDCity == null || MDDProvince == null || MDDCity == null || backMark == null || favorableMark == null)
                {
                    info.Add("参数CodeUser，SFDProvince，SFDCity，MDDProvince，MDDCity，backMark，favorableMark不能为null！举例", "http://218.92.115.55/M_Sph/Vehicle/ReleaseVehicle.aspx?CodeUser=1DA60DDD8025725AE053A864016A725A&SFDProvince=北京&SFDCity=&MDDProvince=江苏省&MDDCity=&BackMark=0&favorableMark=0");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //判断用户编码、始发省、目的省、货物类型、手机是否为空
                if (codeUser == "" || SFDProvince == "" || MDDProvince == "" || backMark == "" || favorableMark == "")
                {
                    info.Add("参数CodeUser，SFDProvince，MDDProvince，BackMark,FavorableMark必须填数据！举例", "Account=18036600293&SFDProvince=北京&MDDProvince=江苏省&BackMark=0&favorableMark=0");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //发布车源
                //string userName = "";
                //string vehicleNum = "";
                //string vehicleLen = "";
                //string vehicleType = "";
                //string tons = "";
                //string sql =
                //    string.Format("select * from TB_YGPH_USER_AUTH where account='{0}' and roleType='{1}'", account, "1");
                //var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathHmw).ExecuteTable(sql);
                ////有无会员认证
                //if (dt.Rows.Count != 0)
                //{
                //    userName = dt.Rows[0]["username"].ToString();
                //    vehicleNum = dt.Rows[0]["vehiclenum"].ToString();
                //    vehicleLen = dt.Rows[0]["vehiclelen"].ToString();
                //    vehicleType = dt.Rows[0]["vehicletype"].ToString();
                //    tons = dt.Rows[0]["tons"].ToString();
                //}



                //获取用户信息
                string sql =
                    string.Format(@"select authstate from TB_SPH_USER_AUTH where code_user='{0}' and roletype='{1}'", codeUser, "1");
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0 || Convert.ToString(dt.Rows[0]["authstate"]) != "1")
                {
                    info.Add("IsReg", "No");
                    info.Add("Message", "用户未认证！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                else
                {
                    //插入车源
                    sql =
                        string.Format("insert into TB_SPH_EMPTY_VEHICLE (sfd_province,sfd_city,mdd_province,mdd_city,mark_back,mark_favorable,code_user) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", SFDProvince, SFDCity, MDDProvince, MDDCity, backMark, favorableMark, codeUser);
                    dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                    info.Add("IsRelease", "Yes");
                    info.Add("Message", "发布成功！");
                }

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsRelease", "No");
                info.Add("Message", string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}