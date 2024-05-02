using System.Runtime.InteropServices;
using InteropDotNet;

namespace ScanImeiApp.Tesseract;

/// <summary>
/// Work around legacy Tesseract Interop code
/// https://github.com/charlesw/tesseract/issues/503#issuecomment-1224633046
/// </summary>
public static class TesseractLinuxLoaderFix
{
    /// <summary>
    /// Override .so search path used on Linux by Tesseract
    /// </summary>
    public static void Patch()
    {
        // Only apply patch on Linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            LibraryLoader.Instance.CustomSearchPath = $"{AppDomain.CurrentDomain.BaseDirectory}/runtimes";
    }
}