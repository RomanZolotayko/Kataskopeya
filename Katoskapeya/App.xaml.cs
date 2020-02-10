using Kataskopeya.EF;
using Kataskopeya.Helpers;
using System.Windows;

namespace Kataskopeya
{
    public partial class App : Application
    {
        private KataskopeyaContext _context;

        public App()
        {
            _context = new KataskopeyaContext();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DbSeedHelper.SeedDatabase(_context);
        }
    }
}
