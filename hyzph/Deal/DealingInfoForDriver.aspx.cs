//
//文件名：    DealingInfoForDriver.aspx.cs
//功能描述：  获取司机正在交易的信息
//创建时间：  2015/06/25
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

namespace hyzph.Deal
{
    public partial class DealingInfoForDriver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //账号
            var account = Request.Params["Account"];

            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (account == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsDealingInfo", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "http://218.92.115.55/M_Sph/Deal/DealingInfoForDriver.aspx?Account=1F4BA283C3F95122E053A86401695122";
                    info.Add("参数Account不能为空！举例", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //查询司机正在交易的信息
                string sql =
                    string.Format("select * from TB_SPH_ORDER where CODE_USER_SECOND = '{0}' and MARK_DEAL='{1}'", account, "0");
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsDealingInfo", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "暂无交易信息！";
                    info.Add("Message", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
            
                string[,] ary = new string[dt.Rows.Count, 11];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    ary[iRow, 0] = dt.Rows[iRow]["id"].ToString();
                    ary[iRow, 1] = dt.Rows[iRow]["sfd"].ToString();
                    ary[iRow, 2] = dt.Rows[iRow]["mdd"].ToString();
                    ary[iRow, 3] = dt.Rows[iRow]["MARK_INSURANCE"].ToString();
                    ary[iRow, 4] = dt.Rows[iRow]["CODE_CARGO_SOURCE"].ToString();
                    ary[iRow, 5] = dt.Rows[iRow]["ORDERNUM"].ToString();          
                }

                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    sql = string.Format("select * from TB_DMT_CARGO where pkid='{0}'", dt.Rows[iRow]["CODE_CARGO_SOURCE"].ToString());
                    var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        string[] arry0 = new string[1];
                        arry0[0] = "NO";
                        info.Add("IsDealingInfo", arry0);
                        string[] arry1 = new string[1];
                        arry1[1] = "网络错误，请稍后再试！";
                        info.Add("Message", arry1);
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    ary[iRow, 6] = dt1.Rows[0]["cargoname"].ToString();
                    ary[iRow, 7] = dt1.Rows[0]["weight"].ToString();
                    ary[iRow, 8] = dt1.Rows[0]["volume"].ToString();
                    ary[iRow, 9] = dt1.Rows[0]["vehiclelen"].ToString();
                    ary[iRow, 10] = dt1.Rows[0]["vehicletype"].ToString();
                }

                string[] arry2 = new string[1];
                arry2[0] = "Yes";
                info.Add("IsDealingInfo", arry2);
                info.Add("DealingInfo", ary);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "NO";
                info.Add("IsDealingInfo", arry0);
                string[] arry1 = new string[1];
                arry1[0] = string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message);
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}