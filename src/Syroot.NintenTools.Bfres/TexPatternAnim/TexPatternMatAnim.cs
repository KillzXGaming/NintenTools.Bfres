using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a texture pattern material animation in a <see cref="TexPatternAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(TexPatternMatAnim) + " {" + nameof(Name) + "}")]
    public class TexPatternMatAnim : IResData
    {
        public TexPatternMatAnim()
        {
            Name = "";
            PatternAnimInfos = new List<PatternAnimInfo>();
            Curves = new List<AnimCurve>();
            BaseDataList = new List<ushort>();
            BeginCurve = 0;
            BeginPatAnim = 0;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the name of the animated <see cref="Material"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="PatternAnimInfo"/> instances.
        /// </summary>
        public IList<PatternAnimInfo> PatternAnimInfos { get; set; }

        /// <summary>
        /// Gets or sets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; set; }

        /// <summary>
        /// Gets or sets the initial <see cref="PatternAnimInfo"/> indices.
        /// </summary>
        public IList<ushort> BaseDataList { get; set; }

        /// <summary>
        /// Gets or sets the index of the first <see cref="AnimCurve"/> relative to all curves of the parent
        /// <see cref="TexPatternAnim.TexPatternMatAnims"/> instances.
        /// </summary>
        internal int BeginCurve { get; set; }

        /// <summary>
        /// Gets or sets the index of the first <see cref="PatternAnimInfo"/> relative to all param anim infos of the
        /// parent <see cref="TexPatternAnim.TexPatternMatAnims"/> instances.
        /// </summary>
        internal int BeginPatAnim { get; set; }

        public void Import(string FileName, ResFile ResFile)
        {
            using (ResFileLoader loader = new ResFileLoader(this, ResFile, FileName))
            {
                loader.ImportSection();
            }
        }

        public void Export(string FileName, ResFile ResFile)
        {
            using (ResFileSaver saver = new ResFileSaver(this, ResFile, FileName))
            {
                saver.ExportSection();
            }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ushort numPatAnim = loader.ReadUInt16();
            ushort numCurve = loader.ReadUInt16();
            BeginCurve = loader.ReadInt32();
            BeginPatAnim = loader.ReadInt32();
            Name = loader.LoadString();
            PatternAnimInfos = loader.LoadList<PatternAnimInfo>(numPatAnim);
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseDataList = loader.LoadCustom(() => loader.ReadUInt16s(numPatAnim));
        }

        internal long PosPatternAnimInfosOffset;
        internal long PosCurvessOffset;
        internal long PosBaseDataListOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.Write((ushort)PatternAnimInfos.Count);
            saver.Write((ushort)Curves.Count);
            saver.Write(BeginCurve);
            saver.Write(BeginPatAnim);
            saver.SaveString(Name);
            PosPatternAnimInfosOffset = saver.SaveOffsetPos();
            PosCurvessOffset = saver.SaveOffsetPos();
            PosBaseDataListOffset = saver.SaveOffsetPos();
        }
    }
}