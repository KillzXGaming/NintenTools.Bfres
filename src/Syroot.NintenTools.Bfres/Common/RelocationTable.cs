using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an _RLT section in a <see cref="ResFile"/> subfile, storing pointers to sections in a bfres.
    /// </summary>  
    public class RelocationTable : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "_RLT";

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
    
        }

        void IResData.Save(ResFileSaver saver)
        {
      
        }
    }
}
