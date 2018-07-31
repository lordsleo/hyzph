//
//文件名：    DealForConsingor.aspx.cs
//功能描述：  货主交易
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
    public partial class DealForConsingor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //甲方账号(货主)
            var firstAccount = Request.Params["FirstAccount"];
            //乙方账号（司机）
            var secondAccount = Request.Params["SecondAccount"];
            //交易源ID（货源ID）
            var originId = Request.Params["OriginId"];
            //订单号
            var orderNum = Request.Params["OrderNum"];
            //始发地
            var SFD = Request.Params["SFD"];
            //目的地
            var MDD = Request.Params["MDD"];
            //交易金额
            var amount = Request.Params["Amount"];
            //保险是否购买
            var insurance = Request.Params["Insurance"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (firstAccount == null || secondAccount == null || originId == null || orderNum == null || SFD == null || MDD == null || amount == null || insurance == null)
                {
                    info.Add("参数FirstAccount，SecondAccount，OriginId，OrderNum，SFD，MDD，Amount，Insurance不能为空！举例", "http://218.92.115.55/M_Sph/Deal/DealForConsingor.aspx?FirstAccount=1DBAB54785BDB214E053A864016AB214&SecondAccount=1F4BA283C3F95122E053A86401695122&OriginId=931&OrderNum=YGPH201506251507&SFD=江苏省连云港&MDD=安徽省&Amount=100&insurance=1");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //查询此货物是否已交易
                string sql =
                    string.Format("select * from TB_SPH_ORDER where CODE_CARGO_SOURCE='{0}' and CODE_USER_FIRST='{1}' and CODE_USER_SECOND='{2}'", originId, firstAccount, secondAccount);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count != 0)
                {
                    info.Add("IsDeal", "No");
                    info.Add("Message", "此货源双方正在交易！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //插入甲方交易记录
                sql =
                    string.Format("insert into TB_SPH_ORDER (CODE_CARGO_SOURCE,sfd,mdd,amount,CODE_USER_FIRST,DEALTIME_FIRST,MARK_INSURANCE,CODE_USER_SECOND) values('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY-MM-DD HH24:MI:SS'),'{6}','{7}')", originId, SFD, MDD, amount, firstAccount, DateTime.Now, insurance, secondAccount);
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                sql =
                    string.Format("select * from TB_SPH_ORDER where CODE_CARGO_SOURCE = '{0}' and CODE_USER_FIRST='{1}' and CODE_USER_SECOND='{2}'", originId, firstAccount, secondAccount);
                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsDeal", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else
                {
                    info.Add("IsDeal", "Yes");
                    info.Add("Message", "已发起成交！");
                }        

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsDeal", "No");
                info.Add("Message", string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}