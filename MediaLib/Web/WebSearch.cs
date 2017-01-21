using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaLib
{
    namespace Web
    {
        class Keyword
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string Link { get; set; }
        }

        class HttpState
        {
            private string _statusDescription;

            public string StatusDescription
            {
                get { return _statusDescription; }
                set { _statusDescription = value; }
            }

            /// <summary>
            /// 回调 址址, 登陆测试中使用
            /// </summary>
            private string _callBackUrl;

            public string CallBackUrl
            {
                get { return _callBackUrl; }
                set { _callBackUrl = value; }
            }


            /// <summary>
            /// 网页网址 绝对路径格式
            /// </summary>
            private string _url;

            public string Url
            {
                get { return _url; }
                set { _url = value; }
            }

            /// <summary>
            /// 字符串的形式的Cookie信息
            /// </summary>
            private string _cookies;

            public string Cookies
            {
                get { return _cookies; }
                set { _cookies = value; }
            }

            /// <summary>
            /// Cookie信息
            /// </summary>
            private CookieContainer _cookieContainer = new CookieContainer();

            public CookieContainer CookieContainer
            {
                get { return _cookieContainer; }
                set { _cookieContainer = value; }
            }

            /// <summary>
            /// 网页源码
            /// </summary>
            private string _html;

            public string Html
            {
                get { return _html; }
                set { _html = value; }
            }

            /// <summary>
            /// 验证码临时文件(绝对路径)
            /// </summary>
            private string _tmpValCodePic;

            public string TmpValCodePic
            {
                get { return _tmpValCodePic; }
                set { _tmpValCodePic = value; }
            }

            /// <summary>
            /// 验证码临时文件名(相对路径)
            /// </summary>
            private string _tmpValCodeFileName = "emptyPic.gif";

            public string TmpValCodeFileName
            {
                get { return _tmpValCodeFileName; }
                set { _tmpValCodeFileName = value; }
            }

            /// <summary>
            /// 有验证码
            /// </summary>
            private bool _isValCode;

            public bool IsValCode
            {
                get { return _isValCode; }
                set { _isValCode = value; }
            }

            /// <summary>
            /// 验证码URL
            /// </summary>
            private string _valCodeURL;

            public string ValCodeURL
            {
                get { return _valCodeURL; }
                set { _valCodeURL = value; }
            }

            /// <summary>
            /// 验证码识别后的值
            /// </summary>
            private string _valCodeValue;

            public string ValCodeValue
            {
                get { return _valCodeValue; }
                set { _valCodeValue = value; }
            }

            /// <summary>
            /// 其它参数
            /// </summary>
            private Hashtable _otherParams = new Hashtable();

            public Hashtable OtherParams
            {
                get { return _otherParams; }
                set { _otherParams = value; }
            }

            // 重复添加处理 add by fengcj  09/11/19 PM
            public void addOtherParam(object key, object value)
            {
                if (!this.OtherParams.ContainsKey(key))
                    this.OtherParams.Add(key, value);
                else
                {
                    this.OtherParams[key] = value;
                }
            }

            public void removeOtherParam(object key)
            {
                this.OtherParams.Remove(key);
            }

            public object getOtherParam(object key)
            {
                return this.OtherParams[key];
            }
        }


        class webSearch
        {
            public void baidu_Click(String keyword)
            {
                int num = 100;//搜索条数
                string url = "http://www.baidu.com/s?wd=" + keyword + "&rn=" + num + "";
                string html = search(url, "utf-8");
                BaiduSearch baidu = new BaiduSearch();
                if (!string.IsNullOrEmpty(html))
                {
                    int count = baidu.GetSearchCount(html);//搜索条数
                    if (count > 0)
                    {
                        List<Keyword> keywords = baidu.GetKeywords(html, keyword);
                    }

                }
            }
            /// <summary>
            /// 搜索处理
            /// </summary>
            /// <param name="url">搜索网址</param>
            /// <param name="Chareset">编码</param>
            public string search(string url, string Chareset)
            {
                HttpState result = new HttpState();
                Uri uri = new Uri(url);
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.UseDefaultCredentials = true;
                myHttpWebRequest.ContentType = "text/html";
                myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.CookieContainer = new CookieContainer();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    // 从 ResponseStream 中读取HTML源码并格式化 add by cqp
                    result.Html = readResponseStream(response, Chareset);
                    result.CookieContainer = myHttpWebRequest.CookieContainer;
                    return result.Html;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

            }
            public string readResponseStream(HttpWebResponse response, string Chareset)
            {
                string result = "";
                using (StreamReader responseReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(Chareset)))
                {
                    result = formatHTML(responseReader.ReadToEnd());
                }

                return result;
            }
            /// <summary>
            /// 描述:格式化网页源码
            /// 
            /// </summary>
            /// <param name="htmlContent"></param>
            /// <returns></returns>
            public string formatHTML(string htmlContent)
            {
                string result = "";

                result = htmlContent.Replace("&raquo;", "").Replace("&nbsp;", "")
                        .Replace("&copy;", "").Replace("/r", "").Replace("/t", "")
                        .Replace("/n", "").Replace("&amp;", "&");
                return result;
            }
        }

        class BaiduSearch
            {
                protected string uri = "http://www.baidu.com/s?wd=";
                protected Encoding queryEncoding = Encoding.GetEncoding("gb2312");
                protected Encoding pageEncoding = Encoding.GetEncoding("gb2312");
                protected string resultPattern = @"(?<=找到相关结果[约]?)[0-9,]*?(?=个)";
                public int GetSearchCount(string html)
                {
                    int result = 0;
                    string searchcount = string.Empty;

                    Regex regex = new Regex(resultPattern);
                    Match match = regex.Match(html);

                    if (match.Success)
                    {
                        searchcount = match.Value;
                    }
                    else
                    {
                        searchcount = "0";
                    }

                    if (searchcount.IndexOf(",") > 0)
                    {
                        searchcount = searchcount.Replace(",", string.Empty);
                    }

                    int.TryParse(searchcount, out result);

                    return result;
                }

                public List<Keyword> GetKeywords(string html, string word)
                {
                    int i = 1;
                    List<Keyword> keywords = new List<Keyword>();
                    string ss = "<a.*?href=\"(?<url>.*?)\".*><em>(?<title>.*?)</em>*.百度百科</a>";
                    MatchCollection mcTable = Regex.Matches(html, ss);
                    foreach (Match mTable in mcTable)
                    {
                        if (mTable.Success)
                        {
                            Keyword keyword = new Keyword();
                            keyword.ID = i++;
                            keyword.Title = Regex.Replace(mTable.Groups["title"].Value, "<[^>]*>", string.Empty);
                            keyword.Link = mTable.Groups["url"].Value;
                            keywords.Add(keyword);

                        }
                    }

                    return keywords;
                }
            }
    }
}
