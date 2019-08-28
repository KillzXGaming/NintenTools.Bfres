using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a vertex shape animation in a <see cref="ShapeAnim"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(VertexShapeAnim) + " {" + nameof(Name) + "}")]
    public class VertexShapeAnim : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the name of the animated <see cref="Shape"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="KeyShapeAnimInfo"/> instances.
        /// </summary>
        public IList<KeyShapeAnimInfo> KeyShapeAnimInfos { get; set; }

        /// <summary>
        /// Gets or sets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; set; }

        /// <summary>
        /// Gets or sets the list of base values, excluding the base shape (which is always being initialized with 0f).
        /// </summary>
        public float[] BaseDataList { get; set; }

        /// <summary>
        /// Gets or sets the index of the first <see cref="AnimCurve"/> relative to all curves of the parent
        /// <see cref="ShapeAnim.VertexShapeAnims"/> instances.
        /// </summary>
        internal int BeginCurve { get; set; }

        /// <summary>
        /// Gets or sets the index of the first <see cref="KeyShapeAnimInfo"/> relative to all key shape anim infos of
        /// the parent <see cref="ShapeAnim.VertexShapeAnims"/> instances.
        /// </summary>
        internal int BeginKeyShapeAnim { get; set; }

        private ushort unk;

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
            ushort numCurve;
            ushort numKeyShapeAnim;
            if (loader.ResFile.Version >= 0x03040000)
            {
                numCurve = loader.ReadUInt16();
                numKeyShapeAnim = loader.ReadUInt16();
            }
            else
            {
                numCurve = loader.ReadByte();
                numKeyShapeAnim = loader.ReadByte();
                unk = loader.ReadUInt16();
            }

            BeginCurve = loader.ReadInt32();
            BeginKeyShapeAnim = loader.ReadInt32();
            Name = loader.LoadString();
            KeyShapeAnimInfos = loader.LoadList<KeyShapeAnimInfo>(numKeyShapeAnim);
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseDataList = loader.LoadCustom(() => loader.ReadSingles(numKeyShapeAnim - 1)); // Without base shape.
        }


        internal long PosKeyShapeAnimInfosOffset;
        internal long PosCurvessOffset;
        internal long PosBaseDataListOffset;

        void IResData.Save(ResFileSaver saver)
        {
            if (saver.ResFile.Version >= 0x03040000)
            {
                saver.Write((ushort)Curves.Count);
                saver.Write((ushort)KeyShapeAnimInfos.Count);
            }
            else
            {
                saver.Write((byte)Curves.Count);
                saver.Write((byte)KeyShapeAnimInfos.Count);
                saver.Write((ushort)unk);
            }


            saver.Write(BeginCurve);
            saver.Write(BeginKeyShapeAnim);
            saver.SaveString(Name);
            PosKeyShapeAnimInfosOffset = saver.SaveOffsetPos();
            PosCurvessOffset = saver.SaveOffsetPos();
            PosBaseDataListOffset = saver.SaveOffsetPos();
        }
    }
}