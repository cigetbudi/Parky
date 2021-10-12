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
    [ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private ITrailRepository _tRepo;
        private readonly IMapper _mapper;
        public TrailsController(ITrailRepository tRepo, IMapper mapper)
        {
            _tRepo = tRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Ambil semua list Trail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDTO>))]
        public IActionResult GetTrails()
        {
            var objList = _tRepo.GetTrails();
            var objDTO = new List<TrailDTO>();
            foreach (var obj in objList)
            {
                objDTO.Add(_mapper.Map<TrailDTO>(obj));
            }
            return Ok(objDTO);
        }

        [HttpGet("{trailId:int}", Name ="GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _tRepo.GetTrail(trailId);
            if (obj==null)
            {
                return NotFound();
            }
            var objDTO = _mapper.Map<TrailDTO>(obj);
            ////tanpa automapper
            //var objDTO = new TrailDTO()
            //{
            //    Created = obj.Created,
            //    Id = obj.Id,
            //    Name = obj.Name,
            //    State = obj.State
            //};
            return Ok(objDTO);
        }

        /// <summary>
        /// Create National Park
        /// </summary>
        /// <param name="trailDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDTO))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateTrail([FromBody] TrailCreateDTO trailDTO)
        {
            //kosong
            if (trailDTO == null)
            {
                return NotFound();
            }
            //duplikat
            if (_tRepo.TrailExists(trailDTO.Name))
            {
                ModelState.AddModelError("", "Nama sudah ada");
                return StatusCode(404, ModelState);
            }
            //lain-lain
            var tObj = _mapper.Map<Trail>(trailDTO);
            if (!_tRepo.CreateTrail(tObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat menyimpan {tObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute( "GetTrail", new {trailId = tObj.Id}, tObj);
        }

        /// <summary>
        /// Update Trail
        /// </summary>
        /// <param name="trailId"></param>
        /// <param name="trailDTO"></param>
        /// <returns></returns>
        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail (int trailId, [FromBody] TrailUpdateDTO trailDTO)
        {
            if (trailDTO==null || trailId != trailDTO.Id)
            {
                return BadRequest(ModelState);
            }

            //konversi model ke DTO
            var tObj = _mapper.Map<Trail>(trailDTO);
            if (!_tRepo.UpdateTrail(tObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat merubah {tObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        /// <summary>
        /// Hapus Trail
        /// </summary>
        /// <param name="trailId"> IDnya</param>
        /// <returns></returns>
        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail (int trailId)
        {
            if (!_tRepo.TrailExists(trailId))
            {
                return NotFound();
            }
            var tObj = _tRepo.GetTrail(trailId);
            if (!_tRepo.DeleteTrail(tObj))
            {
                ModelState.AddModelError("", $"ada kesalahan saat menghapus {tObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
