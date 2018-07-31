//
//文件名：    ModifyGoods.aspx.cs
//功能描述：  修改货源
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
    public partial class ModifyGoods : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ID
            var id = Request.Params["ID"];
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
                if (id == null || SFDProvince == null || SFDCity == null || SFDCounty == null || MDDProvince == null || MDDCity == null || MDDCounty == null || goods == null || weight == null || volume == null || vehicleLen == null || vehicleType == null || mobile == null || phone == null)
                {
                    info.Add("参数ID，SFDProvince，SFDCity，SFDCounty，MDDProvince，MDDCity，MDDCounty，Goods，Weight，Volume，VehicleLen，VehicleType，Mobile，Phone，Contact，Distance不能为null！举例", "http://218.92.115.55/M_Sph/Goods/ModifyGoods.aspx?ID=1951A407932F9040E053A86401699040&SFDProvince=北京&SFDCity=&SFDCounty=&MDDProvince=江苏省&MDDCity=&MDDCounty=&Goods=煤炭&Weight=&Volume=&VehicleLen=&VehicleType=&Mobile=18000000000&Phone=&Contact=&Distance=");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //判断ID、始发省、目的省、货物类型、手机是否为空
                if (id == "" || SFDProvince == "" || MDDProvince == "" || goods == "" || mobile == "")
                {
                    info.Add("参数ID，SFDProvince，MDDProvince，Goods,Mobile必须填数据！举例", "ID=1951A407932F9040E053A86401699040&SFDProvince=北京&MDDProvince=江苏省&Goods=煤炭&Mobile=18000000000");
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

                //判断此货源是否存在
                string sql =
                    string.Format("select * from TB_DMT_CARGO where PKID='{0}'", id);
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsModify", "No");
                    info.Add("Message", "此货源不存在！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //更新货源
                sql =
                    string.Format(@"update TB_DMT_CARGO 
                                    set sfd_province='{0}',sfd_city='{1}',sfd_county='{2}',mdd_province='{3}',mdd_city='{4}',mdd_county='{5}',cargoname='{6}',weight='{7}',volume='{8}',vehicleLen='{9}',vehicleType='{10}',mobile='{11}',phone='{12}',opeartetime=to_date('{13}','YYYY-MM-DD HH24:MI:SS'),contact_man='{14}',diatance='{15}',style='{16}' where  pkid='{17}'"
                                    , SFDProvince, SFDCity, SFDCounty, MDDProvince, MDDCity, MDDCounty, goods, weight, volume, vehicleLen, vehicleType, mobile, phone, DateTime.Now, contact, distance, "1", id);
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);

                info.Add("IsModify", "Yes");
                info.Add("Message", "发布成功！");
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsModify", "No");
                info.Add("Message", string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}