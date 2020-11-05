using CartoonReptile.Common;
using CartoonReptile.Dao;
using CartoonReptile.Model.HanHanCartoon;
using CartoonReptile.Model.HanHanCartoon.Enum;
using CartoonReptile.Model.Quartz.Enum;
using CartoonReptile.Model.SDK;
using CartoonReptile.Model.SDK.HanHanCartoon;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Service
{
    public class HanHanCartoonService : BaseService
    {
        private static string hanHanHost = "http://www.1manhua.net/";
        private static string imgHost = "http://18.125084.com/dm06/";

        //抓取图片并行任务数
        private static int imgParallel = 20;

        //抓取漫画并行任务数
        private static int cartoonParallel = 10;

        //抓取漫画链接并行任务数
        private static int cartoonUrlParallel = 20;
        public HanHanCartoonService()
        {
            db = new DbContext();
        }

        public void Run()
        {
            var reponseCartoon = GetCartoonInfo(new CartoonUrlReponse()
            {
                Name = "天鹅绒之吻",
                Url = "http://www.1manhua.net/manhua8373.html",
            });
            HandleReponseData(new List<CartoonReponse>() { reponseCartoon });


        }

        /// <summary>
        /// 获取指定分类下所有漫画链接
        /// </summary>
        /// <returns></returns>
        public List<CartoonUrlReponse> GetAllCartoonUrl()
        {
            //剧情分类页数
            int pageCount = 407;
            //剧情分类
            string url = "/comic/class_5/{0}.html";
            //并发数

            List<string> plotList = new List<string>();
            List<CartoonUrlReponse> cartoonUrlList = new List<CartoonUrlReponse>();
            object cartoonUrlListLock = new object();

            for (int i = 1; i <= pageCount; i++)
            {
                plotList.Add(string.Format(hanHanHost + url, i));
            }

            plotList.AsParallel().WithDegreeOfParallelism(cartoonUrlParallel).ForAll(s =>
            {
                List<CartoonUrlReponse> cartoonUrlPageList = GetPageCartoonUrl(s);
                lock (cartoonUrlListLock)
                {
                    cartoonUrlList.AddRange(cartoonUrlPageList);
                }
            });

            return cartoonUrlList;
        }

        /// <summary>
        /// 获取一页的漫画链接
        /// </summary>
        /// <returns></returns>
        public List<CartoonUrlReponse> GetPageCartoonUrl(string url)
        {
            List<CartoonUrlReponse> cartoonUrlList = new List<CartoonUrlReponse>();
            string htmlText = new CommService().LoopRetryReturn<string>(7000, 10, () =>
            {
                return ReptileHelper.RequestAction(new RequestOptions()
                {
                    Method = "Get",
                    Uri = new Uri(url),
                    Timeout = 20000
                });
            }, () => { Log(1, (int)MsgTypeEnum.ExceptionMsg, $"获取剧情分类漫画链接失败，链接：{url}"); });

            if (string.IsNullOrWhiteSpace(htmlText))
            {
                Log(1, (int)MsgTypeEnum.ExceptionMsg, $"剧情分类漫画返回消息为空，链接：{url}");
                return cartoonUrlList;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            HtmlNodeCollection HtmlNodeList = doc.DocumentNode.SelectNodes("//div[@id='list']/div[@class='cComicList']/li/a");
            //判断页面有没有漫画
            if (HtmlNodeList == null || HtmlNodeList.Count <= 0)
            {
                CommService.Log(1, (int)MsgTypeEnum.ExceptionMsg, $"解析数据异常，htmlText：{htmlText}");
                return cartoonUrlList;
            }
            foreach (HtmlNode htmlNode in HtmlNodeList)
            {
                cartoonUrlList.Add(new CartoonUrlReponse()
                {
                    Name = htmlNode.Attributes["title"].Value,
                    Url = hanHanHost + htmlNode.Attributes["href"].Value,
                });
            }
            return cartoonUrlList;
        }

        /// <summary>
        /// 获取漫画全部信息
        /// </summary>
        /// <param name="reponse"></param>
        /// <returns></returns>
        public CartoonReponse GetCartoonInfo(CartoonUrlReponse reponse)
        {
            string htmlText = new CommService().LoopRetryReturn<string>(7000, 10, () =>
            {
                return ReptileHelper.RequestAction(new RequestOptions()
                {
                    Method = "Get",
                    Uri = new Uri(reponse.Url),
                    Timeout = 20000
                });
            }, () => { Log(1, (int)MsgTypeEnum.ExceptionMsg, $"获取漫画页面信息失败，链接：{reponse.Url}"); });

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            HtmlNodeCollection HtmlNodeList = doc.DocumentNode.SelectNodes("//div[@id='about_kit']/ul/li");
            CartoonReponse cartoon = new CartoonReponse()
            {
                Url = reponse.Url,
                CoverImgUrl = doc.DocumentNode.SelectNodes("//div[@id='about_style']/img")[0].Attributes["src"].Value,
                Name = HtmlNodeList[0].InnerText.Replace("\r", "").Replace("\n", "").Trim(),
                Author = HtmlNodeList[1].InnerText.Replace("作者:", ""),
                State = HtmlNodeList[2].InnerText.Replace("状态:", ""),
                UpdateTime = HtmlNodeList[4].InnerText.Replace("更新:", ""),
                CollectionNumber = Convert.ToInt32(HtmlNodeList[5].InnerText.Replace("收藏:", "").Replace("人收藏本漫画", "")),
                EvaluateNumber = Convert.ToInt32(HtmlNodeList[6].InnerText.Replace("(", ")").Split(')')[1].Replace("人评", "")),
                Synopsis = HtmlNodeList[7].InnerText.Replace("简介:", ""),
                EvaluateFraction = Convert.ToDecimal(HtmlNodeList[6].FirstChild.NextSibling.NextSibling.InnerText),
            };

            //获取漫画详细章节
            HtmlNodeCollection divHtmlNodeList = doc.DocumentNode.SelectNodes("//div[@class='cVolTag']");
            HtmlNodeCollection ulHtmlNodeList = doc.DocumentNode.SelectNodes("//ul[@class='cVolUl']");
            if (divHtmlNodeList == null || divHtmlNodeList.Count == 0)
            {
                Log(1, (int)MsgTypeEnum.ExceptionMsg, $"漫画章节为空，链接：{reponse.Url}");
                return cartoon;
            }
            if (ulHtmlNodeList == null || ulHtmlNodeList.Count == 0)
            {
                Log(1, (int)MsgTypeEnum.ExceptionMsg, $"漫画话数为空，链接：{reponse.Url}");
                return cartoon;
            }

            List<ChapterReponse> ChapterList = new List<ChapterReponse>();
            for (int i = 0; i < divHtmlNodeList.Count; i++)
            {
                foreach (var liNode in ulHtmlNodeList[i].ChildNodes.Reverse())
                {
                    ChapterReponse chapter = new ChapterReponse()
                    {
                        ChapterName = divHtmlNodeList[i].InnerText,
                        PieceName = liNode.FirstChild.InnerText,
                        Url = hanHanHost + liNode.FirstChild.Attributes["href"].Value,
                        //ImgPieceList = GetPieceImg(hanHanHost + liNode.FirstChild.Attributes["href"].Value)
                    };
                    Log(1, (int)MsgTypeEnum.NormalMsg, $"【{cartoon.Name}】番剧：{chapter.ChapterName}，集数：{chapter.PieceName}，抓取完成！");
                    ChapterList.Add(chapter);
                }
            }
            cartoon.ChapterList = ChapterList;
            Log(1, (int)MsgTypeEnum.NormalMsg, $"【{cartoon.Name}】抓取完成！");
            return cartoon;
        }

        /// <summary>
        /// 获取每话所有图片
        /// </summary>
        /// <param name="url">第1张图片</param>
        public List<PieceReponse> GetPieceImg(string imgUrl)
        {
            List<PieceReponse> imgList = new List<PieceReponse>();

            //1、获取第一页的图片
            string firstHtmlText = new CommService().LoopRetryReturn<string>(7000, 10, () =>
            {
                return ReptileHelper.RequestAction(new RequestOptions()
                {
                    Method = "Get",
                    Uri = new Uri(imgUrl),
                    Timeout = 20000
                });
            }, () => { Log(1, (int)MsgTypeEnum.ExceptionMsg, $"获取漫画篇章图片失败，链接：{imgUrl}"); });
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(firstHtmlText);
            string firstImgUrl = doc.DocumentNode.SelectNodes("//div[@id='iBodyQ']/img")[0].Attributes["name"].Value;
            imgList.Add(new PieceReponse() { Sort = 1, WebSiteUrl = imgUrl, ImgUrl = DecryptimgUrl(firstImgUrl) });

            //2、获取图片数
            HtmlNodeCollection pageNode = doc.DocumentNode.SelectNodes("//div[@class='cH1']/b");
            string pageCountStr = pageNode[0].InnerText.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            int pageCount = Convert.ToInt32(pageCountStr.Substring(2, pageCountStr.Length - 2));

            //3、获取图片模板地址 
            string imgUrlTemplate = imgUrl.Substring(0, imgUrl.LastIndexOf("/") + 1) + "{0}.html?" + imgUrl.Substring(imgUrl.LastIndexOf("?") + 1);

            //4、创建每张图片地址的字符串集合
            List<KeyValuePair<int, string>> imgWebsiteUrlList = new List<KeyValuePair<int, string>>();
            for (int i = 2; i <= pageCount; i++)
            {
                imgWebsiteUrlList.Add(new KeyValuePair<int, string>(i, string.Format(imgUrlTemplate, i)));
            }

            //5、开并行任务抓取
            imgWebsiteUrlList.AsParallel().WithDegreeOfParallelism(imgParallel).ForAll(x =>
            {
                string htmlText = new CommService().LoopRetryReturn<string>(7000, 10, () =>
                {
                    return ReptileHelper.RequestAction(new RequestOptions()
                    {
                        Method = "Get",
                        Uri = new Uri(x.Value),
                        Timeout = 20000
                    });
                }, () => { Log(1, (int)MsgTypeEnum.ExceptionMsg, $"获取漫画篇章图片失败，链接：{x.Value}"); });
                HtmlDocument docHtml = new HtmlDocument();
                docHtml.LoadHtml(htmlText);
                string currentImgUrl = docHtml.DocumentNode.SelectNodes("//div[@id='iBodyQ']/img")[0].Attributes["name"].Value;
                imgList.Add(new PieceReponse()
                {
                    Sort = x.Key,
                    WebSiteUrl = x.Value,
                    ImgUrl = DecryptimgUrl(currentImgUrl),
                    ImgHost = imgUrl,
                });
            });
            return imgList.OrderBy(x => x.Sort).ToList();
        }


        /// <summary>
        /// 解密图片地址
        /// </summary>
        /// <param name="imgUrl">加密的图片地址</param>
        /// <returns></returns>
        public string DecryptimgUrl(string imgUrl)
        {
            string a = imgUrl.Substring(imgUrl.Length - 1);
            string b = "abcdefghijklmnopqrstuvwxyz";
            int c = b.IndexOf(a) + 1;
            string d = imgUrl.Substring(imgUrl.Length - c - 12, 11);
            imgUrl = imgUrl.Substring(0, imgUrl.Length - c - 12);
            string k = d.Substring(0, d.Length - 1);
            char f = Convert.ToChar(d.Substring(d.Length - 1));
            for (int i = 0; i < k.Length; i++)
            {
                imgUrl = imgUrl.Replace(k[i].ToString(), i.ToString());
            }
            string[] ByteStr = imgUrl.Split(new char[] { 'x' });
            try
            {
                byte[] bb = new byte[ByteStr.Length];
                for (int i = 0; i < ByteStr.Length; i++)
                {
                    bb[i] = Convert.ToByte(Convert.ToInt32(ByteStr[i]));
                }
                imgUrl = Encoding.UTF8.GetString(bb);
            }
            catch (Exception ex)
            {
                Log(1, (int)MsgTypeEnum.ExceptionMsg, $"解析漫画地址图片错误！错误消息：{ex.Message}");
                return null;
            }
            return imgUrl;
        }

        /// <summary>
        /// 处理接口返回数据
        /// </summary>
        /// <param name="response"></param>
        private void HandleReponseData(List<CartoonReponse> response)
        {
            var cartoonDao = db.HanHanCartoonDao;
            var chapterDao = db.HanHanCartoonChapterDao;
            var pieceDao = db.HanHanCartoonPieceDao;

            List<string> cartoonNameList = response.Select(x => x.Name).ToList();
            List<HanHanCartoonEntity> oldCartoonEntityList = db.HanHanCartoonDao.GetQueryable().Where(x => cartoonNameList.Contains(x.CartoonName)).ToList();
            List<int> oldCartoonIdList = oldCartoonEntityList.Select(x => x.Id).ToList();
            List<HanHanCartoonChapterEntity> oldChapterEntitieList = db.HanHanCartoonChapterDao.GetQueryable().Where(x => oldCartoonIdList.Contains(x.HanHanCartoonId)).ToList();
            List<HanHanCartoonPieceEntity> oldPieceEntitieList = db.HanHanCartoonPieceDao.GetQueryable().Where(x => oldCartoonIdList.Contains(x.HanHanCartoonId)).ToList();

            //1、筛选需要添加的漫画
            List<string> insertCartoonNameList = cartoonNameList.Except(oldCartoonEntityList.Select(x => x.CartoonName)).ToList();
            var insertReponseList = response.Where(x => insertCartoonNameList.Contains(x.Name)).ToList();
            var insertEntityList = ConvertToEntity(true, insertReponseList);
            if (insertEntityList.Count > 0)
            {
                Tran(DbEnum.HanHanCartoonDB, (dbContext) =>
                {
                    var cartoonTranDao = dbContext.HanHanCartoonDao;
                    var chapterTranDao = dbContext.HanHanCartoonChapterDao;
                    var pieceTranDao = dbContext.HanHanCartoonPieceDao;
                    var imgTranDao = dbContext.HanHanCartoonPieceImgDao;
                    insertEntityList.ForEach(cartoon =>
                    {
                        var cartoonEntity = cartoon.Cartoon;
                        cartoonEntity = cartoonTranDao.Insert(cartoonEntity);
                        cartoon.ChapterList.ForEach(chapter =>
                        {
                            var chapterEntity = chapter.Chapter;
                            chapterEntity.HanHanCartoonId = cartoonEntity.Id;
                            chapterEntity = chapterTranDao.Insert(chapterEntity);
                            chapter.PieceList.ForEach(piece =>
                            {
                                var pieceEntity = piece.Piece;
                                pieceEntity.HanHanCartoonId = cartoonEntity.Id;
                                pieceEntity.HanHanCartoonChapterId = chapterEntity.Id;
                                pieceEntity = pieceTranDao.Insert(pieceEntity);
                                piece.PieceImgList.ForEach(img =>
                                {
                                    img.HanHanCartoonPieceId = pieceEntity.Id;
                                });
                                imgTranDao.Insert(piece.PieceImgList);
                            });
                        });
                    });
                });
            }

            //2、筛选需要更新漫画
            List<string> updateCartoonNameList = cartoonNameList.Except(insertCartoonNameList).ToList();
            var updateReponseList = response.Where(x => updateCartoonNameList.Contains(x.Name)).ToList();
            var updateEntityList = ConvertToEntity(false, insertReponseList);
            var oldUpdateCartoonEntityList = oldCartoonEntityList.Where(x => updateCartoonNameList.Contains(x.CartoonName)).ToList();
            if (updateEntityList.Count > 0)
            {
                updateEntityList.ForEach(cartoon =>
                {
                    var cartoonEntity = oldUpdateCartoonEntityList.FirstOrDefault(x => x.CartoonName == cartoon.Cartoon.CartoonName);
                    var cartoonReponseEntity = response.FirstOrDefault(x => x.Name == cartoon.Cartoon.CartoonName);
                    if (cartoonEntity == null || cartoonReponseEntity == null)
                    {
                        Log(1, (int)MsgTypeEnum.ExceptionMsg, $"更新漫画异常，漫画不存在！漫画名：{cartoon.Cartoon.CartoonName}");
                        return;
                    }
                    //获取漫画章节信息
                    cartoon.ChapterList.ForEach(Chapter =>
                    {
                        var ChapterEntity = oldChapterEntitieList.FirstOrDefault(x => x.HanHanCartoonId == cartoonEntity.Id && x.ChapterName == Chapter.Chapter.ChapterName);
                        //获取番剧所有漫画图片
                        if (ChapterEntity == null)
                        {
                            Chapter.PieceList.ForEach(piece =>
                            {
                                var url = cartoonReponseEntity.ChapterList.FirstOrDefault(x => x.ChapterName == Chapter.Chapter.ChapterName && x.PieceName == piece.Piece.PieceName).Url;
                                piece.PieceImgList = GetPieceImg(url).Select(x => new HanHanCartoonPieceImgEntity()
                                {
                                    ImgHost = x.ImgHost,
                                    LocalhostImgUrl = "",
                                    Sort = x.Sort,
                                    WebImgUrl = x.ImgUrl,
                                    WebPageUrl = x.WebSiteUrl
                                }).ToList();
                            });
                        }
                        else//更新番剧
                        {
                            var oldNameList = oldPieceEntitieList.Where(x => x.HanHanCartoonChapterId == ChapterEntity.Id).Select(x => x.PieceName).ToList();
                            var insertNameList = Chapter.PieceList.Select(x => x.Piece).Select(x => x.PieceName).Except(oldNameList);
                            var insertPieceList = Chapter.PieceList.Where(x => insertNameList.Contains(x.Piece.PieceName)).ToList();

                            var maxSort = oldPieceEntitieList.Max(x => x.Sort);
                            foreach (var piece in insertPieceList)
                            {
                                var url = cartoonReponseEntity.ChapterList.FirstOrDefault(x => x.ChapterName == Chapter.Chapter.ChapterName && x.PieceName == piece.Piece.PieceName).Url;
                                maxSort++;
                                piece.Piece.Sort = maxSort;
                                piece.PieceImgList = GetPieceImg(url).Select(x => new HanHanCartoonPieceImgEntity()
                                {
                                    ImgHost = x.ImgHost,
                                    LocalhostImgUrl = "",
                                    Sort = x.Sort,
                                    WebImgUrl = x.ImgUrl,
                                    WebPageUrl = x.WebSiteUrl
                                }).ToList();
                            }
                            Chapter.PieceList = insertPieceList;
                        }
                    });
                    //更新数据
                    Tran(DbEnum.HanHanCartoonDB, (dbContext) =>
                    {
                        var cartoonTranDao = dbContext.HanHanCartoonDao;
                        var chapterTranDao = dbContext.HanHanCartoonChapterDao;
                        var pieceTranDao = dbContext.HanHanCartoonPieceDao;
                        var imgTranDao = dbContext.HanHanCartoonPieceImgDao;
                        cartoonTranDao.Update(cartoon.Cartoon);
                        cartoon.ChapterList.ForEach(chapter =>
                        {
                            var chapterEntity = chapter.Chapter;
                            chapterEntity.HanHanCartoonId = cartoonEntity.Id;
                            chapterEntity = chapterTranDao.Insert(chapterEntity);
                            chapter.PieceList.ForEach(piece =>
                            {
                                var pieceEntity = piece.Piece;
                                pieceEntity.HanHanCartoonId = cartoonEntity.Id;
                                pieceEntity.HanHanCartoonChapterId = chapterEntity.Id;
                                pieceEntity = pieceTranDao.Insert(pieceEntity);
                                piece.PieceImgList.ForEach(img =>
                                {
                                    img.HanHanCartoonPieceId = pieceEntity.Id;
                                });
                                imgTranDao.Insert(piece.PieceImgList);
                            });
                        });
                    });
                });
            }
        }

        /// <summary>
        /// 将抓取数据转换为实体类型，并获取图片
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<CartoonJoinChapter> ConvertToEntity(bool isGetImg, List<CartoonReponse> response)
        {
            List<CartoonJoinChapter> entityList = new List<CartoonJoinChapter>();
            try
            {
                response.ForEach(cartoon =>
                {
                    HanHanCartoonEntity cartoonEntity = new HanHanCartoonEntity()
                    {
                        CartoonName = cartoon.Name,
                        Author = cartoon.Author,
                        State = (int)CommService.GetEnumDescription<StateTypeEnum>(cartoon.State),
                        CollectionNumber = cartoon.CollectionNumber,
                        EvaluateFraction = cartoon.EvaluateFraction,
                        EvaluateNumber = cartoon.EvaluateNumber,
                        CreateTime = Convert.ToDateTime(cartoon.UpdateTime),
                        UpdateTime = Convert.ToDateTime(cartoon.UpdateTime),
                        Synopsis = cartoon.Synopsis,
                        WebCoverImgUrl = cartoon.CoverImgUrl,
                        WebPageUrl = cartoon.Url,
                        LocalhostCoverImgUrl = "",
                        LastSyncTime = DateTime.Now,
                    };
                    var chapterList = cartoon.ChapterList.GroupBy(y => y.ChapterName).ToList();
                    List<ChapterJoinPiece> chapterJoinList = new List<ChapterJoinPiece>();
                    int i = 0;
                    chapterList.ForEach(chapter =>
                    {
                        HanHanCartoonChapterEntity chapterEntity = new HanHanCartoonChapterEntity()
                        {
                            ChapterName = chapterList[i].Key,
                            CreateTime = Convert.ToDateTime(cartoon.UpdateTime),
                            UpdateTime = Convert.ToDateTime(cartoon.UpdateTime),
                            Sort = i,
                        };
                        i++;
                        int n = 0;
                        List<PieceJoinPieceImg> pieceJoinList = new List<PieceJoinPieceImg>();
                        chapter.ToList().ForEach(piece =>
                        {
                            //获取图片
                            if (isGetImg) piece.ImgPieceList = GetPieceImg(piece.Url);
                            HanHanCartoonPieceEntity pieceEntity = new HanHanCartoonPieceEntity()
                            {
                                PieceName = piece.PieceName,
                                Sort = n,
                                CreateTime = Convert.ToDateTime(cartoon.UpdateTime),
                                UpdateTime = Convert.ToDateTime(cartoon.UpdateTime),
                            };
                            n++;
                            var ImgList = new List<HanHanCartoonPieceImgEntity>();
                            if (piece.ImgPieceList != null)
                            {
                                ImgList = piece.ImgPieceList.Select(x => new HanHanCartoonPieceImgEntity()
                                {
                                    ImgHost = imgHost,
                                    LocalhostImgUrl = "",
                                    Sort = x.Sort,
                                    WebImgUrl = x.ImgUrl,
                                    WebPageUrl = x.WebSiteUrl

                                }).ToList();
                            }
                            pieceJoinList.Add(new PieceJoinPieceImg() { Piece = pieceEntity, PieceImgList = ImgList });
                        });
                        chapterJoinList.Add(new ChapterJoinPiece() { Chapter = chapterEntity, PieceList = pieceJoinList });
                    });
                    entityList.Add(new CartoonJoinChapter() { Cartoon = cartoonEntity, ChapterList = chapterJoinList });
                });
                return entityList;
            }
            catch (Exception ex)
            {
                Log(1, (int)MsgTypeEnum.ExceptionMsg, $"漫画数据转换实体类型异常：异常消息：{ex.Message}");
                return null;
            }
        }


        public List<HanHanCartoonPieceImgEntity> ConvertToPieceEntity(string url)
        {
            return GetPieceImg(url).Select(x => new HanHanCartoonPieceImgEntity()
            {
                ImgHost = x.ImgHost,
                LocalhostImgUrl = "",
                Sort = x.Sort,
                WebImgUrl = x.ImgUrl,
                WebPageUrl = x.WebSiteUrl
            }).ToList();
        }


    }


    public class CartoonJoinChapter
    {
        public HanHanCartoonEntity Cartoon { get; set; }
        public List<ChapterJoinPiece> ChapterList { get; set; }
    }
    public class ChapterJoinPiece
    {
        public HanHanCartoonChapterEntity Chapter { get; set; }
        public List<PieceJoinPieceImg> PieceList { get; set; }
    }
    public class PieceJoinPieceImg
    {
        public HanHanCartoonPieceEntity Piece { get; set; }
        public List<HanHanCartoonPieceImgEntity> PieceImgList { get; set; }
    }


}
