//
//文件名：    GetGoodsReleased.aspx.cs
//功能描述：  获取已发布货源记录
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
    public partial class GetGoodsReleased : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户编码
            var codeUser = Request.Params["CodeUser"];

            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (codeUser == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsGet", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "举例：http://218.92.115.55/M_Sph/Goods/GetGoodsReleasing.aspx?CodeUser=1DBAB54785BDB214E053A864016AB214";
                    info.Add("参数CodeUser不能为null！", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取用户信息
                string sql =
                    string.Format(@"select a.usertype, a.code_company, a.iport_code_user, b.authstate 
                                    from tb_sph_user a, tb_sph_user_auth b 
                                    where a.code_user=b.code_user and a.code_user='{0}' and b.roletype='{1}'"
                                    , codeUser, "2");
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0 || Convert.ToString(dt.Rows[0]["authstate"]) != "1")
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsGet", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "用户未认证！";
                    info.Add("Message", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                //判断是否为Iport用户
                if ((Convert.ToString(dt.Rows[0]["usertype"]) == "1") && (Convert.ToString(dt.Rows[0]["code_company"]) != "null"))
                {
                    codeUser = Convert.ToString(dt.Rows[0]["iport_code_user"]);
                }

                //获取发布中货源
                sql =
                    string.Format("select * from TB_DMT_CARGO where code_user='{0}' and is_end='{1}' order by opeartetime desc", codeUser, "1");
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);

                string[,] ary = new string[dt.Rows.Count, 17];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    ary[iRow, 0] = dt.Rows[iRow]["pkid"].ToString();
                    ary[iRow, 1] = dt.Rows[iRow]["sfd_province"].ToString();
                    ary[iRow, 2] = dt.Rows[iRow]["sfd_city"].ToString();
                    ary[iRow, 3] = dt.Rows[iRow]["sfd_county"].ToString();
                    ary[iRow, 4] = dt.Rows[iRow]["mdd_province"].ToString();
                    ary[iRow, 5] = dt.Rows[iRow]["mdd_city"].ToString();
                    ary[iRow, 6] = dt.Rows[iRow]["mdd_county"].ToString();
                    ary[iRow, 7] = dt.Rows[iRow]["cargoname"].ToString();
                    ary[iRow, 8] = dt.Rows[iRow]["weight"].ToString();
                    ary[iRow, 9] = dt.Rows[iRow]["volume"].ToString();
                    ary[iRow, 10] = dt.Rows[iRow]["vehiclelen"].ToString(); 
                    ary[iRow, 11] = dt.Rows[iRow]["vehicletype"].ToString(); 
                    ary[iRow, 12] = dt.Rows[iRow]["mobile"].ToString(); 
                    ary[iRow, 13] = dt.Rows[iRow]["phone"].ToString(); 
                    ary[iRow, 14] = dt.Rows[iRow]["opeartetime"].ToString();
                    ary[iRow, 15] = dt.Rows[iRow]["diatance"].ToString();
                    ary[iRow, 16] = dt.Rows[iRow]["contact_man"].ToString();
                }

                string[] arry2 = new string[1];
                arry2[0] = "Yes";
                info.Add("IsGet", arry2);
                info.Add("GoodsReleased", ary);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "NO";
                info.Add("IsGet", arry0);
                string[] arry1 = new string[1];
                arry1[0] = string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message);
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}