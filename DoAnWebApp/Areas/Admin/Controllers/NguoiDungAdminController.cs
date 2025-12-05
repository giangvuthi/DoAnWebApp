using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DoAnWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NguoiDungAdminController : Controller
    {
        private readonly NguoiDungService _service;

        public NguoiDungAdminController(NguoiDungService service)
        {
            _service = service;
        }

        // GET: NguoiDungAdmin
        [HttpGet]
        public async Task<IActionResult> Index(string keyword)
        {
            List<NguoiDung> list;
            if (string.IsNullOrEmpty(keyword))
            {
                list = await _service.GetAllAsync();
            }
            else
            {
                ViewBag.Keyword = keyword;
                list = await _service.SearchByNameAsync(keyword);
            }

            return View("Index", list);
        }

        // POST: NguoiDungAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string task, string ma, string ten, string matkhau, int? quyen)
        {
            if (string.IsNullOrEmpty(task) || string.IsNullOrEmpty(ma))
            {
                TempData["ThongBao"] = "Vui lòng nhập đầy đủ thông tin!";
                TempData["MaDangNhap"] = ma;
                TempData["TenND"] = ten;
                TempData["MatKhau"] = matkhau;
                TempData["Quyen"] = quyen;
                TempData["TaskTam"] = task;
                return RedirectToAction("Index");
            }

            try
            {
                if (task == "delete")
                {
                    await _service.DeleteAsync(ma);
                }
                else
                {
                    if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(matkhau) || quyen == null)
                    {
                        TempData["ThongBao"] = "Vui lòng nhập đầy đủ thông tin!";
                        TempData["MaDangNhap"] = ma;
                        TempData["TenND"] = ten;
                        TempData["MatKhau"] = matkhau;
                        TempData["Quyen"] = quyen;
                        TempData["TaskTam"] = task;
                        return RedirectToAction("Index");
                    }

                    var ng = new NguoiDung
                    {
                        MaDangNhap = ma,
                        TenND = ten,
                        MatKhau = matkhau,
                        Quyen = quyen.Value
                    };

                    if (task == "update")
                    {
                        await _service.UpdateAsync(ng);
                    }
                    else if (task == "add")
                    {
                        var existing = await _service.GetByIDAsync(ma);
                        if (existing != null)
                        {
                            TempData["ThongBao"] = "Mã đăng nhập đã tồn tại! Vui lòng nhập mã khác";
                            TempData["MaDangNhap"] = ma;
                            TempData["TenND"] = ten;
                            TempData["MatKhau"] = matkhau;
                            TempData["Quyen"] = quyen;
                            TempData["TaskTam"] = task;
                            return RedirectToAction("Index");
                        }

                        await _service.InsertAsync(ng);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ThongBao"] = "Có lỗi xảy ra: " + ex.Message;
                TempData["MaDangNhap"] = ma;
                TempData["TenND"] = ten;
                TempData["MatKhau"] = matkhau;
                TempData["Quyen"] = quyen;
                TempData["TaskTam"] = task;
                return RedirectToAction("Index");
            }
        }
    }
}
