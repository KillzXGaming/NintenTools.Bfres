using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.Bfres.Helpers;

namespace Syroot.NintenTools.Bfres.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ResFile resFile = new ResFile();

            resFile.Save("NewFile.bfres");
        }
    }
}
