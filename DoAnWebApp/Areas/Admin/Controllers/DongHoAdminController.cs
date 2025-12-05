using DoAnWebApp.Models;
using DoAnWebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DongHoAdminController : Controller
    {
        private readonly DongHoService _dhService;
        private readonly LoaiDHService _loaiService;
        private readonly IWebHostEnvironment _env;

        public DongHoAdminController(
            DongHoService dhService,
            LoaiDHService loaiService,
            IWebHostEnvironment env)
        {
            _dhService = dhService;
            _loaiService = loaiService;
            _env = env;
        }

        // ==================== GET =====================
        public async Task<IActionResult> Index(string? task, string? keyword, int? loai)
        {
            List<DongHo> list = new();
            List<LoaiDH> listLoai = await _loaiService.GetAllAsync();

            if (task == null)
            {
                list = await _dhService.GetAllAsync();
            }
            else
            {
                switch (task)
                {
                    case "searchByType":
                        if (loai != null)
                        {
                            list = await _dhService.SearchByLoaiAsync(loai.Value);
                            ViewBag.loai = loai.Value;
                        }
                        break;

                    case "searchByName":
                        list = await _dhService.SearchByNameAsync(keyword ?? "");
                        ViewBag.keyword = keyword;
                        break;

                    default:
                        list = await _dhService.GetAllAsync();
                        break;
                }
            }

            ViewBag.listLoai = listLoai;
            return View("Index", list);
        }


        // ==================== POST =====================
        [HttpPost]
        public async Task<IActionResult> Index(
            string task,
            string ma,
            string ten,
            string loai,
            string gia,
            IFormFile? anh,
            string anhHienTai,
            string solgt,
            string mota,
            string motact)
        {
            // =================== XÓA ====================
            if (task == "delete")
            {
                await _dhService.DeleteAsync(ma);
                return RedirectToAction("Index");
            }

            // ================== CHECK RỖNG ==================
            if (string.IsNullOrEmpty(ma) ||
                string.IsNullOrEmpty(ten) ||
                string.IsNullOrEmpty(loai) ||
                string.IsNullOrEmpty(gia) ||
                string.IsNullOrEmpty(solgt) ||
                string.IsNullOrEmpty(mota) ||
                string.IsNullOrEmpty(motact))
            {
                TempData["ThongBao"] = "Vui lòng nhập đầy đủ thông tin!";
                TempData["TaskTam"] = task;

                TempData["MaTam"] = ma;
                TempData["TenTam"] = ten;
                TempData["LoaiTam"] = loai;
                TempData["GiaTam"] = gia;
                TempData["AnhTam"] = anhHienTai;
                TempData["SoLgTam"] = solgt;
                TempData["MoTaTam"] = mota;
                TempData["MoTaCTTam"] = motact;

                return RedirectToAction("Index");
            }

            // ================= PARSE =================
            int loaiInt = int.Parse(loai);
            double giaDb = double.Parse(gia);
            int slInt = int.Parse(solgt);

            // ================= UPLOAD ẢNH =================
            string fileName = anhHienTai;

            if (anh != null && anh.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "IMG");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                fileName = anh.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await anh.CopyToAsync(stream);
                }
            }

            // ================= TẠO OBJ =================
            var dh = new DongHo
            {
                MaSP = ma,
                TenSP = ten,
                MaLoai = loaiInt,
                DonGia = giaDb,
                SoLuongTon = slInt,
                MoTa = mota,
                MoTaChiTiet = motact,
                Anh = fileName
            };

            // ================= UPDATE =================
            if (task == "update")
            {
                await _dhService.UpdateAsync(dh);
            }
            else
            {
                // ================= ADD =================
                var exists = await _dhService.GetByIDAsync(ma);
                if (exists != null)
                {
                    TempData["ThongBao"] = "Mã sản phẩm đã tồn tại! Vui lòng nhập mã khác.";
                    TempData["TaskTam"] = task;

                    TempData["MaTam"] = ma;
                    TempData["TenTam"] = ten;
                    TempData["LoaiTam"] = loai;
                    TempData["GiaTam"] = gia;
                    TempData["AnhTam"] = fileName;
                    TempData["SoLgTam"] = solgt;
                    TempData["MoTaTam"] = mota;
                    TempData["MoTaCTTam"] = motact;

                    return RedirectToAction("Index");
                }

                await _dhService.InsertAsync(dh);
            }

            return RedirectToAction("Index");
        }
    }
}
