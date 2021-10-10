using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;
        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateNationalPark(NationalParkDTO nationalPark)
        {
            _db.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalParkDTO nationalPark)
        {
            _db.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalParkDTO GetNationalPark(int nationalParkId)
        {
            return _db.NationalParks.FirstOrDefault(o => o.Id == nationalParkId);
        }

        public ICollection<NationalParkDTO> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(o => o.Name).ToList();
        }

        public bool NationalParkExists(int id)
        {
            return _db.NationalParks.Any(a => a.Id == id);
        }

        public bool NationalParkExists(string name)
        {
            bool value = _db.NationalParks.Any(o => o.Name.ToLower().Trim() == name.ToLower());
            return value;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalParkDTO nationalPark)
        {
            _db.NationalParks.Update(nationalPark);
            return Save();
        }
    }
}
