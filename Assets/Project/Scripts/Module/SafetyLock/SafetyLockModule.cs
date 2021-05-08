using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    /// <summary>
    /// 时间锁功能，需要依赖Ini存储模块
    /// </summary>
    public class SafetyLockModule : BusinessModule
    {
        public override void Create(object args = null)
        {
            base.Create(args);
        }

        /// <summary>
        /// 当前时间信息
        /// </summary>
        public YearMonthDay nowYMD;

        /// <summary>
        /// 有效时间默认为60天
        /// </summary>
        public int availableTime=60;

        /// <summary>
        /// 过期提醒事件
        /// </summary>
        public Action<bool> ExpireEvent;

        private string SafetyLock = "SafetyLock";
        private string encryptKey = "slow";//字符串加密密钥(注意：密钥只能是4位)


        public void Init()
        {
            //建立当前时间戳密钥
            int nowYear = int.Parse(DateTime.Now.ToString("yyyy"));
            int nowMouth = int.Parse(DateTime.Now.ToString("MM"));
            int nowDay = int.Parse(DateTime.Now.ToString("dd"));
            YearMonthDay yearMonthDay = new YearMonthDay() { year = nowYear, month = nowMouth, day = nowDay, unitime = ConvertDateTiemp(DateTime.Now) };
            nowYMD = yearMonthDay;
            string encryptString = Encrypt(nowYMD.unitime.ToString());

            //获取之前时间戳密钥
            string lastEncryptString = IniStorage.GetString(SafetyLock, IniStorage.SectionName.safety);
            //有的话解密比较，没有的话首次写入
            if (lastEncryptString=="")
            {
                IniStorage.WriteString("SafetyLock", IniStorage.SectionName.safety, encryptString);
                Debug.Log("Is First Enter SafetyModule 首次进入加密模块");
            }
            else
            {
                //上次的时间戳
                long lastUnitTime = long.Parse(Decrypt(lastEncryptString));
                //间隔时间，单位s
                long intervalTime = nowYMD.unitime - lastUnitTime;
                //进入判断间隔天数逻辑
                JudgeDays(intervalTime);
            }
        }

        /// <summary>
        /// 判断间隔了多少天，是否超过了设定日期
        /// </summary>
        /// <param name="intervalTime"></param>
        private void JudgeDays(long intervalTime)
        {
            //精确到秒 一天是86400秒 一个月是2626560秒 一年是31536000秒
            float intervalDay = intervalTime / 86400.0f;
            //Debug.Log("间隔天数 : " + (int)intervalDay + " 天. 设定天数为 : "+ availableTime+" 天");
            if (availableTime==0)
            {
                ExpireEvent?.Invoke(false);
                return;
            }
            if ((int)intervalDay>availableTime)
            {
                //超过使用期限
                ExpireEvent?.Invoke(true);
            }
            else
            {
                //未超过使用期限
                ExpireEvent?.Invoke(false);
            }
        }
        //============================================================================================================

        /// <summary>
        /// 获取时间戳（精确到秒）
        /// </summary>
        /// <param name="time">时间</param>
        public long ConvertDateTiemp(DateTime time)
        {
            return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            //等价于：
            //return ((time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000000) * 1000;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public DateTime GetTime(string timeStamp)
        {
            if (timeStamp.Length > 10)
            {
                timeStamp = timeStamp.Substring(0, 10);
            }
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 拿到mac地址
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            string physicalAddress = "";
            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
            physicalAddress = nice[0].GetPhysicalAddress().ToString();
            #region 这也是一种获取方式
            //foreach (NetworkInterface adaper in nice)
            //{
            //Debug.Log(adaper.Description);
            //if (adaper.Description == "en0")
            //{
            //    physicalAddress = adaper.GetPhysicalAddress().ToString();
            //    break;
            //}
            //else
            //{
            //    physicalAddress = adaper.GetPhysicalAddress().ToString();
            //    if (physicalAddress != "")
            //    {
            //        break;
            //    };
            //}
            //}
            #endregion
            return physicalAddress;
        }

        #region 加密解密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Encrypt(string str)
        {//加密字符串
            try
            {
                byte[] key = Encoding.Unicode.GetBytes(encryptKey);//密钥
                byte[] data = Encoding.Unicode.GetBytes(str);//待加密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化加密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Convert.ToBase64String(temp);//返回加密后的字符串
            }
            catch
            {
                return str;
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Decrypt(string str)
        {//解密字符串
            try
            {
                byte[] key = Encoding.Unicode.GetBytes(encryptKey);//密钥
                byte[] data = Convert.FromBase64String(str);//待解密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化解密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Encoding.Unicode.GetString(temp);//返回解密后的字符串
            }
            catch
            {
                return str;
            }
        }
        #endregion
    }

    public class YearMonthDay
    {
        public int year;
        public int month;
        public int day;
        public long unitime;
        //public DateTime datetime;
    }
}