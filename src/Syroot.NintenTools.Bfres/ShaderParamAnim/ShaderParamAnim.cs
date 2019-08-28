using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Syroot.NintenTools.Bfres.Core;
using System.IO;

namespace Syroot.NintenTools.Bfres
{
    public enum ShaderParamAnimType
    {
        ShaderParameter,
        TextureSRT,
        Color
    }

    /// <summary>
    /// Represents an FSHU subfile in a <see cref="ResFile"/>, storing shader parameter animations of a
    /// <see cref="Model"/> instance.
    /// </summary>
    [DebuggerDisplay(nameof(ShaderParamAnim) + " {" + nameof(Name) + "}")]
    public class ShaderParamAnim : IResData
    {
        public ShaderParamAnim()
        {
            Name = "";
            Path = "";

            Flags = 0;
            FrameCount = 0;
            BakedSize = 0;
            BindModel = new Model();
            BindIndices = new ushort[0];

            ShaderParamMatAnims = new List<ShaderParamMatAnim>();
            UserData = new ResDict<UserData>();
        }
        
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHU";

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        internal ShaderParamAnimType ParamAnimType { get; set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="ResDict{ShaderParamAnim}"/> instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public ShaderParamAnimFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="AnimCurve"/> instances of all
        /// <see cref="ShaderParamMatAnims"/>.
        /// </summary>
        public uint BakedSize { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="Model"/> instance affected by this animation.
        /// </summary>
        public Model BindModel { get; set; }

        /// <summary>
        /// Gets the indices of the <see cref="Material"/> instances in the <see cref="Model.Materials"/> dictionary to
        /// bind for each animation. <see cref="UInt16.MaxValue"/> specifies no binding.
        /// </summary>
        public ushort[] BindIndices { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShaderParamMatAnim"/> instances creating the animation.
        /// </summary>
        public IList<ShaderParamMatAnim> ShaderParamMatAnims { get; set; }

        /// <summary>
        /// Gets or sets customly attached <see cref="UserData"/> instances.
        /// </summary>
        public ResDict<UserData> UserData { get; set; }

        public void Import(string FileName, ResFile ResFile, ShaderParamAnimType ParamAnimType)
        {
            using (ResFileLoader loader = new ResFileLoader(this, ResFile, FileName))
            {
                loader.ImportSection();
            }
        }

        public void Export(string FileName, ResFile ResFile, ShaderParamAnimType ParamAnimType)
        {
            using (ResFileSaver saver = new ResFileSaver(this, ResFile, FileName))
            {
                saver.ExportSection(ParamAnimType);
            }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        private uint unk;

        void IResData.Load(ResFileLoader loader)
        {
            if (loader.ResFile.Version >= 0x02040000)
            {
                loader.CheckSignature(_signature);
                Name = loader.LoadString();
                Path = loader.LoadString();
                Flags = loader.ReadEnum<ShaderParamAnimFlags>(true);

                ushort numMatAnim = 0;
                if (loader.ResFile.Version >= 0x03040000)
                {
                    FrameCount = loader.ReadInt32();
                    numMatAnim = loader.ReadUInt16();
                    ushort numUserData = loader.ReadUInt16();
                    int numParamAnim = loader.ReadInt32();
                    int numCurve = loader.ReadInt32();
                    BakedSize = loader.ReadUInt32();
                }
                else
                {
                    FrameCount = loader.ReadUInt16();
                    numMatAnim = loader.ReadUInt16();
                    unk = loader.ReadUInt32();
                    int numCurve = loader.ReadInt32();
                    BakedSize = loader.ReadUInt32();
                    int padding2 = loader.ReadInt32();
                }
                BindModel = loader.Load<Model>();
                BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numMatAnim));
                ShaderParamMatAnims = loader.LoadList<ShaderParamMatAnim>(numMatAnim);
                UserData = loader.LoadDict<UserData>();
            }
            else
            {
                Flags = loader.ReadEnum<ShaderParamAnimFlags>(true);
                FrameCount = loader.ReadInt16();
                ushort numMatAnim = loader.ReadUInt16();
                ushort numUserData = loader.ReadUInt16();
                ushort unk = loader.ReadUInt16();
                BakedSize = loader.ReadUInt32();
                Name = loader.LoadString();
                Path = loader.LoadString();
                BindModel = loader.Load<Model>();
                BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numMatAnim));
                ShaderParamMatAnims = loader.LoadList<ShaderParamMatAnim>(numMatAnim);
            }

        }

        internal long PosBindModelOffset;
        internal long PosBindIndicesOffset;
        internal long PosShaderParamMatAnimsOffset;
        internal long PosUserDataOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.SaveString(Path);
            saver.Write(Flags, true);
            if (saver.ResFile.Version >= 0x03040000)
            {
                saver.Write(FrameCount);
                saver.Write((ushort)ShaderParamMatAnims.Count);
                saver.Write((ushort)UserData.Count);

                int curveCount = ShaderParamMatAnims.Sum((x) => x.Curves.Count);
                foreach (var mat in ShaderParamMatAnims)
                    curveCount += mat.ParamAnimInfos.Sum((x) => x.ConstantCount);
                
                saver.Write(curveCount);
                saver.Write(ShaderParamMatAnims.Sum((x) => x.Curves.Count));
                saver.Write(BakedSize);
            }
            else
            {
                saver.Write((ushort)FrameCount);
                saver.Write((ushort)ShaderParamMatAnims.Count);
                saver.Write(unk);
                saver.Write(ShaderParamMatAnims.Sum((x) => x.Curves.Count));
                saver.Write(BakedSize);
                saver.Write(0);
            }

            PosBindModelOffset = saver.SaveOffsetPos();
            PosBindIndicesOffset = saver.SaveOffsetPos();
            PosShaderParamMatAnimsOffset = saver.SaveOffsetPos();
            PosUserDataOffset = saver.SaveOffsetPos();
        }
    }
    
    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum ShaderParamAnimFlags : uint
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
}