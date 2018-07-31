//
//文件名：    FindGoodsByKeyword.aspx.cs
//功能描述：  查找发布中货源记录,通过关键字
//创建时间：  2016/03/08
//作者：      
//修改时间：  
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
    public partial class FindGoodsByKeyword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //数据起始行
            var strStartRow = Request.Params["StartRow"];
            //行数
            var strCount = Request.Params["Count"];
            //关键字
            string strKeyWord = Request.Params["KeyWord"];

            //strStartRow = "1";
            //strCount = "10";
            //strKeyWord = "江苏";


            try
            {
                if (strStartRow == null || strCount == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "查找货源失败！").DicInfo());
                    return;
                }

                if (string.IsNullOrWhiteSpace(strKeyWord))
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "请输入关键字！").DicInfo());
                    return;
                }

                string strSql =
                    string.Format(@"select distinct * 
                                    from TB_DMT_CARGO t 
                                    where t.sfd_province like '%{0}%' or t.sfd_city like '%{0}%' or t.sfd_county like '%{0}%' or t.sfd_address like '%{0}% ' 
                                    or t.mdd_province like '%{0}%' or t.mdd_city like '%{0}%' or t.mdd_county like '%{0}%' or t.mdd_address like '%{0}% ' 
                                    or t.cargoname like '%{0}%' or t.vehicletype like '%{0}%' or t.contact_man like '%{0}%' or t.mobile like '%{0}%'
                                    and is_end ='0' 
                                    order by opeartetime desc",
                                    strKeyWord);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(strSql, Convert.ToInt32(strStartRow), Convert.ToInt32(strStartRow) + Convert.ToInt32(strCount) - 1);
                if (dt.Rows.Count <= 0)
                {
                    string strWarning = strStartRow == "1" ? "暂无数据！" : "暂无更多数据！";
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, strWarning).DicInfo());
                    return;
                }

                string[,] strArray = new string[dt.Rows.Count, 19];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    strArray[iRow, 0] = dt.Rows[iRow]["pkid"].ToString();
                    strArray[iRow, 1] = dt.Rows[iRow]["sfd_province"].ToString();
                    strArray[iRow, 2] = dt.Rows[iRow]["sfd_city"].ToString();
                    strArray[iRow, 3] = dt.Rows[iRow]["sfd_county"].ToString();
                    strArray[iRow, 4] = dt.Rows[iRow]["mdd_province"].ToString();
                    strArray[iRow, 5] = dt.Rows[iRow]["mdd_city"].ToString();
                    strArray[iRow, 6] = dt.Rows[iRow]["mdd_county"].ToString();
                    strArray[iRow, 7] = dt.Rows[iRow]["cargoname"].ToString();
                    strArray[iRow, 8] = dt.Rows[iRow]["weight"].ToString();
                    strArray[iRow, 9] = dt.Rows[iRow]["volume"].ToString();
                    strArray[iRow, 10] = dt.Rows[iRow]["vehiclelen"].ToString();
                    strArray[iRow, 11] = dt.Rows[iRow]["vehicletype"].ToString();
                    strArray[iRow, 12] = dt.Rows[iRow]["mobile"].ToString();
                    strArray[iRow, 13] = dt.Rows[iRow]["phone"].ToString();
                    strArray[iRow, 14] = dt.Rows[iRow]["opeartetime"].ToString();
                    strArray[iRow, 15] = dt.Rows[iRow]["diatance"].ToString();
                    strArray[iRow, 16] = dt.Rows[iRow]["contact_man"].ToString();
                    strArray[iRow, 17] = dt.Rows[iRow]["sfd_address"].ToString();
                    strArray[iRow, 18] = dt.Rows[iRow]["mdd_address"].ToString();
                }

                Json = JsonConvert.SerializeObject(new DicPackage(true, strArray, null).DicInfo());
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：获取数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
            }
        }
        protected string Json;
    }
}