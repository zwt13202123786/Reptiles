using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanHanReptile
{
    public class SqlOperation
    {
        public static string InsertWholeCartoon(Cartoon cartoon, string categoryName)
        {
            //清除添加失败漫画
            //ClearFailInfo();

            //是否添加成功
            string Mess = "1";

            TBCrawlState crawlState = new TBCrawlState()
            {
                Name = cartoon.Name,
                State = 0,
            };
            int caroonCrawlState = UpdateCrawlState(crawlState);
            if (caroonCrawlState <= 0)
            {
                if (caroonCrawlState == -1)
                {
                    Mess = "漫画爬取记录存在，但状态不为0！";
                }
                else
                {
                    Mess = "添加开始爬取状态失败！";
                }
                return Mess;
            }

            //清除添加失败漫画
            if (cartoon.Name.IndexOf(' ') >= 0)
            {
                string cartoonName = cartoon.Name.Replace(" ", "");
                if (ClearInsertError(cartoonName) > 0)
                {
                    Console.WriteLine("清除添加异常漫画成功，漫画【" + cartoonName + "】");
                }
                else
                {
                    Console.WriteLine("数据库中没有要清除的漫画【" + cartoonName + "】");
                }

            }


            //1、添加漫画连载信息
            TBCartoonState cartoonState = new TBCartoonState()
            {
                Name = cartoon.State,
            };
            cartoonState.ID = InsertCartoonState(cartoonState);
            if (cartoonState.ID <= 0)
            {
                Mess = "添加漫画连载信息失败！";
                return Mess;
            }

            //2、抽离出漫画类，添加漫画表信息
            TBCartoon tBCartoon = new TBCartoon();
            tBCartoon.Name = cartoon.Name;
            tBCartoon.UpdateTime = Convert.ToDateTime(cartoon.UpdateTime);
            tBCartoon.WebPageUrl = cartoon.Url;
            tBCartoon.State = cartoonState.ID;
            tBCartoon.CoverImgUrl = cartoon.CoverImgUrl;
            tBCartoon.CollectionNumber = cartoon.CollectionNumber;
            tBCartoon.EvaluateFraction = cartoon.EvaluateFraction;
            tBCartoon.EvaluateNumber = cartoon.EvaluateNumber;
            tBCartoon.Synopsis = cartoon.Synopsis;
            tBCartoon.CreateTime = Convert.ToDateTime(cartoon.UpdateTime);

            //添加漫画信息
            tBCartoon.ID = InsertTBCartoon(tBCartoon);
            if (tBCartoon.ID <= 0)
            {
                Mess = "添加漫画信息失败！";
                return Mess;
            }

            //3、添加作者信息
            TBAuthor author = new TBAuthor();
            author.Name = cartoon.Author;
            author.Synopsis = "...";
            author.ID = InsertAuthor(author);
            if (author.ID <= 0)
            {
                Mess = "添加作者信息失败！";
                return Mess;
            }

            //4、添加漫画作者关系信息
            TBCartoonAuthor cartoonAuthor = new TBCartoonAuthor()
            {
                Cartoon_ID = tBCartoon.ID,
                Author_ID = author.ID,
            };
            if (InsertCartoonAuthor(cartoonAuthor) <= 0)
            {
                Mess = "添加漫画作者关系信息失败！";
                return Mess;
            }

            //5、添加漫画类型关系信息
            TBCartoonCategory cartoonCategory = new TBCartoonCategory()
            {
                Catoon_ID = tBCartoon.ID,
                CategoryName = categoryName,
            };
            if (InsertTBCartoonCategory(cartoonCategory) <= 0)
            {
                Mess = "添加漫画类型关系信息失败！";
                return Mess;
            }

            //6、添加漫画番剧信息
            //--获取番剧的类
            List<TBChapter> chaptersList = new List<TBChapter>();
            for (int i = 0; i < cartoon.ChapterList.Count; i++)
            {
                TBChapter chapter = new TBChapter()
                {
                    Cartoon_ID = tBCartoon.ID,
                    Name = cartoon.ChapterList[i].ChapterName,
                    CreateTime = tBCartoon.CreateTime,
                    UpdateTime = tBCartoon.CreateTime,
                };
                if (chaptersList.Count == 0) chaptersList.Add(chapter);

                bool IsExist = false; //判断番剧是否在集合中
                foreach (TBChapter item in chaptersList)
                {
                    if (item.Name == chapter.Name)
                    {
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist) chaptersList.Add(chapter);
            }
            //--添加番剧数据
            if (chaptersList.Count == 0)
            {
                Mess = "添加漫画番剧信息失败！";
                return Mess;
            }
            foreach (TBChapter item in chaptersList)
            {
                item.ID = InsertChapter(item);

            }


            //7、添加漫画番剧集数信息
            List<TBChapterNumber> chapterNumbersList = new List<TBChapterNumber>();
            for (int i = 0; i < cartoon.ChapterList.Count; i++)
            {
                TBChapterNumber chapterNumber = new TBChapterNumber();
                int chapter_ID = 0;
                foreach (TBChapter item in chaptersList)
                {
                    if (cartoon.ChapterList[i].ChapterName == item.Name)
                    {
                        chapter_ID = item.ID;
                        break;
                    }
                }
                chapterNumber.Chapter_ID = chapter_ID;
                chapterNumber.Name = cartoon.ChapterList[i].PieceName;
                chapterNumber.Sort = i;
                chapterNumbersList.Add(chapterNumber);
            }
            if (chapterNumbersList.Count == 0)
            {
                Mess = "添加漫画番剧集数信息失败！";
                return Mess;
            }
            foreach (TBChapterNumber item in chapterNumbersList)
            {
                item.ID = InsertChapterNumber(item);
            }


            //8、添加漫画图片信息  
            List<TBPiece> pieceList = new List<TBPiece>();
            for (int i = 0; i < cartoon.ChapterList.Count; i++)
            {
                int chapterNumber_ID = 0;
                foreach (TBChapterNumber item in chapterNumbersList)
                {
                    if (cartoon.ChapterList[i].PieceName == item.Name)
                    {
                        chapterNumber_ID = item.ID;
                        break;
                    }
                }

                int j = 0;
                foreach (var item in cartoon.ChapterList[i].ImgDictionary)
                {
                    TBPiece piece = new TBPiece();
                    piece.ChapterNumber_ID = chapterNumber_ID;
                    piece.WebPageUrl = item.Key;
                    piece.WebImgUrl = item.Value;
                    piece.Sort = j;
                    pieceList.Add(piece);
                    j++;
                }
            }
            foreach (TBPiece item in pieceList)
            {
                if (InsertPiece(item) <= 0)
                {
                    Mess = "添加漫画图片信息失败！";
                    return Mess;
                }
            }

            TBCrawlState crawlState1 = new TBCrawlState()
            {
                Name = cartoon.Name,
                State = 2,
            };
            if (UpdateCrawlState(crawlState1) <= 0)
            {
                Mess = "修改结束爬取状态失败！";
                return Mess;
            }
            return Mess;
        }



        //添加爬取状态记录
        public static int UpdateCrawlState(TBCrawlState crawlState)
        {
            string procName = "sp_UpdateCrawlState";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",SqlDbType.NVarChar,200),
                new SqlParameter("@State",SqlDbType.Int),
            };
            sqlParameters[0].Value = crawlState.Name;
            sqlParameters[1].Value = crawlState.State;
            object o = SqlHelper.ExecuteProcedure(procName, sqlParameters);
            return Convert.ToInt32(o);
        }

        //修改爬取状态记录
        //public int UpdateCrawlState(TBCrawlState crawlState)
        //{
        //    string sql = "update CrawlState set State = @State where Name = @Name";
        //    SqlParameter[] sqlParameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@State",crawlState.State),
        //        new SqlParameter("@Name",crawlState.Name),
        //    };
        //    return SqlHelper.ExecuteNonQuery(sql, sqlParameters);
        //}

        /// <summary>
        /// 添加漫画信息，返回漫画主键
        /// </summary>
        /// <param name="cartoon"></param>
        /// <returns></returns>
        public static int InsertTBCartoon(TBCartoon cartoon)
        {

            string sql = "insert into Cartoon(Name, WebPageUrl, State, CoverImgUrl, UpdateTime, EvaluateFraction, EvaluateNumber, Synopsis, CollectionNumber) values(@Name,@WebPageUrl,@State,@CoverImgUrl,@UpdateTime,@EvaluateFraction,@EvaluateNumber,@Synopsis,@CollectionNumber);select SCOPE_IDENTITY()";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",cartoon.Name),
                new SqlParameter("@WebPageUrl",cartoon.WebPageUrl),
                new SqlParameter("@State",cartoon.State),
                new SqlParameter("@CoverImgUrl",cartoon.CoverImgUrl),
                new SqlParameter("@UpdateTime",cartoon.UpdateTime),
                new SqlParameter("@EvaluateFraction",cartoon.EvaluateFraction),
                new SqlParameter("@EvaluateNumber",cartoon.EvaluateNumber),
                new SqlParameter("@Synopsis",cartoon.Synopsis),
                new SqlParameter("@CollectionNumber",cartoon.CollectionNumber),
            };
            object o = SqlHelper.ExecuteScalar(sql, sqlParameters);
            return Convert.ToInt32(o);
        }

        /// <summary>
        /// 添加作者，返回作者ID
        /// </summary>
        /// <returns></returns>
        public static int InsertAuthor(TBAuthor author)
        {
            string procName = "sp_InsertAuthorInfo";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",SqlDbType.NVarChar,100),
                new SqlParameter("@Synopsis",SqlDbType.NVarChar,500),
            };
            sqlParameters[0].Value = author.Name;
            sqlParameters[1].Value = author.Synopsis;
            object o = SqlHelper.ExecuteProcedure(procName, sqlParameters);
            return Convert.ToInt32(o);
        }

        /// <summary>
        ///添加漫画作者关系表
        /// </summary>
        /// <param name="cartoonAuthor"></param>
        /// <returns></returns>
        public static int InsertCartoonAuthor(TBCartoonAuthor cartoonAuthor)
        {
            string sql = "insert into CartoonAuthor values(@Cartoon_ID,@Author_ID) ";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("Cartoon_ID",cartoonAuthor.Cartoon_ID),
                 new SqlParameter("Author_ID",cartoonAuthor.Author_ID),
            };
            return (int)SqlHelper.ExecuteNonQuery(sql, sqlParameters);
        }

        /// <summary>
        /// 添加漫画连载信息
        /// </summary>
        /// <param name="cartoonState"></param>
        /// <returns></returns>
        public static int InsertCartoonState(TBCartoonState cartoonState)
        {
            "".Replace("\r\n", "");
            string procName = "sp_InsertCartoonState";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",cartoonState.Name),
            };
            object o = SqlHelper.ExecuteProcedure(procName, sqlParameters);
            return Convert.ToInt32(o);

        }










        /// <summary>
        /// 添加漫画类别表
        /// </summary>
        /// <param name="cartoonCategory"></param>
        /// <returns></returns>
        public static int InsertTBCartoonCategory(TBCartoonCategory cartoonCategory)
        {
            string sql = "sp_InsertTBCartoonCategoryInfo";
            SqlParameter[] sqlParameters = new SqlParameter[]
             {
                new SqlParameter("@Cartoon_ID",cartoonCategory.Catoon_ID),
                 new SqlParameter("@Category_Name",cartoonCategory.CategoryName),
             };
            object o = SqlHelper.ExecuteProcedure(sql, sqlParameters);
            return Convert.ToInt32(o);
        }

        /// <summary>
        /// 添加漫画番剧表（更新时间和创建时间一致）
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public static int InsertChapter(TBChapter chapter)
        {
            string sql = "Insert into Chapter values(@Cartoon_ID,@Name,@CreateTime,@UpdateTime);select SCOPE_IDENTITY()";
            SqlParameter[] sqlParameters = new SqlParameter[]
             {
                new SqlParameter("@Cartoon_ID",chapter.Cartoon_ID),
                 new SqlParameter("@Name",chapter.Name),
                 new SqlParameter("@CreateTime",chapter.CreateTime),
                 new SqlParameter("@UpdateTime",chapter.UpdateTime),
             };
            object o = SqlHelper.ExecuteScalar(sql, sqlParameters);
            return Convert.ToInt32(o);
        }

        /// <summary>
        /// 添加章节（番剧）集数表
        /// </summary>
        /// <param name="chapterNumber"></param>
        /// <returns></returns>
        public static int InsertChapterNumber(TBChapterNumber chapterNumber)
        {
            string sql = "Insert into ChapterNumber values(@Chapter_ID,@Name,@Sort);select SCOPE_IDENTITY()";
            SqlParameter[] sqlParameters = new SqlParameter[]
             {
                new SqlParameter("@Chapter_ID",chapterNumber.Chapter_ID),
                new SqlParameter("@Name",chapterNumber.Name),
                new SqlParameter("@Sort",chapterNumber.Sort),
             };
            object o = SqlHelper.ExecuteScalar(sql, sqlParameters);
            return Convert.ToInt32(o);
        }

        /// <summary>
        /// 添加章节篇幅表
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static int InsertPiece(TBPiece piece)
        {
            string sql = "Insert into Piece(ChapterNumber_ID,WebPageUrl,WebImgUrl,Sort) values(@ChapterNumber_ID,@WebPageUrl,@WebImgUrl,@Sort)";
            SqlParameter[] sqlParameters = new SqlParameter[]
             {
                new SqlParameter("@ChapterNumber_ID",piece.ChapterNumber_ID),
                new SqlParameter("@WebPageUrl",piece.WebPageUrl),
                new SqlParameter("@WebImgUrl",piece.WebImgUrl),
                new SqlParameter("@Sort",piece.Sort),
             };
            return SqlHelper.ExecuteNonQuery(sql, sqlParameters);
        }

        /// <summary>
        /// 清除添加失败的漫画
        /// </summary>
        public static void ClearFailInfo()
        {
            string procName = "sp_ClearFailCartoon";
            SqlHelper.ExecuteReader(procName);
        }

        /// <summary>
        /// 判断漫画时候添加陈成功
        /// </summary>
        /// <param name="cartoonName"></param>
        /// <returns></returns>
        public static int IsSaveSuccess(string cartoonName)
        {
            string sql = "select top 1 state from CrawlState where Name = @Name";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",cartoonName),
            };
            object o = SqlHelper.ExecuteScalar(sql, sqlParameters);
            return Convert.ToInt32(o);
        }

        public static int ClearInsertError(string cartoonName)
        {
            string sql = "update CrawlState set state = 0 where Name = @Name";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Name",cartoonName),
            };
            return SqlHelper.ExecuteNonQuery(sql, sqlParameters);
        }



    }


















    //漫画类
    public class TBCartoon
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }
        public string WebPageUrl { get; set; }//漫画地址
        public int State { get; set; }
        public string CoverImgUrl { get; set; }//漫画封面地址
        public int? CollectionNumber { get; set; }//收藏数量
        public double? EvaluateFraction { get; set; }//评价分数
        public int? EvaluateNumber { get; set; }//评价人数
        public string Synopsis { get; set; }//简介
        public DateTime CreateTime { get; set; }
    }


    //章节（番剧）表
    public class TBChapter
    {
        public int ID { get; set; }
        public int? Cartoon_ID { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }

    //章节（番剧）集数表
    public class TBChapterNumber
    {
        public int ID { get; set; }
        public int? Chapter_ID { get; set; }
        public string Name { get; set; }
        public int? Sort { get; set; }
    }

    //章节篇幅表
    public class TBPiece
    {
        public int ID { get; set; }
        public int? ChapterNumber_ID { get; set; }
        public string WebPageUrl { get; set; }
        public string WebImgUrl { get; set; }
        public string LocalhostImgUrl { get; set; }
        public int? Sort { get; set; }
    }


    //作者表
    public class TBAuthor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Synopsis { get; set; }//作者简介
    }

    public class TBCartoonAuthor
    {
        public int Cartoon_ID { get; set; }
        public int Author_ID { get; set; }
    }

    //漫画类别关系表
    public class TBCartoonCategory
    {
        public int? Catoon_ID { get; set; }
        public string CategoryName { get; set; }
    }




    //漫画爬取状态表（记录漫画有没有爬完）
    public class TBCrawlState
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int State { get; set; }
    }

    public class TBCartoonState
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }



}
