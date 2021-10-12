using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;
        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails.Include(o => o.NationalPark).FirstOrDefault(o => o.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(o => o.NationalPark).OrderBy(o => o.Name).ToList();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
           return  _db.Trails.Include(o => o.NationalPark).Where( o => o.NationalParkId == npId).ToList();
        }

        public bool Save()
        {

            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool TrailExists(string name)
        {
            bool value = _db.Trails.Any(o => o.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int id)
        {
            return _db.Trails.Any(o => o.Id == id);
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }
    }
}
