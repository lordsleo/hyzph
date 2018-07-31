//
//文件名：    FindGoods.aspx.cs
//功能描述：  查找发布中货源记录（旧版接口）
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
    public partial class FindGoods : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //始发地省，市
            var SFDProvince = Request.Params["SFDProvince"];
            var SFDCity = Request.Params["SFDCity"];
            //目的地区省，市
            var MDDProvince = Request.Params["MDDProvince"];
            var MDDCity = Request.Params["MDDCity"];

            //SFDProvince = "全国";
            //SFDCity = "";
            //MDDProvince = "全国";
            //MDDCity = "";

            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (SFDProvince == null || SFDCity == null || MDDProvince == null || MDDCity == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsFind", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "http://218.92.115.55/M_Sph/Goods/FindGoods.aspx?SFDProvince=北京&SFDCity=&MDDProvince=江苏省&MDDCity=";
                    info.Add("参数SFDProvince，SFDCity，MDDProvince，MDDCity不能为空！举例", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //判断始发省、目的省是否为空
                if (SFDProvince == "" || MDDProvince == "")
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "NO";
                    info.Add("IsFind", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "SFDProvince=北京&MDDProvince=江苏省";
                    info.Add("参数SFDProvince，MDDProvince必须填数据！举例", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql = "";
                //查找匹配货源;
                if (SFDProvince == "全国" && MDDProvince == "全国")
                {
                    sql =
                        string.Format("select * from TB_DMT_CARGO where is_end ='0' order by opeartetime desc");
                }
                else if (SFDProvince != "全国" && MDDProvince == "全国")
                {
                    if (SFDCity == "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO where sfd_province like '%{0}%' and is_end ='0' order by opeartetime desc", SFDProvince);
                    }
                    else
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO where sfd_province like '%{0}%' and sfd_city like '%{1}%' and is_end ='0' order by opeartetime desc", SFDProvince, SFDCity);
                    }
                }
                else if (SFDProvince == "全国" && MDDProvince != "全国")
                {
                    if (MDDCity == "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO where mdd_province like '%{0}%' and is_end ='0' order by opeartetime desc", MDDProvince);
                    }
                    else
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO where mdd_province like '%{0}%' and mdd_city like '%{1}%' and is_end ='0' order by opeartetime desc", MDDProvince, MDDCity);
                    }
                }
                else if (SFDProvince != "全国" && MDDProvince != "全国")
                {
                    if (SFDCity == "" && MDDCity == "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO   where sfd_province like '%{0}%' and mdd_province like '%{1}%' and is_end ='0' order by opeartetime desc ", SFDProvince, MDDProvince);
                    }
                    else if (SFDCity != "" && MDDCity == "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO  where sfd_province like '%{0}%' and mdd_province like '%{1}%' and sfd_city like '%{2}%' and is_end ='0' order by opeartetime desc", SFDProvince, MDDProvince, SFDCity);
                    }
                    else if (SFDCity == "" && MDDCity != "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO  where sfd_province like '%{0}%' and mdd_province like '%{1}%' and mdd_city like '%{2}%' and is_end ='0' order by opeartetime desc", SFDProvince, MDDProvince, MDDCity);
                    }
                    else if (SFDCity != "" && MDDCity != "")
                    {
                        sql =
                            string.Format("select * from TB_DMT_CARGO  where sfd_province like '%{0}%' and mdd_province like '%{1}%' and sfd_city like '%{2}%' and mdd_city like '%{3}%' and is_end ='0' order by opeartetime desc", SFDProvince, MDDProvince, SFDCity, MDDCity);
                    }
                }

                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
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
                info.Add("Isfind", arry2);
                string[] arry3 = new string[1];
                info.Add("GoodsReleasing", ary);
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