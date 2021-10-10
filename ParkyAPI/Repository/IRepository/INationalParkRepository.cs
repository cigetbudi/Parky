using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface INationalParkRepository
    {
        ICollection<NationalParkDTO> GetNationalParks();
        NationalParkDTO GetNationalPark(int nationalParkId);
        bool NationalParkExists(string name);
        bool NationalParkExists(int id);
        bool CreateNationalPark(NationalParkDTO nationalPark);
        bool UpdateNationalPark(NationalParkDTO nationalPark);
        bool DeleteNationalPark(NationalParkDTO nationalPark);
        bool Save();
    }
}
