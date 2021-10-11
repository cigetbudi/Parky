using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();
            var objDTO = new List<NationalParkDTO>();

            foreach (var obj in objList)
            {
                objDTO.Add(_mapper.Map<NationalParkDTO>(obj));
            }
            return Ok(objDTO);
        }

        [HttpGet("{nationalParkId:int}",Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj==null)
            {
                return NotFound();
            }
            var objDTO = _mapper.Map<NationalParkDTO>(obj);
            return Ok(objDTO);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            //kalo kosong
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }
            //kalo duplikat
            if (_npRepo.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError("","Nama sudah ada");
                return StatusCode(404, ModelState);
            }
            ////kalo ada row yang ga sesuai
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //kalo error lain2
            var npObj = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_npRepo.CreateNationalPark(npObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat menyimpan {npObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId=npObj.Id }, npObj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkId != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }
           
            //konvert model ke DTO
            var npObj = _mapper.Map<NationalPark>(nationalParkDTO);
            //error lain2
            if (!_npRepo.UpdateNationalPark(npObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat menyimpan {npObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
