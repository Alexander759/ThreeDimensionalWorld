using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Utility
{
    public class AllowedFormats
    {
        public const string Png = ".png";
        public const string Jpg = ".jpg";
        public const string Gif = ".gif";

        public const string Stl = ".stl";

        private static readonly string[] allowedImageFormats = [Png, Jpg, Gif];
        private static readonly string[] allowed3dFormats = [Stl];

        public static string[] AllowedImageFormats => allowedImageFormats;
        public static string[] Allowed3dFormats => allowed3dFormats;
        public static string[] AllAllowedFormats
        {
            get
            {
                List<string> allFormats = [.. allowedImageFormats, .. allowed3dFormats];
                return [.. allFormats];
            }
        }
    }
}
