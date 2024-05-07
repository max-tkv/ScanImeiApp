using System.Runtime.InteropServices;
using InteropDotNet;

namespace ScanImeiApp.Tesseract;

/// <summary>
/// Работа с устаревшим кодом Tesseract Interop.
/// https://github.com/charlesw/tesseract/issues/503#issuecomment-1224633046
/// </summary>
public static class TesseractLinuxLoaderFix
{
    /// <summary>
    /// Переопределите путь поиска .so, используемый Tesseract в Linux
    /// </summary>
    public static void Patch()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LibraryLoader.Instance.CustomSearchPath = $"{AppDomain.CurrentDomain.BaseDirectory}/runtimes";   
        }
    }
}