using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Utility
{
    public class AllowedFormats
    {
        // Public constants for allowed image formats
        public const string Png = ".png";
        public const string Jpg = ".jpg";
        public const string Gif = ".gif";

        // Public constants for allowed 3D formats
        public const string Stl = ".stl";

        // Private static arrays for each type of format
        private static string[] allowedImageFormats = { Png, Jpg, Gif };
        private static string[] allowed3dFormats = { Stl };

        // Public properties to access the allowed formats
        public static string[] AllowedImageFormats => allowedImageFormats;
        public static string[] Allowed3dFormats => allowed3dFormats;
        public static string[] AllAllowedFormats
        {
            get
            {
                List<string> allFormats = new List<string>();
                allFormats.AddRange(allowedImageFormats);
                allFormats.AddRange(allowed3dFormats);
                return allFormats.ToArray();
            }
        }
    }
}
