using System;

namespace ServiceInterface.Common
{
    public static class TokenTool
    {
        public static bool VerifyToken(string token)
        {
            string tk = "MV4FGbDeCY/c0E5Xh9k8Mg==";
            if (token == tk)
            {
                return true;
            }
            else { return false; }       
        }

        /// <summary>
        /// 手机号码验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string VerifyMobile(string mobile)
        {
            //判断手机号是否合理
            if (mobile.Length != 11)
            {
                return string.Format("手机号长度必须是11位！");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^((0?1[358]\d{9})|((0(10|2[1-3]|[3-9]\d{2}))?[1-9]\d{6,7}))$"))
            {
                return string.Format("手机号不存在1！");
            }

            return "ture";
        }

        /// <summary>
        /// 隐藏身份证（167***********2222）
        /// </summary>
        /// <param name="idNumber">身份证号</param>
        /// <returns></returns>
        public static string HideIDCard(string idNumber)
        {
            if (idNumber.Length == 18)
            {
                return HideIDCard18(idNumber);
            }
            else if (idNumber.Length == 15)
            {
                return HideIDCard15(idNumber);
            }
            else
            {
                return "false";
            }

        }

        /// <summary>  
        /// 18位身份证号码隐藏  
        /// </summary>  
        private static string HideIDCard18(string idNumber)
        {
            string aaa = idNumber.Substring(14, 3);
            return idNumber.Substring(0, 5) + "**********" + idNumber.Substring(15, 3);
        }
        /// <summary>  
        /// 16位身份证号码隐藏 
        /// </summary>  
        private static string  HideIDCard15(string idNumber)
        {     
            return idNumber.Substring(0, 5) + "*******" + idNumber.Substring(15, 3);
        }  
    

        /// <summary>  
        /// 验证身份证合理性  
        /// </summary>  
        /// <param name="Id">身份证号</param>  
        /// <returns></returns>  
        public static bool CheckIDCard(string idNumber)
        {
            if (idNumber.Length == 18)
            {
                bool check = CheckIDCard18(idNumber);
                return check;
            }
            else if (idNumber.Length == 15)
            {
                bool check = CheckIDCard15(idNumber);
                return check;
            }
            else
            {
                return false;
            }
        }
        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        private static bool CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }
        /// <summary>  
        /// 16位身份证号码验证  
        /// </summary>  
        private static bool CheckIDCard15(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            return true;
        }  
    }
}