using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FTXP subfile in a <see cref="ResFile"/>, storing texture material pattern animations.
    /// </summary>
    [DebuggerDisplay(nameof(TexPatternAnim) + " {" + nameof(Name) + "}")]
    public class TexPatternAnim : IResData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeAnim"/> class.
        /// </summary>
        public TexPatternAnim()
        {
            Name = "";
            Path = "";
            Flags = 0;
            BindModel = new Model();
            BindIndices = new ushort[0];
            TexPatternMatAnims = new List<TexPatternMatAnim>();
            FrameCount = 0;
            BakedSize = 0;
            BindIndices = new ushort[0];
            UserData = new ResDict<UserData>();
            TextureRefs = new ResDict<TextureRef>();
            TextureRefNames = new List<TextureRef>();
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FTXP";
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in
        /// <see cref="ResDict{TexPatternAnim}"/> instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets flags controlling how animation data is stored or how the animation should be played.
        /// </summary>
        public TexPatternAnimFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames this animation plays.
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes required to bake all <see cref="AnimCurve"/> instances of all
        /// <see cref="TexPatternMatAnims"/>.
        /// </summary>
        public uint BakedSize { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model"/> instance affected by this animation.
        /// </summary>
        public Model BindModel { get; set; }

        /// <summary>
        /// Gets or sets the indices of the <see cref="Material"/> instances in the <see cref="Model.Materials"/>
        /// dictionary to bind for each animation. <see cref="UInt16.MaxValue"/> specifies no binding.
        /// </summary>
        public ushort[] BindIndices { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TexPatternAnim"/> instances creating the animation.
        /// </summary>
        public IList<TexPatternMatAnim> TexPatternMatAnims { get; set; }
        

        /// <summary>
        /// Gets or sets the <see cref="TextureRef"/> instances pointing to <see cref="Texture"/> instances
        /// participating in the animation.
        /// </summary>
        public ResDict<TextureRef> TextureRefs { get; set; }

        /// <summary>
        /// Note used for older bfres files
        /// Gets or sets the <see cref="TextureRef"/> instances pointing to <see cref="Texture"/> instances
        /// participating in the animation.
        /// </summary>
        public IList<TextureRef> TextureRefNames { get; set; }

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
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Path = loader.LoadString();
            Flags = loader.ReadEnum<TexPatternAnimFlags>(true);
            ushort numMatAnim = 0;
            ushort numTextureRef = 0;
            if (loader.ResFile.Version >= 0x03040000)
            {
                ushort numUserData = loader.ReadUInt16();
                FrameCount = loader.ReadInt32();
                numTextureRef = loader.ReadUInt16();
                numMatAnim = loader.ReadUInt16();
                int numPatAnim = loader.ReadInt32();
                int numCurve = loader.ReadInt32();
                BakedSize = loader.ReadUInt32();
            }
            else
            {
                FrameCount = loader.ReadUInt16();
                numTextureRef = loader.ReadUInt16();
                numMatAnim = loader.ReadUInt16();
                ushort numUserData = loader.ReadUInt16();
           //     loader.Seek(2); //padding
                int numPatAnim = loader.ReadInt16();
                int numCurve = loader.ReadInt32();
                BakedSize = loader.ReadUInt32();
                loader.Seek(4); //padding
            }


            BindModel = loader.Load<Model>();
            BindIndices = loader.LoadCustom(() => loader.ReadUInt16s(numMatAnim));
            TexPatternMatAnims = loader.LoadList<TexPatternMatAnim>(numMatAnim);
            if (loader.ResFile.Version >= 0x03040000)
                TextureRefs = loader.LoadDict<TextureRef>();
            else
            {
                int TextureCount = 0;
                foreach (var patternAnim in TexPatternMatAnims)
                {
                    foreach (var curve in patternAnim.Curves)
                    {
                        List<uint> frames = new List<uint>();
                        foreach (float key in curve.Keys)
                        {
                      //      Console.WriteLine((uint)key);
                            frames.Add((uint)key);
                        }
                        TextureCount = (short)frames.Max();
                        /*        
                              for (int i = 0; i < (ushort)curve.Frames.Length; i++)
                              {
                                  if (curve.Scale != 0)
                                  {
                                      int test = (int)curve.Keys[i, 0];
                                      float key = curve.Offset + test * curve.Scale;
                                      frames.Add((int)key);
                                  }
                                  else
                                  {
                                      float test = curve.Keys[i, 0];
                                      int key = curve.Offset + (int)test;
                                      frames.Add((int)key);

                                      int testCeiling = (int)Math.Ceiling(test);
                                      int testFloor = (int)Math.Floor(test);
                                      int testRound = (int)Math.Round(test);

                                      Console.WriteLine("convert int = {0}", (Decimal10x5)test);
                                  }
                              }*/
                    }
                }
                Console.WriteLine(Name + " Tex Total " + (TextureCount + 1));

                TextureRefNames = loader.LoadList<TextureRef>(numTextureRef);
            }
            UserData = loader.LoadDict<UserData>();
        }

        internal long PosBindModelOffset;
        internal long PosBindIndicesOffset;
        internal long PosTexPatternMatAnims;
        internal long PosTexureListOffset;
        internal long PosUserDataOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.SaveString(Path);
            saver.Write(Flags, true);
            if (saver.ResFile.Version >= 0x03040000)
            {
                saver.Write((ushort)UserData.Count);
                saver.Write(FrameCount);
                saver.Write((ushort)TextureRefs.Count);
                saver.Write((ushort)TexPatternMatAnims.Count);
                saver.Write(TexPatternMatAnims.Sum((x) => x.PatternAnimInfos.Count));
                saver.Write(TexPatternMatAnims.Sum((x) => x.Curves.Count));
                saver.Write(BakedSize);
            }
            else
            {
                if (TextureRefs == null)
                    TextureRefs = new ResDict<TextureRef>();

                saver.Write((ushort)FrameCount);
                saver.Write((ushort)TextureRefNames.Count);
                saver.Write((ushort)TexPatternMatAnims.Count);
                saver.Write((ushort)UserData.Count);
                saver.Write((ushort)TexPatternMatAnims.Sum((x) => x.PatternAnimInfos.Count));
                saver.Write(TexPatternMatAnims.Sum((x) => x.Curves.Count));
                saver.Write(BakedSize);
                saver.Seek(4);
            }

            PosBindModelOffset = saver.SaveOffsetPos();
            PosBindIndicesOffset = saver.SaveOffsetPos();
            PosTexPatternMatAnims = saver.SaveOffsetPos();
            PosTexureListOffset = saver.SaveOffsetPos();
            PosUserDataOffset = saver.SaveOffsetPos();
        }
    }

    /// <summary>
    /// Represents flags specifying how animation data is stored or should be played.
    /// </summary>
    [Flags]
    public enum TexPatternAnimFlags : ushort
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