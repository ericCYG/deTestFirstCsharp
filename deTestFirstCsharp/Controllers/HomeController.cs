using deTestFirstCsharp.Dacs;
using deTestFirstCsharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace deTestFirstCsharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GSSWEBContext _context;

        public HomeController(ILogger<HomeController> logger, GSSWEBContext context)
        {

            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {

            //var temp = _context.deTestWebFormMember;
            //List<CMemberViewModel> list = new List<CMemberViewModel>();
            //foreach (deTestWebFormMember item in temp)
            //{
            //    list.Add(new CMemberViewModel() { deTestWebFormMemberBody = item });
            //} //題目不准ef

            DataTable dt = new Utility().SqlDataReader("select * from deTestWebFormMember", new Dictionary<string, object>());
            List<CMemberViewModel> cMemberViewModels = new List<CMemberViewModel>();
            foreach (DataRow dr in dt.Rows)
            {

                cMemberViewModels.Add(new CMemberViewModel()
                {
                    id = Convert.ToInt32(dr["id"]),
                    name = dr["name"].ToString(),
                    sex = dr["sex"].ToString(),
                    phone = dr["phone"].ToString(),
                    address = dr["address"].ToString()
                });
            }

            return View(cMemberViewModels);
        }
        [HttpPost]
        public IActionResult Index(CMemberViewModel cmvm)
        {
            # region
            ///記前端的字因為會跳檔所以只能用tempdata
            ///
            TempData["id"] = cmvm.id;
            TempData["name"] = cmvm.name;
            TempData["sex"] = cmvm.sex;
            TempData["phone"] = cmvm.phone;
            TempData["address"] = cmvm.address;
            #endregion
            string sql = " select * from deTestWebFormMember ";

            int count = 0;//如果他是第一項就不用加and ，第二項開始要加and
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            foreach (var item in cmvm.deTestWebFormMemberBody.GetType().GetProperties())
            {
                //class中有值sql就要加 where 
                if (item.PropertyType == typeof(string))
                {
                    if (item.GetValue(cmvm.deTestWebFormMemberBody) != null)
                    {
                        sql += " where ";
                        break;
                    }
                }
                if (item.PropertyType == typeof(int))
                {
                    if (Convert.ToInt32(item.GetValue(cmvm.deTestWebFormMemberBody)) > 0)
                    {
                        sql += " where ";
                        break;
                    }
                }
            }

            if (cmvm.deTestWebFormMemberBody.id > 0)
            {
                _ = count++ == 0 ? sql += " id = @id " : sql += " and id = @id ";
                keyValuePairs.Add("id", cmvm.deTestWebFormMemberBody.id);
            }
            if (cmvm.deTestWebFormMemberBody.name != null)
            {
                _ = count++ == 0 ? sql += " name like '%' + @name + '%' " : sql += " and name like '%' + @name + '%' ";
                keyValuePairs.Add("name", cmvm.deTestWebFormMemberBody.name);
            }
            if (cmvm.deTestWebFormMemberBody.sex != null)
            {
                _ = count++ == 0 ? sql += " sex like '%' + @sex  + '%' " : sql += " and sex like '%' + @sex + '%' ";
                keyValuePairs.Add("sex", cmvm.deTestWebFormMemberBody.sex);
            }
            if (cmvm.deTestWebFormMemberBody.phone != null)
            {
                _ = count++ == 0 ? sql += " phone like '%' + @phone + '%' " : sql += " and phone like '%' + @phone + '%' ";
                keyValuePairs.Add("phone", cmvm.deTestWebFormMemberBody.phone);
            }
            if (cmvm.deTestWebFormMemberBody.address != null)
            {
                _ = count++ == 0 ? sql += " address like '%' + @address + '%' " : sql += " and address like '%' + @address + '%' ";
                keyValuePairs.Add("address", cmvm.deTestWebFormMemberBody.address);
            }

            DataTable dt = new Utility().SqlDataReader(sql, keyValuePairs);

            List<CMemberViewModel> list = new List<CMemberViewModel>();

            foreach (DataRow dr in dt.Rows)
            {

                list.Add(new CMemberViewModel()
                {
                    id = Convert.ToInt32(dr["id"]),
                    name = dr["name"].ToString(),
                    sex = dr["sex"].ToString(),
                    phone = dr["phone"].ToString(),
                    address = dr["address"].ToString()
                });
            }

            return View(list);
        }
        [HttpPost]
        public IActionResult Create(CMemberViewModel cmvm)
        {
            #region
            ///記前端的字因為會跳檔所以只能用tempdata
            ///
            TempData["id"] = cmvm.id;
            TempData["name"] = cmvm.name;
            TempData["sex"] = cmvm.sex;
            TempData["phone"] = cmvm.phone;
            TempData["address"] = cmvm.address;
            #endregion
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            foreach (var item in cmvm.deTestWebFormMemberBody.GetType().GetProperties())
            {

                if (item.PropertyType == typeof(int))
                {
                    if (Convert.ToInt32(item.GetValue(cmvm.deTestWebFormMemberBody)) < 0)
                    {
                        TempData["Alert"] = "數字部份，有空值";
                        return RedirectToAction("index", "home");
                    }
                    keyValuePairs.Add(item.Name, Convert.ToInt32(item.GetValue(cmvm.deTestWebFormMemberBody)));
                }
                if (item.PropertyType == typeof(string))
                {
                    if (item.GetValue(cmvm.deTestWebFormMemberBody) == null)
                    {
                        TempData["Alert"] = "文字部份，有空值";
                        return RedirectToAction("index", "home");
                    }
                    keyValuePairs.Add(item.Name, item.GetValue(cmvm.deTestWebFormMemberBody));
                }
            }

            string sql = @"
                                        INSERT INTO [dbo].[deTestWebFormMember]
                                                   (
                                                    [id]
                                                   ,[name]
                                                   ,[sex]
                                                   ,[phone]
                                                   ,[address]
                                                    )
                                             VALUES
                                                   (
                                                    @id
                                                   ,@name
                                                   ,@sex
                                                   ,@phone
                                                   ,@address
                                                    )
                                        ";

            string result = new Utility().SqlDataNonQuery(sql, keyValuePairs);
            //TempData["Alert"] = "成功新增 " + result + "  筆 。";
            //TempData["Alert"] = "aa";
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public IActionResult Edit(CMemberViewModel cmvm)
        {

            #region
            ///記前端的字因為會跳檔所以只能用tempdata
            ///
            TempData["id"] = cmvm.id;
            TempData["name"] = cmvm.name;
            TempData["sex"] = cmvm.sex;
            TempData["phone"] = cmvm.phone;
            TempData["address"] = cmvm.address;
            #endregion

            string sql = @"
                                        UPDATE [dbo].[deTestWebFormMember]
                                           SET 
                                              [name] = @name
                                              ,[sex] = @sex
                                              ,[phone] = @phone
                                              ,[address] = @address
                                         WHERE id = @id
                                        ";

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            keyValuePairs.Add("name", cmvm.name);
            keyValuePairs["address"] = cmvm.address;
            keyValuePairs.Add("sex", cmvm.sex);
            keyValuePairs.Add("phone", cmvm.phone);
            keyValuePairs["id"] = cmvm.id;

            new Utility().SqlDataNonQuery(sql, keyValuePairs);

            return RedirectToAction("index", "home");
        }
        public IActionResult Edit(int id)
        {

            string sql = @"  select * from deTestWebFormMember where id = @id ";

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("id", id);

            DataTable dt = new Utility().SqlDataReader(sql, keyValuePairs);

            if (dt.Rows.Count != 0)
            {


                #region
                ///記前端的字因為會跳檔所以只能用tempdata
                ///
                TempData["id"] = dt.Rows[0]["id"];
                TempData["name"] = dt.Rows[0]["name"];
                TempData["sex"] = dt.Rows[0]["sex"];
                TempData["phone"] = dt.Rows[0]["phone"];
                TempData["address"] = dt.Rows[0]["address"];
                #endregion
            }



            return RedirectToAction("index", "home");
        }


        public IActionResult Delete(int id)
        {

            string sql = @"
                                        DELETE FROM [dbo].[deTestWebFormMember]
                                              WHERE  id = @id
                                        ";

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            keyValuePairs.Add("id", id);

            TempData["Alert"] = "成功刪除 " + new Utility().SqlDataNonQuery(sql, keyValuePairs) + " 筆資料！";

            return RedirectToAction("index", "home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}