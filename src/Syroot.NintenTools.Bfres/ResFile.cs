using System.Diagnostics;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres.Core;
using System.ComponentModel;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a NintendoWare for Cafe (NW4F) graphics data archive file.
    /// </summary>
    [DebuggerDisplay(nameof(ResFile) + " {" + nameof(Name) + "}")]
    public class ResFile : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FRES";

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class.
        /// </summary>
        public ResFile()
        {
            System.Console.WriteLine("Creating res file....");

            Name = "";
            Alignment = 8192;

            VersionMajor = 3;
            VersionMajor2 = 4;
            VersionMinor = 0;
            VersionMinor2 = 4;

            //Initialize Dictionaries
            Models = new ResDict<Model>();
            Textures = new ResDict<Texture>();
            SkeletalAnims = new ResDict<SkeletalAnim>();
            ShaderParamAnims = new ResDict<ShaderParamAnim>();
            ColorAnims = new ResDict<ShaderParamAnim>();
            TexSrtAnims = new ResDict<ShaderParamAnim>();
            TexPatternAnims = new ResDict<TexPatternAnim>();
            BoneVisibilityAnims = new ResDict<VisibilityAnim>();
            MatVisibilityAnims = new ResDict<VisibilityAnim>();
            ShapeAnims = new ResDict<ShapeAnim>();
            SceneAnims = new ResDict<SceneAnim>();
            ExternalFiles = new ResDict<ExternalFile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class from the given <paramref name="stream"/> which
        /// is optionally left open.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after reading, otherwise <c>false</c>.</param>
        public ResFile(Stream stream, bool leaveOpen = false)
        {
            using (ResFileLoader loader = new ResFileLoader(this, stream, leaveOpen))
            {
                loader.Execute();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResFile"/> class from the file with the given
        /// <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public ResFile(string fileName)
        {
            using (ResFileLoader loader = new ResFileLoader(this, fileName))
            {
                loader.Execute();
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the alignment to use for raw data blocks in the file.
        /// </summary>
        [Browsable(true)]
        [Category("Binary Info")]
        [DisplayName("Alignment")]
        public uint Alignment { get; set; }

        /// <summary>
        /// Gets or sets a name describing the contents.
        /// </summary>
        [Browsable(true)]
        [Category("Binary Info")]
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the revision of the BFRES structure formats.
        /// </summary>
        internal uint Version { get; set; }

        /// <summary>
        /// Gets or sets the major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Version")]
        [DisplayName("Version Major")]
        public string VersioFull
        {
            get
            {
                return $"{VersionMajor},{VersionMajor2},{VersionMinor},{VersionMinor2}";
            }
        }

        /// <summary>
        /// Gets or sets the major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Major")]
        public uint VersionMajor { get; set; }
        /// <summary>
        /// Gets or sets the second major revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Major 2")]
        public uint VersionMajor2 { get; set; }
        /// <summary>
        /// Gets or sets the minor revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Minor")]
        public uint VersionMinor { get; set; }
        /// <summary>
        /// Gets or sets the second minor revision of the BFRES structure formats.
        /// </summary>
        [Browsable(true)]
        [Category("Version")]
        [DisplayName("Version Minor 2")]
        public uint VersionMinor2 { get; set; }

        /// <summary>
        /// Gets the byte order in which data is stored. Must be the endianness of the target platform.
        /// </summary>
        [Browsable(false)]
        public ByteOrder ByteOrder { get; private set; } = ByteOrder.BigEndian;

        /// <summary>
        /// Gets or sets the stored <see cref="Model"/> (FMDL) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<Model> Models { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="Texture"/> (FTEX) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<Texture> Textures { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="SkeletalAnim"/> (FSKA) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<SkeletalAnim> SkeletalAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<ShaderParamAnim> ShaderParamAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for color animations.
        /// </summary>
        [Browsable(false)]
        public ResDict<ShaderParamAnim> ColorAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for texture SRT animations.
        /// </summary>
        [Browsable(false)]
        public ResDict<ShaderParamAnim> TexSrtAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="TexPatternAnim"/> (FTXP) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<TexPatternAnim> TexPatternAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="VisibilityAnim"/> (FVIS) instances for bone visibility animations.
        /// </summary>
        [Browsable(false)]
        public ResDict<VisibilityAnim> BoneVisibilityAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="VisibilityAnim"/> (FVIS) instances for material visibility animations.
        /// </summary>
        [Browsable(false)]
        public ResDict<VisibilityAnim> MatVisibilityAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShapeAnim"/> (FSHA) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<ShapeAnim> ShapeAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="SceneAnim"/> (FSCN) instances.
        /// </summary>
        [Browsable(false)]
        public ResDict<SceneAnim> SceneAnims { get; set; }

        /// <summary>
        /// Gets or sets attached <see cref="ExternalFile"/> instances. The key of the dictionary typically represents
        /// the name of the file they were originally created from.
        /// </summary>
        [Browsable(false)]
        public ResDict<ExternalFile> ExternalFiles { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Saves the contents in the given <paramref name="stream"/> and optionally leaves it open
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the contents into.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after writing, otherwise <c>false</c>.</param>
        public void Save(Stream stream, bool leaveOpen = false)
        {
            using (ResFileSaver saver = new ResFileSaver(this, stream, leaveOpen))
            {
                saver.Execute();
            }
        }

        /// <summary>
        /// Saves the contents in the file with the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to save the contents into.</param>
        public void Save(string fileName)
        {
            using (ResFileSaver saver = new ResFileSaver(this, fileName))
            {
                saver.Execute();
            }
        }

        internal void SetVersionInfo(uint Version)
        {
            VersionMajor = Version >> 24;
            VersionMajor2 = Version >> 16 & 0xFF;
            VersionMinor = Version >> 8 & 0xFF;
            VersionMinor2 = Version & 0xFF;
        }

        internal uint SaveVersion()
        {
            return VersionMajor << 24 | VersionMajor2 << 16 | VersionMinor << 8 | VersionMinor2;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Version = loader.ReadUInt32();
            SetVersionInfo(Version);

            ByteOrder = loader.ReadEnum<ByteOrder>(true);
            ushort sizHeader = loader.ReadUInt16();
            uint sizFile = loader.ReadUInt32();
            Alignment = loader.ReadUInt32();
            Name = loader.LoadString();
            uint sizStringPool = loader.ReadUInt32();
            uint ofsStringPool = loader.ReadOffset();
            Models = loader.LoadDict<Model>();
            Textures = loader.LoadDict<Texture>();
            SkeletalAnims = loader.LoadDict<SkeletalAnim>();
            ShaderParamAnims = loader.LoadDict<ShaderParamAnim>();
            ColorAnims = loader.LoadDict<ShaderParamAnim>();
            TexSrtAnims = loader.LoadDict<ShaderParamAnim>();
            TexPatternAnims = loader.LoadDict<TexPatternAnim>();
            BoneVisibilityAnims = loader.LoadDict<VisibilityAnim>();
            MatVisibilityAnims = loader.LoadDict<VisibilityAnim>();
            ShapeAnims = loader.LoadDict<ShapeAnim>();

            if (loader.ResFile.Version >= 0x02040000)
            {
                SceneAnims = loader.LoadDict<SceneAnim>();
                ExternalFiles = loader.LoadDict<ExternalFile>();
                ushort numModel = loader.ReadUInt16();
                ushort numTexture = loader.ReadUInt16();
                ushort numSkeletalAnim = loader.ReadUInt16();
                ushort numShaderParamAnim = loader.ReadUInt16();
                ushort numColorAnim = loader.ReadUInt16();
                ushort numTexSrtAnim = loader.ReadUInt16();
                ushort numTexPatternAnim = loader.ReadUInt16();
                ushort numBoneVisibilityAnim = loader.ReadUInt16();
                ushort numMatVisibilityAnim = loader.ReadUInt16();
                ushort numShapeAnim = loader.ReadUInt16();
                ushort numSceneAnim = loader.ReadUInt16();
                ushort numExternalFile = loader.ReadUInt16();
                uint userPointer = loader.ReadUInt32();
            }
            else //Note very old versions have no counts and is mostly unkown atm
            {
                uint userPointer = loader.ReadUInt32();
                uint userPointer2 = loader.ReadUInt32();

                SceneAnims = loader.LoadDict<SceneAnim>();
                ExternalFiles = loader.LoadDict<ExternalFile>();
            }

        }

        internal long ModelOffset = 0;
        internal long TextureOffset = 0;
        internal long SkeletonAnimationOffset = 0;
        internal long ShaderParamAnimationOffset = 0;
        internal long ColorParamAnimationOffset = 0;
        internal long TexSrtParamAnimationOffset = 0;
        internal long TexPatParamAnimationOffset = 0;
        internal long BoneVisAnimationOffset = 0;
        internal long MatVisAnimationOffset = 0;
        internal long ShapeAnimationOffset = 0;
        internal long SceneAnimationOffset = 0;
        internal long ExternalFileOffset = 0;

        void IResData.Save(ResFileSaver saver)
        {
            PreSave(); 
            
            saver.WriteSignature(_signature);
            saver.Write(SaveVersion());
            saver.Write(ByteOrder.BigEndian, true);
            saver.Write((ushort)0x0010); // SizHeader
            saver.SaveFieldFileSize();
            saver.Write(Alignment);
            saver.SaveString(Name);
            saver.SaveFieldStringPool();

            ModelOffset = saver.SaveOffsetPos();
            TextureOffset = saver.SaveOffsetPos();
            SkeletonAnimationOffset = saver.SaveOffsetPos();
            ShaderParamAnimationOffset = saver.SaveOffsetPos();
            ColorParamAnimationOffset = saver.SaveOffsetPos();
            TexSrtParamAnimationOffset = saver.SaveOffsetPos();
            TexPatParamAnimationOffset = saver.SaveOffsetPos();
            BoneVisAnimationOffset = saver.SaveOffsetPos();
            MatVisAnimationOffset = saver.SaveOffsetPos();
            ShapeAnimationOffset = saver.SaveOffsetPos();
            SceneAnimationOffset = saver.SaveOffsetPos();
            ExternalFileOffset = saver.SaveOffsetPos();
            saver.Write((ushort)Models.Count);
            saver.Write((ushort)Textures.Count);
            saver.Write((ushort)SkeletalAnims.Count);
            saver.Write((ushort)ShaderParamAnims.Count);
            saver.Write((ushort)ColorAnims.Count);
            saver.Write((ushort)TexSrtAnims.Count);
            saver.Write((ushort)TexPatternAnims.Count);
            saver.Write((ushort)BoneVisibilityAnims.Count);
            saver.Write((ushort)MatVisibilityAnims.Count);
            saver.Write((ushort)ShapeAnims.Count);
            saver.Write((ushort)SceneAnims.Count);
            saver.Write((ushort)ExternalFiles.Count);
            saver.Write(0); // UserPointer
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------
        
        private void PreSave()
        {
            // Update Shape instances.
            foreach (Model model in Models.Values)
            {
                foreach (Shape shape in model.Shapes.Values)
                {
                    shape.VertexBuffer = model.VertexBuffers[shape.VertexBufferIndex];
                }
            }

            // Update SkeletalAnim instances.
            foreach (SkeletalAnim anim in SkeletalAnims.Values)
            {
                int curveIndex = 0;
                foreach (BoneAnim subAnim in anim.BoneAnims)
                {
                    subAnim.BeginCurve = curveIndex;
                    curveIndex += subAnim.Curves.Count;
                }
            }

            // Update TexPatternAnim instances.
            foreach (TexPatternAnim anim in TexPatternAnims.Values)
            {
                int curveIndex = 0;
                int infoIndex = 0;
                foreach (TexPatternMatAnim subAnim in anim.TexPatternMatAnims)
                {
                    subAnim.BeginCurve = curveIndex;
                    subAnim.BeginPatAnim = infoIndex;
                    curveIndex += subAnim.Curves.Count;
                    infoIndex += subAnim.PatternAnimInfos.Count;
                }
            }

            // Update ShaderParamAnim instances.
            foreach (ShaderParamAnim anim in ShaderParamAnims.Values)
            {
                int curveIndex = 0;
                int infoIndex = 0;
                foreach (ShaderParamMatAnim subAnim in anim.ShaderParamMatAnims)
                {
                    subAnim.BeginCurve = curveIndex;
                    subAnim.BeginParamAnim = infoIndex;
                    curveIndex += subAnim.Curves.Count;
                    infoIndex += subAnim.ParamAnimInfos.Count;
                }
            }

            // Update ShapeAnim instances.
            foreach (ShapeAnim anim in ShapeAnims.Values)
            {
                int curveIndex = 0;
                int infoIndex = 0;
                foreach (VertexShapeAnim subAnim in anim.VertexShapeAnims)
                {
                    subAnim.BeginCurve = curveIndex;
                    subAnim.BeginKeyShapeAnim = infoIndex;
                    curveIndex += subAnim.Curves.Count;
                    infoIndex += subAnim.KeyShapeAnimInfos.Count;
                }
            }
        }
    }
}
