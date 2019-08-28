using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FVIS subfile in a <see cref="ResFile"/>, storing visibility animations of <see cref="Bone"/> or
    /// <see cref="Material"/> instances.
    /// </summary>
    [DebuggerDisplay(nameof(VisibilityAnim) + " {" + nameof(Name) + "}")]
    public class VisibilityAnim : IResData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibilityAnim"/> class.
        /// </summary>
        public VisibilityAnim()
        {
            Name = "";
            Path = "";
            Flags = 0;
            FrameCount = 0;
            BakedSize = 0;
            Curves = new List<AnimCurve>();
            BindIndices = new ushort[0];
            Names = new List<string>();
            BaseDataList = new bool[0];
            UserData = new ResDict<UserData>();
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FVIS";

        private const ushort _flagsMask = 0b00000000_00000111;
        private const ushort _flagsMaskType = 0b00000001_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------
        
        private ushort _flags;
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="ResDict{VisibilityAnim}"/> instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public VisibilityAnimFlags Flags
        {
            get { return (VisibilityAnimFlags)(_flags & _flagsMask); }
            set { _flags = (ushort)(_flags & ~_flagsMask | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the kind of data the animation controls.
        /// </summary>
        public VisibilityAnimType Type
        {
            get { return (VisibilityAnimType)(_flags & _flagsMaskType); }
            set { _flags = (ushort)(_flags & ~_flagsMaskType | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="Curves"/>.
        /// </summary>
        public uint BakedSize { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model"/> instance affected by this animation.
        /// </summary>
        public Model BindModel { get; set; }

        /// <summary>
        /// Gets or sets the indices of entries in the <see cref="Skeleton.Bones"/> or <see cref="Model.Materials"/>
        /// dictionaries to bind to for each animation. <see cref="UInt16.MaxValue"/> specifies no binding.
        /// </summary>
        public ushort[] BindIndices { get; set; }

        /// <summary>
        /// Gets or sets the names of entries in the <see cref="Skeleton.Bones"/> or <see cref="Model.Materials"/>
        /// dictionaries to bind to for each animation.
        /// </summary>
        public IList<string> Names { get; set; }

        /// <summary>
        /// Gets or sets <see cref="AnimCurve"/> instances animating properties of objects stored in this section.
        /// </summary>
        public IList<AnimCurve> Curves { get; set; }

        /// <summary>
        /// Gets or sets boolean values storing the initial visibility for each <see cref="Bone"/> or
        /// <see cref="Material"/>.
        /// </summary>
        public bool[] BaseDataList { get; set; }

        /// <summary>
        /// Gets or sets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public ResDict<UserData> UserData { get; set; }

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
            if (loader.ResFile.Version >= 0x03040000)
                loader.CheckSignature(_signature);
            else
                loader.ReadChars(4);
            Name = loader.LoadString();
            Path = loader.LoadString();
            _flags = loader.ReadUInt16();
            ushort numAnim = 0;
            ushort numCurve = 0;
            if (loader.ResFile.Version >= 0x03040000)
            {
                ushort numUserData = loader.ReadUInt16();
                FrameCount = loader.ReadInt32();
                numAnim = loader.ReadUInt16();
                numCurve = loader.ReadUInt16();
                BakedSize = loader.ReadUInt32();
            }
            else
            {
                FrameCount = loader.ReadInt16();
                numAnim = loader.ReadUInt16();
                numCurve = loader.ReadUInt16();
                ushort numUserData = loader.ReadUInt16();
                BakedSize = loader.ReadUInt32();
                int padding2 = loader.ReadInt16();
            }
            BindModel = loader.Load<Model>();
            BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numAnim));
            Names = loader.LoadCustom(() => loader.LoadStrings(numAnim)); // Offset to name list.
            Curves = loader.LoadList<AnimCurve>(numCurve);
            BaseDataList = loader.LoadCustom(() =>
            {
                bool[] baseData = new bool[numAnim];
                int i = 0;
                while (i < numAnim)
                {
                    byte b = loader.ReadByte();
                    for (int j = 0; j < 8 && i < numAnim; j++)
                    {
                        baseData[i++] = b.GetBit(j);
                    }
                }
                return baseData;
            });
            UserData = loader.LoadDict<UserData>();
        }

        internal long PosBindModelOffset;
        internal long PosBindIndicesOffset;
        internal long PosNamesOffset;
        internal long PosCurvesOffset;
        internal long PosBaseDataOffset;
        internal long PosUserDataOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.SaveString(Path);
            saver.Write(_flags);
            if (saver.ResFile.Version >= 0x03040000)
            {
                saver.Write((ushort)UserData.Count);
                saver.Write(FrameCount);
                saver.Write((ushort)Names.Count);
                saver.Write((ushort)Curves.Count);
                saver.Write(BakedSize);
            }
            else
            {
                saver.Write((ushort)FrameCount);
                saver.Write((ushort)Names.Count);
                saver.Write((ushort)Curves.Count);
                saver.Write((ushort)UserData.Count);
                saver.Write(BakedSize);
                saver.Write((ushort)0);
            }

            PosBindModelOffset = saver.SaveOffsetPos();
            PosBindIndicesOffset = saver.SaveOffsetPos();
            PosNamesOffset = saver.SaveOffsetPos();
            PosCurvesOffset = saver.SaveOffsetPos();
            PosBaseDataOffset = saver.SaveOffsetPos();
            PosUserDataOffset = saver.SaveOffsetPos();
        }
    }
    
    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum VisibilityAnimFlags : ushort
    {
        /// <summary>
        /// The stored curve data has been baked.
        /// </summary>
        BakedCurve = 1 << 0,

        /// <summary>
        /// The animation repeats from the start after the last frame has been played.
        /// </summary>
        Looping = 1 << 2
    }

    /// <summary>
    /// Represents the kind of data the visibility animation controls.
    /// </summary>
    public enum VisibilityAnimType : ushort
    {
        /// <summary>
        /// Bone visiblity is controlled.
        /// </summary>
        Bone,

        /// <summary>
        /// Material visibility is controlled.
        /// </summary>
        Material = 1 << 8
    }
}