using Kataskopeya.EF;
using Kataskopeya.EF.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Kataskopeya.Services
{
    public class CamerasService
    {
        private readonly KataskopeyaContext _context;

        public CamerasService()
        {
            _context = new KataskopeyaContext();
        }

        public void SaveNewCamera(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentException(nameof(camera));
            }

            _context.Cameras.Add(camera);

            _context.SaveChanges();
        }

        public async Task<bool> IsCameraExists(string sourceUrl)
        {
            var camera = await _context.Cameras.FirstOrDefaultAsync(c => c.Url == sourceUrl);

            if (camera == null)
            {
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<Camera>> GetCameras()
        {
            var cameras = await _context.Cameras.ToListAsync();

            return cameras;
        }

        public async Task<bool> RemoveCamera(string url)
        {
            var camera = await _context.Cameras.FirstOrDefaultAsync(c => c.Url == url);

            if (camera != null)
            {
                _context.Cameras.Remove(camera);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
