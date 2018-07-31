//
//文件名：    ReleaseGoods.aspx.cs
//功能描述：  发布货源
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
    public partial class ReleaseGoods : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];
            //始发地省，市，区县
            var SFDProvince = Request.Params["SFDProvince"];
            var SFDCity = Request.Params["SFDCity"];
            var SFDCounty = Request.Params["SFDCounty"];
            //目的地区省，市，区县  
            var MDDProvince = Request.Params["MDDProvince"];      
            var MDDCity = Request.Params["MDDCity"];
            var MDDCounty = Request.Params["MDDCounty"];
            //货物类型
            var goods = Request.Params["Goods"];
            //重量
            var weight = Request.Params["Weight"];
            //体积（方）
            var volume = Request.Params["Volume"];
            //车长
            var vehicleLen = Request.Params["VehicleLen"];
            //车型
            var vehicleType = Request.Params["VehicleType"];
            //手机
            var mobile = Request.Params["Mobile"];
            //电话
            var phone = Request.Params["Phone"];
            //联系人
            var contact = Request.Params["Contact"];
            //距离
            var distance = Request.Params["Distance"];
         
            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || SFDProvince == null || SFDCity == null || SFDCounty == null || MDDProvince == null || MDDCity == null || MDDCounty == null || goods == null || weight == null || volume == null || vehicleLen == null || vehicleType == null || mobile == null || phone == null || contact == null || distance == null)
                {
                    info.Add("参数CodeUser，SFDProvince，SFDCity，SFDCounty，MDDProvince，MDDCity，MDDCounty，Goods，Weight，Volume，VehicleLen，VehicleType，Mobile，Phone，Contact，Distance不能为null！举例", "http://218.92.115.55/M_Sph/Goods/ReleaseGoods.aspx?CodeUser=1DA60DDD8025725AE053A864016A725A&SFDProvince=北京&SFDCity=&SFDCounty=&MDDProvince=江苏省&MDDCity=&MDDCounty=&Goods=煤炭&Weight=&Volume=&VehicleLen=&VehicleType=&Mobile=18000000000&Phone=&Contact=&Distance=");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //判断账号、始发省、目的省、货物类型、手机是否为空
                if (codeUser == "" || SFDProvince == "" || MDDProvince == "" || goods == "" || mobile == "")
                {
                    info.Add("参数CodeUser，SFDProvince，MDDProvince，Goods,Mobile必须填数据！举例", "CodeUser=1DA60DDD8025725AE053A864016A725A&SFDProvince=北京&MDDProvince=江苏省&Goods=煤炭&Mobile=18000000000");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //手机号码验证
                string message = TokenTool.VerifyMobile(mobile);
                if (message != "ture")
                {
                    info.Add("IsRelease", "No");
                    info.Add("Message", message);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取用户信息
                string sql =
                    string.Format(@"select a.usertype, a.code_company, a.iport_code_user, b.username, b.authstate 
                                    from tb_sph_user a, tb_sph_user_auth b 
                                    where a.code_user=b.code_user and a.code_user='{0}' and b.roletype='{1}'" 
                                    , codeUser, "2");
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0 || Convert.ToString(dt.Rows[0]["authstate"]) != "1")
                {
                    info.Add("IsRelease", "No");
                    info.Add("Message", "用户未认证！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //判断是否为Iport用户
                if ((Convert.ToString(dt.Rows[0]["usertype"]) == "1") && (Convert.ToString(dt.Rows[0]["code_company"]) != "null"))
                {
                    codeUser = Convert.ToString(dt.Rows[0]["iport_code_user"]);
                }

                //发布货源
                sql =
                    string.Format(@"insert into TB_DMT_CARGO (sfd_province,sfd_city,sfd_county,mdd_province,mdd_city,mdd_county,cargoname,weight,volume,vehicleLen,vehicleType,mobile,phone,diatance,code_user,code_client,opeartor,contact_man,style) 
                                    values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                    , SFDProvince, SFDCity, SFDCounty, MDDProvince, MDDCity, MDDCounty, goods, weight, volume, vehicleLen, vehicleType, mobile, phone, distance, codeUser, Convert.ToString(dt.Rows[0]["code_company"]), Convert.ToString(dt.Rows[0]["username"]), contact , "1");
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);         

                info.Add("IsRelease", "Yes");
                info.Add("Message", "发布成功！");
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsRelease", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}