using System.Windows.Forms;

namespace Kataskopeya.Common.Constants

{
    public static class DisplayData
    {
        public static int DisplayWidth
        {
            get
            {
                return Screen.PrimaryScreen.Bounds.Width;
            }
        }

        public static int DisplayHeight
        {
            get
            {
                return Screen.PrimaryScreen.Bounds.Height;
            }
        }
    }
}
