using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _tRepo;
        public TrailsController(INationalParkRepository npRepo, ITrailRepository tRepo)
        {
            _npRepo = npRepo;
            _tRepo = tRepo;
        }
        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        //GET UPSERT
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);
            TrailsVM objVM = new TrailsVM()
            {
                //dropdownlist dari TrailVM->IEnumerable<SelectListItem> NPList
                NPList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                //null jadi harus create new trail
                Trail = new Trail()
            };

            if (id == null)
            {
                //create
                return View(objVM);
            }

            //update
            objVM.Trail = await _tRepo.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault());
            //gagal ambil ID
            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
            
        }


        //POST Upsert
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Trail.Id == 0)
                {
                    await _tRepo.CreateAsync(SD.TrailAPIPath, obj.Trail);
                }
                else
                {
                    await _tRepo.UpdateAsync(SD.TrailAPIPath + obj.Trail.Id, obj.Trail);
                }
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);
                TrailsVM objVM = new TrailsVM()
                {
                    
                    NPList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail
                };
                return View(objVM);
            }
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _tRepo.GetAllAsync(SD.TrailAPIPath)});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _tRepo.DeleteAsync(SD.TrailAPIPath, id);
            if (status)
            {
                return Json(new { success = true, message = "Data telah terhapus" });
            }
            return Json(new { succes = false, message = "Data gagal terhapus" });
        }
    }
}
