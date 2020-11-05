using System;
using CartoonReptile.Dao;
using CartoonReptile.Model.HanHanCartoon.Enum;
using CartoonReptile.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //DbContext db = new DbContext();
            //var list = db.HanHanCartoonDao.GetQueryable().ToList();
            HanHanCartoonService service = new HanHanCartoonService();
            service.Run();
            //var r = service.GetCartoonInfo(new CartoonReptile.Model.SDK.HanHanCartoon.CartoonUrlReponse() { 
            // Name = "偶像大师：闪耀色彩",
            // Url = "http://www.1manhua.net/manhua38561.html",
            //});
            //StateTypeEnum t = (StateTypeEnum)Enum.Parse(typeof(StateTypeEnum), "连载", true);
            // y = (int)t;
            //string imgUrl = "http://www.1manhua.net/page84865/1.html?s=6";
            //string imgUrlTemplate = imgUrl.Substring(0, imgUrl.LastIndexOf("/") + 1) + "{0}.html?"+ imgUrl.Substring(imgUrl.LastIndexOf("?")+1) ;

            //string strPosition = "";

            //StateTypeEnum position = (StateTypeEnum)Enum.Parse(typeof(StateTypeEnum), strPosition, true);
            //int u = (int)position;
           // var y = (int)CommService.GetEnumDescription<StateTypeEnum>("完结");
        }

        public void m (int y)
        {
            for (int i = 0; i < y; i++)
            {
                y++;
                var t = 1;
            }
        }

    }
}
