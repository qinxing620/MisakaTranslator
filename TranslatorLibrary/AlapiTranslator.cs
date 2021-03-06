﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary
{

    public class AlapiTranslator : ITranslator
    {
        private string errorInfo;//错误信息

        public string GetLastError()
        {
            return errorInfo;
        }

        public string Translate(string sourceText, string desLang, string srcLang)
        {
            if (sourceText == "" || desLang == "" || srcLang == "")
            {
                errorInfo = "Param Missing";
                return null;
            }

            if (desLang == "kr")
                desLang = "kor";
            if (srcLang == "kr")
                srcLang = "kor";
            if (desLang == "fr")
                desLang = "fra";
            if (srcLang == "fr")
                srcLang = "fra";

            // 原文
            string q = sourceText;
            string retString;

            
            string url = "https://v1.alapi.cn/api/fanyi?q=" + q + "&from=" + srcLang + "&to=" + desLang;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = null;
            request.Timeout = 6000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (WebException ex)
            {
                errorInfo = "Request Timeout";
                return null;
            }

            AliapiTransResult oinfo = JsonConvert.DeserializeObject<AliapiTransResult>(retString);

            if (oinfo.msg == "success")
            {
                string ret = "";

                //得到翻译结果
                if (oinfo.data.trans_result.Count == 1)
                {
                    for (int i = 0; i < oinfo.data.trans_result.Count; i++)
                    {
                        ret += oinfo.data.trans_result[i].dst;
                    }
                    return ret;
                }
                else
                {
                    errorInfo = "UnknownError";
                    return null;
                }
            }
            else
            {
                errorInfo = "Error:" + oinfo.msg;
                return null;
            }
        }

        public void TranslatorInit(string param1 = "", string param2 = "")
        {
            //不用初始化
        }

        class AliapiTransResult
        {
            public int code { get; set; }
            public string msg { get; set; }
            public AliapiTransData data { get; set; }
        }

        class AliapiTransData
        {
            public string from { get; set; }
            public string to { get; set; }
            public List<AliapiTransResData> trans_result { get; set; }
        }

        class AliapiTransResData
        {
            public string src { get; set; }
            public string dst { get; set; }
        }
    }
}
