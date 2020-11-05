using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanHanReptile
{
    public class HanHanReptile
    {
        string RootImgUrl = "http://164.94201314.net/dm06";
        string RootUrl = "http://www.1manhua.net";
        string CartoonUrlStr = "http://www.1manhua.net/comic/class_";

        /// <summary>
        /// 获取指定分类目录下的所有漫画
        /// </summary>
        /// <param name="index">要爬取的类型ID</param>
        /// <returns></returns>
        public List<CartoonAll> GetAllCartoon(int index, string typeName, int number)
        {
            List<CartoonAll> CartoonAllList = new List<CartoonAll>();
            int tempIndex = 1;
            bool IsNext = true;
            int CartoonCount = 0;
            while (IsNext)
            {
                //1、拼接地址
                string tempUrl = CartoonUrlStr + index + "/" + tempIndex + ".html";

                //2、爬取数据
                string HtmlStr = GetHtml(tempUrl);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(HtmlStr);
                HtmlNodeCollection HtmlNodeList = doc.DocumentNode.SelectNodes("//div[@id='list']/div[@class='cComicList']/li/a");
                //判断页面有没有漫画
                if (HtmlNodeList == null || HtmlNodeList.Count <= 0)
                {
                    IsNext = false;
                    break;
                }
                //统计爬取条数
                CartoonCount += HtmlNodeList.Count;
                foreach (HtmlNode item in HtmlNodeList)
                {
                    //新增：返回指定数量的漫画
                    if (CartoonAllList.Count >= number)
                    {
                        IsNext = false;
                        break;
                    }

                    CartoonAll cartoonAll = new CartoonAll();
                    cartoonAll.Name = item.Attributes["title"].Value;
                    cartoonAll.Url = RootUrl + item.Attributes["href"].Value;
                    cartoonAll.TypeName = typeName;
                    cartoonAll.CrawlState = 0;
                    //判断漫画是否爬取成功
                    if (SqlOperation.IsSaveSuccess(cartoonAll.Name) != 2)
                    {
                        CartoonAllList.Add(cartoonAll);
                    }

                }

                //显示当前爬取总数
                Console.WriteLine(typeName + "分类漫画新增数量：" + CartoonAllList.Count + "条");
                Console.WriteLine(typeName + "分类漫画爬取总数量：" + CartoonCount + "条");
                tempIndex++;



            }
            return CartoonAllList;
        }
        /// <summary>
        /// 获取漫画实体类
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Cartoon GetCartoonChapter(string url)
        {
            string HtmlStr = GetHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlStr);
            Cartoon cartoon = new Cartoon();
            cartoon.Url = url;
            //1、获取漫画封面图片
            HtmlNodeCollection HtmlNodeImg = doc.DocumentNode.SelectNodes("//div[@id='about_style']/img");
            cartoon.CoverImgUrl = HtmlNodeImg[0].Attributes["src"].Value;
            //2、获取漫画信息
            HtmlNodeCollection HtmlNodeList = doc.DocumentNode.SelectNodes("//div[@id='about_kit']/ul/li");
            HtmlNode htmlNode = HtmlNodeList[0];
            cartoon.Name = htmlNode.InnerText.Replace("\r", "").Replace("\n", "").Trim();
            htmlNode = HtmlNodeList[1];
            cartoon.Author = htmlNode.InnerText.Replace("作者:", "");
            htmlNode = HtmlNodeList[2];
            cartoon.State = htmlNode.InnerText.Replace("状态:", "");
            htmlNode = HtmlNodeList[4];
            cartoon.UpdateTime = htmlNode.InnerText.Replace("更新:", "");
            htmlNode = HtmlNodeList[5];
            cartoon.CollectionNumber = Convert.ToInt32(htmlNode.InnerText.Replace("收藏:", "").Replace("人收藏本漫画", ""));
            htmlNode = HtmlNodeList[6];
            cartoon.EvaluateNumber = Convert.ToInt32(htmlNode.InnerText.Replace("(", ")").Split(')')[1].Replace("人评", ""));
            htmlNode = HtmlNodeList[7];
            cartoon.Synopsis = htmlNode.InnerText.Replace("简介:", "");
            htmlNode = HtmlNodeList[6];
            cartoon.EvaluateFraction = Convert.ToDouble(htmlNode.FirstChild.NextSibling.NextSibling.InnerText);

            //3、获取漫画章节详细（每话的图片）
            HtmlNodeCollection divHtmlNodeList = doc.DocumentNode.SelectNodes("//div[@class='cVolTag']");
            HtmlNodeCollection ulHtmlNodeList = doc.DocumentNode.SelectNodes("//ul[@class='cVolUl']");
            if (divHtmlNodeList == null)
            {
                Console.WriteLine("漫画【" + cartoon.Name + "】无内容！");
                return null;
            }
            if (ulHtmlNodeList == null)
            {
                Console.WriteLine("漫画【" + cartoon.Name + "】无内容！");
                return null;
            }
            List<Chapter> ChapterList = new List<Chapter>();
            for (int i = 0; i < divHtmlNodeList.Count; i++)
            {
                Chapter chapter = new Chapter();
                chapter.ChapterName = divHtmlNodeList[i].InnerText;
                HtmlNode ulHtmlNode = ulHtmlNodeList[i];
                HtmlNode firstHtmlNode = ulHtmlNode.FirstChild;
                if (firstHtmlNode == null) break;
                chapter.Url = firstHtmlNode.FirstChild.Attributes["href"].Value;//firstHtmlNode.FirstChild 拿到的是a标签
                chapter.PieceName = firstHtmlNode.FirstChild.InnerText;
                Dictionary<string, string> ImgDictionary = new Dictionary<string, string>();
                chapter.ImgDictionary = GetImgDictionary(RootUrl + chapter.Url, ImgDictionary, cartoon.Name, chapter.ChapterName, chapter.PieceName);
                ChapterList.Add(chapter);

                bool isNext = true;
                HtmlNode nextHtmlNode = firstHtmlNode.NextSibling;
                while (isNext)
                {
                    Chapter chapter1 = new Chapter();
                    chapter1.ChapterName = divHtmlNodeList[i].InnerText;
                    if (nextHtmlNode == null)
                    {
                        isNext = false;
                        if (ChapterList.Count > 0)
                        {
                            //倒序
                            List<Chapter> tempChapterList = new List<Chapter>();
                            for (int j = ChapterList.Count - 1; j >= 0; j--)
                            {
                                tempChapterList.Add(ChapterList[j]);
                            }
                            cartoon.ChapterList = tempChapterList;
                        }

                        break;
                    }
                    chapter1.Url = nextHtmlNode.FirstChild.Attributes["href"].Value;//firstHtmlNode.FirstChild 拿到的是a标签
                    chapter1.PieceName = nextHtmlNode.FirstChild.InnerText;
                    ImgDictionary = new Dictionary<string, string>();
                    chapter1.ImgDictionary = GetImgDictionary(RootUrl + chapter1.Url, ImgDictionary, cartoon.Name, chapter1.ChapterName, chapter1.PieceName);
                    ChapterList.Add(chapter1);
                    nextHtmlNode = nextHtmlNode.NextSibling;
                }
            }
            return cartoon;
        }

        /// <summary>
        /// 获取漫画章节图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ImgDictionary"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetImgDictionary(string url, Dictionary<string, string> imgDictionary, string cartoonName, string chapterName, string pieceName)
        {
            string HtmlStr = GetHtml(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlStr);
            HtmlNodeCollection HtmlNodeList = doc.DocumentNode.SelectNodes("//div[@id='iBodyQ']/img");
            foreach (HtmlNode item in HtmlNodeList)
            {
                string TempUrl = DecryptImgUrl(item.Attributes["name"].Value);
                if (TempUrl == "-1")
                {
                    //Console.WriteLine(string.Format("漫画：{0}|番剧：{1}|集数：{2}--爬取成功！", cartoonName, chapterName, pieceName));
                    return imgDictionary;
                }
                imgDictionary.Add(url, RootImgUrl + TempUrl);
            }
            //重新生成url http://www.1manhua.net/page370871/1.html?s=5
            int index1 = url.LastIndexOf("/");
            string url1 = url.Substring(0, index1 + 1);
            int index2 = url.LastIndexOf(".");
            string temp = url.Substring(index1 + 1, index2 - index1 - 1);
            int num = Convert.ToInt32(temp) + 1;
            string url2 = url.Substring(index2, url.Length - index2);
            if (url2.IndexOf("&d=0") <= 0) url2 += "&d=0";
            url = url1 + num + url2;
            return GetImgDictionary(url, imgDictionary, cartoonName, chapterName, pieceName);
        }

        /// <summary>
        /// 解密图片地址
        /// </summary>
        /// <param name="ImgUrl">加密的图片地址</param>
        /// <returns></returns>
        public string DecryptImgUrl(string ImgUrl)
        {
            //string ImgUrl = "yexoooxopexytxqqxoooxopqxoptxqqxywxtyxyexeqxyextoxtpxtoxtyxtexyexqexqqxoorxqtxywxywxyqxyexoiixqtxywxywxywxttxqtxtpxywxtuxtextexyrxeyxwpxeopoiuytrewqxreoqwxexxtpittn";
            string x = ImgUrl.Substring(ImgUrl.Length - 1);
            string w = "abcdefghijklmnopqrstuvwxyz";
            int xi = w.IndexOf(x) + 1;
            string sk = ImgUrl.Substring(ImgUrl.Length - xi - 12, 11);
            ImgUrl = ImgUrl.Substring(0, ImgUrl.Length - xi - 12);
            string k = sk.Substring(0, sk.Length - 1);
            char f = Convert.ToChar(sk.Substring(sk.Length - 1));
            for (int i = 0; i < k.Length; i++)
            {
                ImgUrl = ImgUrl.Replace(k[i].ToString(), i.ToString());
            }
            string[] ByteStr = ImgUrl.Split(new char[] { 'x' });
            ImgUrl = "";
            try 
            {
                byte[] bb = new byte[ByteStr.Length];
                for (int i = 0; i < ByteStr.Length; i++)
                {
                    bb[i] = Convert.ToByte(Convert.ToInt32(ByteStr[i]));
                }
                ImgUrl = Encoding.UTF8.GetString(bb);
            }
            catch (Exception e)
            {
                return "-1";
            }
            return ImgUrl;
        }
        /// <summary>
        /// 获取网页文本
        /// </summary>
        /// <param name="url">网页地址</param>
        /// <returns></returns>
        public string GetHtml(string url)
        {
            RequestOptions requestOptions = new RequestOptions()
            {
                Method = "Get",
                Uri = new Uri(url),
                Timeout = 20000
            };

            string htmlText = null;
            htmlText = new HtmlDocumentHelper().LoopRetryReturn(7000, () =>
            {
                ReptileHelper reptile = new ReptileHelper();
                return reptile.RequestAction(requestOptions);
            }, "获取网页文本失败");
            return htmlText;
        }



    }

    public class CartoonAll
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string TypeName { get; set; }
        public int CrawlState { get; set; }//爬取状态：0，未爬取，1，正在爬取，2，爬取成功
    }




    //漫画信息类
    public class Cartoon
    {
        /*
        汗汗漫画首页（ddmmcc.com）选择指定的分类之后的漫画节点信息，class="cComicList" 样式在当前h5页面中唯一
        <div class="cComicList">
            <li>
                <a title="魔物娘" href="/manhua36694.html"><img src="http://img.94201314.net/comicui/36694.JPG"><br>魔物娘</a>
            </li>
            ......
        </div>
        */
        public string Name { get; set; }//漫画名
        public string Url { get; set; }//地址
        public string Author { get; set; }//作者
        public string State { get; set; }//状态
        public string CoverImgUrl { get; set; } //封面图片地址
        public string UpdateTime { get; set; }//更新时间
        public int CollectionNumber { get; set; }//收藏数量
        public double EvaluateFraction { get; set; } //评价分数
        public int EvaluateNumber { get; set; }//评价人数
        public string Synopsis { get; set; }//简介
        public List<Chapter> ChapterList { get; set; }//漫画章节详细
    }



    //漫画章节表
    public class Chapter
    {
        /*
        <div class="cVolList">
            <div class="cVolTag">周刊杂志每周每月连载单集</div>
            <ul class="cVolUl">
                <li><a class="l_s" href="/page370857/1.html?s=3" target="_blank" title="只喜欢烦我 01v1集">只喜欢烦我 01v1集</a></li>
            </ul>
        </div>
        */
        public string ChapterName { get; set; } // 番剧名
        public string PieceName { get; set; }  //集数名
        public string Url { get; set; }
        public Dictionary<string, string> ImgDictionary { get; set; } //网页的链接，图片的链接
    }


}
