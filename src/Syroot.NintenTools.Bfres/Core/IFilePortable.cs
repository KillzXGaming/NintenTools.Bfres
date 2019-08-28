using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syroot.NintenTools.Bfres.Core
{
    /// <summary>
    /// Represents the common interface for exporting <see cref="ResFile"/> data instances.
    /// </summary>
    public interface IFilePortable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Loads raw data from the <paramref name="loader"/> data stream into instances.
        /// </summary>
        /// <param name="loader">The <see cref="ResFileLoader"/> to load data with.</param>
        void Export(string FileName, ResFile ResFile);

        /// <summary>
        /// Saves header data of the instance and queues referenced data in the given <paramref name="saver"/>.
        /// </summary>
        /// <param name="saver">The <see cref="ResFileSaver"/> to save headers and queue data with.</param>
        void Import(string FileName, ResFile ResFile);
    }
}
