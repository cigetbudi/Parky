using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{nationalParkId:int}")]
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
    }
}
