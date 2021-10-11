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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Ambil semua list National Parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<NationalParkDTO>))]
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

        /// <summary>
        /// Ambil 1 National Park by ID
        /// </summary>
        /// <param name="nationalParkId"> ID nya</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}",Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj==null)
            {
                return NotFound();
            }
            var objDTO = _mapper.Map<NationalParkDTO>(obj);
            ////tanpa automapper
            //var objDTO = new NationalParkDTO()
            //{
            //    Created = obj.Created,
            //    Id = obj.Id,
            //    Name = obj.Name,
            //    State = obj.State
            //};

            return Ok(objDTO);
        }

        /// <summary>
        /// Create national park
        /// </summary>
        /// <param name="nationalParkDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Update antional park
        /// </summary>
        /// <param name="nationalParkId"> ID nya</param>
        /// <param name="nationalParkDTO"></param>
        /// <returns></returns>
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Hapus National Park by ID
        /// </summary>
        /// <param name="nationalParkId"> ID nya</param>
        /// <returns></returns>
        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var npObj = _npRepo.GetNationalPark(nationalParkId);
            //error lain2
            if (!_npRepo.DeleteNationalPark(npObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat menghapus {npObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
