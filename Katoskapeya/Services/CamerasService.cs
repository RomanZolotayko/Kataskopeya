using Kataskopeya.EF;
using Kataskopeya.EF.Models;
using System;

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
    }
}
