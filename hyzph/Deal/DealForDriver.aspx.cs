//
//文件名：    DealForDriver.aspx.cs
//功能描述：  司机交易
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
using Leo.Oracle;

namespace hyzph.Deal
{
    public partial class DealForDriver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //账号
            var account = Request.Params["Account"];
            //交易源ID（货源ID）
            var originId = Request.Params["OriginId"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (account == null || originId == null)
                {
                    info.Add("参数Account，OriginId不能为空！举例", "http://218.92.115.55/M_Sph/Deal/DealForDriver.aspx?Account=1F4BA283C3F95122E053A86401695122&OriginId=944");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //查询此货物是否已结束交易
                string sql =
                    string.Format("select MARK_DEAL from TB_SPH_ORDER where CODE_CARGO_SOURCE = '{0}' and CODE_USER_SECOND='{1}'", originId, account);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsDeal", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                else
                {
                    if (dt.Rows[0]["MARK_DEAL"].ToString() == "1")
                    {
                        info.Add("IsDeal", "No");
                        info.Add("Message", "网络错误，请稍后再试！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                }

                da.BeginTransaction();
                //更新乙方订单记录
                Json = UpdateSecondOrderRecord(account, originId);
                if (Json != string.Empty)
                {
                    return;
                }
                //更新货源交易标志（货源表）
                Json = UpdateCargoSourceDealMark(originId);
                if (Json != string.Empty)
                {
                    return;
                }

                ////甲方账号
                //string firstAccount = dt.Rows[0]["firstaccount"].ToString();
                ////交易金额
                //string amount = dt.Rows[0]["amount"].ToString();

                ////获取甲方交易后余额
                //sql =
                //    string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}' order by dealtime desc", firstAccount, "2");
                //dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                //if (dt.Rows.Count == 0)
                //{
                //    info.Add("IsDeal", "No");
                //    info.Add("Message", "网络错误，请稍后再试！");
                //    Json = JsonConvert.SerializeObject(info);
                //    return;
                //}
                //string firstBalance =  Convert.ToString( Convert.ToDouble(dt.Rows[0]["balance"]) - Convert.ToDouble(amount));
                ////获取乙方交易后余额
                //sql =
                //    string.Format("select * from TB_YGPH_FUND where account='{0}' and roletype='{1}' order by dealtime desc", account, "1");
                //dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                //if (dt.Rows.Count == 0)
                //{
                //    info.Add("IsDeal", "No");
                //    info.Add("Message", "网络错误，请稍后再试！");
                //    Json = JsonConvert.SerializeObject(info);
                //    return;
                //}
                //string secondBalance =  Convert.ToString( Convert.ToDouble(dt.Rows[0]["balance"]) + Convert.ToDouble(amount));                   
           
                ////插入资金交易记录
                ////甲方（货主）为支出
                //sql =
                //    string.Format("insert into TB_YGPH_FUND (account,expense,balance,dealtime,roletype,content) values('{0}','{1}','{2}',to_date('{3}','YYYY-MM-DD HH24:MI:SS'),'{4}','{5}')", firstAccount, amount, firstBalance, DateTime.Now, "1", "交易");
                //dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);
                ////乙方（司机）为收入
                //sql =
                //  string.Format("insert into TB_YGPH_FUND (account,income,balance,dealtime,roletype,content) values('{0}','{1}','{2}',to_date('{3}','YYYY-MM-DD HH24:MI:SS'),'{4}','{5}')", account, amount, secondBalance, DateTime.Now, "2", "交易");
                //dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(sql);

                info.Add("IsDeal", "Yes");
                info.Add("Message", "交易成功！");
                Json = JsonConvert.SerializeObject(info);
                da.CommitTransaction();
            }
            catch (Exception ex)
            {
                info.Add("IsDeal", "No");
                info.Add("Message", string.Format("{0}：提交数据发生异常。{1}", ex.Source, ex.Message));
                Json = JsonConvert.SerializeObject(info);
                da.RollbackTransaction();
            }
        }
        protected string Json;
        DataAccess da = (DataAccess)new Leo.Oracle.DataAccess(RegistryKey.KeyPathWlxgx);  


        #region 更新乙方订单记录
        /// <summary>
        /// 更新乙方订单记录
        /// </summary>
        /// <param name="account">乙方用户编码</param>
        /// <param name="originId">货源编码</param>
        /// <returns></returns>
        private string UpdateSecondOrderRecord(string account, string originId)
        {
            string strJson = string.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();
            //更新乙方订单记录
            string sql =
                string.Format("update TB_SPH_ORDER set DEALTIME_SECOND=to_date('{0}','YYYY-MM-DD HH24:MI:SS'),MARK_DEAL='{1}' where CODE_CARGO_SOURCE = '{2}' and CODE_USER_SECOND='{3}' ", DateTime.Now, "1", originId, account);
            da.ExecuteNonQuery(sql);    
            return strJson;
        }
        #endregion

        #region 更新货源交易标志（货源表）
        /// <summary>
        /// 更新货源交易标志（货源表）
        /// </summary>
        /// <param name="account">乙方用户编码</param>
        /// <param name="originId">货源编码</param>
        /// <returns></returns>
        private string UpdateCargoSourceDealMark(string originId)
        {
            string strJson = string.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();
            //更新乙方订单记录
            string sql =
                string.Format("update TB_DMT_CARGO set MARK_DEAL='{0}' where PKID='{1}' ", "1", originId);
            da.ExecuteNonQuery(sql);    
            return strJson;
        }
        #endregion
    }
}