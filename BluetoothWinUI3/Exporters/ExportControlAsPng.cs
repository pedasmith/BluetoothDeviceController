using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Exporters
{
    internal class ExportControlAsPng
    {
        public async Task<InMemoryRandomAccessStream> ExportAsync(Control control, Brush background)
        {
            // RenderTargetBitmap: https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.rendertargetbitmap?view=winrt-28000
            // RenderAsync() https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.rendertargetbitmap.renderasync?view=winrt-28000
            // GetPixelsAsync() https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.rendertargetbitmap.getpixelsasync?view=winrt-28000
            // Format is BGRA8 premultiplied

            // Win2D requires B8G8R8A8UIntNormalized
            // which is DXGI_FORMAT_B8G8R8A8_UNORM https://learn.microsoft.com/en-us/uwp/api/windows.graphics.directx.directxpixelformat
            // which is described at https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format
            // as "A four-component, 32-bit unsigned-normalized-integer format that supports 8 bits for each color channel and 8-bit alpha"

            var renderTargetBitmap = new Microsoft.UI.Xaml.Media.Imaging.RenderTargetBitmap();
            var oldBackground = control.Background;
            control.Background = background; // rootPanel.Background
            await renderTargetBitmap.RenderAsync(control);
            control.Background = oldBackground; // switch back to transparent!

            var pixels = await renderTargetBitmap.GetPixelsAsync(); // get an IBuffer in BGRA8 premultiplied format.
            var pixelsArray = pixels.ToArray();

            var outputStream = new InMemoryRandomAccessStream();
            var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, outputStream);
            encoder.SetPixelData(
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                96, // DPI X
                96, // DPI Y
                pixelsArray
            );
            await encoder.FlushAsync(); // catch this one level up
            return outputStream;
        }

    }
}
