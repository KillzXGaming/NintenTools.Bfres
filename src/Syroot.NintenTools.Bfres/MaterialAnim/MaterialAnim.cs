using System.Collections.Generic;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMAA section in a <see cref="ResFile"/> subfile, storing material animation data.
    /// </summary>
    public class MaterialAnim : IResData
    {
        private const string _signature = "FMAA";

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {

        }

        void IResData.Save(ResFileSaver saver)
        {

        }
    }
}
