using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FilteredTaskSwitcher.Win32
{
    public class DWM
    {
        private const int DWM_TNP_RECTDESTINATION = 0x1;
        private const int DWM_TNP_VISIBLE = 0x8;
        private const int DWM_TNP_SOURCECLIENTAREAONLY = 0x10;

        [DllImport("dwmapi.dll")]
        private static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        private static readonly List<IntPtr> ThumbHandles = new List<IntPtr>();

        public static void RegisterThumbnails(IntPtr source, IntPtr dest, Rect rect)
        {
            var result = DwmRegisterThumbnail(dest, source, out IntPtr thumbHandle);
            if (result == 0)
            {
                ThumbHandles.Add(thumbHandle);
                SetLocation(thumbHandle, rect);
            } else
            {
                MessageBox.Show($"Error! DwmRegisterThumbnail returned {result}");
            }
        }

        public static void UnregisterThumbnails()
        {
            foreach (var thumbHandle in ThumbHandles)
            {
                DwmUnregisterThumbnail(thumbHandle);
            }

            ThumbHandles.Clear();
        }

        private static void SetLocation(IntPtr thumbHandle, Rect rectangle)
        {
            DwmQueryThumbnailSourceSize(thumbHandle, out PSIZE size);

            CenterAndScaleRectangle(ref rectangle, size);

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                dwFlags = DWM_TNP_VISIBLE | DWM_TNP_RECTDESTINATION | DWM_TNP_SOURCECLIENTAREAONLY,
                fVisible = true,
                rcDestination = rectangle,
                fSourceClientAreaOnly = true
            };

            DwmUpdateThumbnailProperties(thumbHandle, ref props);
        }

        private static void CenterAndScaleRectangle(ref Rect rectangle, PSIZE size)
        {
            var height = rectangle.Height;
            var width = rectangle.Width;

            if (size.x < size.y)
            {
                double ScaleFactor = width / (double)size.y;
                int scaledX = (int)(size.x * ScaleFactor);
                int xOffset = (width - scaledX) / 2;
                rectangle.Left += xOffset;
                rectangle.Right -= xOffset;
            }

            if (size.y < size.x)
            {
                double ScaleFactor = height / (double)size.x;
                int scaledY = (int)(size.y * ScaleFactor);
                int yOffset = (height - scaledY) / 2;
                rectangle.Top += yOffset;
                rectangle.Bottom -= yOffset;
            }
        }
    }
}
