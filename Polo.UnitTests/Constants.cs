using System.Collections.Generic;

namespace Polo.UnitTests
{
    public class Constants
    {
        public const string SourceFolderName = "Source Fotos";
        public const string DestinationFolderName = "Destination Fotos";
        public const string RawFolderName = "RAW";
        public const string VideoFolderName = "video";
    }

    public class FileExtension
    {
        public const string Jpg = ".jpg";
        public const string Jpeg = ".jpeg";

        public const string Orf = ".orf";
        public const string Crw = ".crw";
        public const string Cr2 = ".cr2";
        public const string Cr3 = ".cr3";
        public const string _3fr = ".3fr";
        public const string Mef = ".mef";
        public const string Nef = ".nef";
        public const string Nrw = ".nrw";
        public const string Pef = ".pef";
        public const string Ptx = ".ptx";
        public const string Rw2 = ".rw2";
        public const string Arw = ".arw";
        public const string Srf = ".srf";
        public const string Sr2 = ".sr2";
        public const string Gpr = ".gpr";
        public const string Raf = ".raf";
        public const string Raw = ".raw";
        public const string Rwl = ".rwl";
        public const string Dng = ".dng";
        public const string Srw = ".srw";
        public const string X3f = ".x3f";

        public const string Mkv = ".mkv";
        public const string Avi = ".avi";
        public const string M2ts = ".m2ts";
        public const string Ts = ".ts";
        public const string Mp4 = ".mp4";
        public const string M4v = ".m4v";
        public const string M4p = ".m4p";
        public const string Mpg = ".mpg";
        public const string Mpeg = ".mpeg";

        public static ICollection<string> JpegExtensions = new[] { Jpg, Jpeg };
        public static ICollection<string> RawExtensions = new[] { Orf, Crw, Cr2, Cr3, _3fr, Mef, Nef, Nrw, Pef, Ptx, Rw2, Arw, Srf, Sr2, Gpr, Raf, Raw, Rwl, Dng, Srw, X3f };
        public static ICollection<string> VideoExtensions = new[] { Mkv, Avi, M2ts, Ts, Mp4, M4v, M4p, Mpg, Mpeg };
    }
}
