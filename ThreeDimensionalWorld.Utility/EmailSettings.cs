using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Utility
{
    public class EmailSettings
    {
        public required string SmtpServer { get; set; }
        public required int Port { get; set; }
        public required string Mail { get; set; }
        public required string Password { get; set; }
    }
}
