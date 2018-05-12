using System.Diagnostics;
using System.IO;
using Syroot.BinaryData;
using Syroot.NintenTools.Bfres.Core;

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
        /// Gets or sets the revision of the BFRES structure formats.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets the byte order in which data is stored. Must be the endianness of the target platform.
        /// </summary>
        public ByteOrder ByteOrder { get; private set; }

       

        /// <summary>
        /// Gets or sets the alignment to use for raw data blocks in the file.
        /// </summary>
        public uint Alignment { get; set; }

        /// <summary>
        /// Gets or sets the target adress size to use for raw data blocks in the file.
        /// </summary>
        public uint TargetAddressSize { get; set; }

        /// <summary>
        /// Gets or sets the flag. Unknown purpose.
        /// </summary>
        public uint Flag { get; set; }

        /// <summary>
        /// Gets or sets the BlockOffset. 
        /// </summary>
        public uint BlockOffset { get; set; }

        /// <summary>
        /// Gets or sets a name describing the contents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="RelocationTable"/> (_RLT) instance.
        /// </summary>
        public RelocationTable RelocationTable { get; set; }


        /// <summary>
        /// Gets or sets the stored <see cref="Model"/> (FMDL) instances.
        /// </summary>
        public ResDict<Model> Models { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="Texture"/> (FTEX) instances.
        /// </summary>
        public ResDict<Texture> Textures { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="SkeletalAnim"/> (FSKA) instances.
        /// </summary>
        public ResDict<SkeletalAnim> SkeletalAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances.
        /// </summary>
        public ResDict<ShaderParamAnim> ShaderParamAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for color animations.
        /// </summary>
        public ResDict<ShaderParamAnim> ColorAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="MaterialAnim"/> (FSHU) instances for color animations.
        /// </summary>
        public ResDict<MaterialAnim> MaterialAnim { get; set; }

        
        /// <summary>
        /// Gets or sets the stored <see cref="ShaderParamAnim"/> (FSHU) instances for texture SRT animations.
        /// </summary>
        public ResDict<ShaderParamAnim> TexSrtAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="TexPatternAnim"/> (FTXP) instances.
        /// </summary>
        public ResDict<TexPatternAnim> TexPatternAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="VisibilityAnim"/> (FVIS) instances for bone visibility animations.
        /// </summary>
        public ResDict<VisibilityAnim> BoneVisibilityAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="VisibilityAnim"/> (FVIS) instances for material visibility animations.
        /// </summary>
        public ResDict<VisibilityAnim> MatVisibilityAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="ShapeAnim"/> (FSHA) instances.
        /// </summary>
        public ResDict<ShapeAnim> ShapeAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="SceneAnim"/> (FSCN) instances.
        /// </summary>
        public ResDict<SceneAnim> SceneAnims { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="BufferMemoryPool"/> instances. 
        /// </summary>
        public BufferMemoryPool BufferMemoryPool { get; set; }

        /// <summary>
        /// Gets or sets the stored <see cref="BufferMemoryPoolInfo"/> instances.
        /// </summary>
        public BufferMemoryPoolInfo BufferMemoryPoolInfo { get; set; }


        /// <summary>
        /// Gets or sets attached <see cref="ExternalFile"/> instances. The key of the dictionary typically represents
        /// the name of the file they were originally created from.
        /// </summary>
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

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ByteOrder = ByteOrder.LittleEndian;

            loader.CheckSignature(_signature);
            uint padding = loader.ReadUInt32();
            Version = loader.ReadUInt32();
            ByteOrder = loader.ReadEnum<ByteOrder>(true);
            Alignment = loader.ReadByte();
            TargetAddressSize = loader.ReadByte();
            uint OffsetToFileName = loader.ReadUInt32();
            Flag = loader.ReadUInt16();
            BlockOffset = loader.ReadUInt16();
            RelocationTable = loader.Load<RelocationTable>();
            uint sizFile = loader.ReadUInt32();
            Name = loader.LoadString();
            Models = loader.LoadDict<Model>();
            Textures = loader.LoadDict<Texture>();
            SkeletalAnims = loader.LoadDict<SkeletalAnim>();
            MaterialAnim = loader.LoadDict<MaterialAnim>();
            BoneVisibilityAnims = loader.LoadDict<VisibilityAnim>();
            ShapeAnims = loader.LoadDict<ShapeAnim>();
            SceneAnims = loader.LoadDict<SceneAnim>();
            BufferMemoryPool = loader.Load<BufferMemoryPool>();
            BufferMemoryPoolInfo = loader.Load<BufferMemoryPoolInfo>();
            ExternalFiles = loader.LoadDict<ExternalFile>();
            uint sizStringPool = loader.ReadUInt32();
            uint ofsStringPool = loader.ReadOffset();
            ushort numModel = loader.ReadUInt16();
            ushort numSkeletalAnim = loader.ReadUInt16();
            ushort numMaterialAnim = loader.ReadUInt16();
            ushort numBoneVisibilityAnim = loader.ReadUInt16();
            ushort numShapeAnim = loader.ReadUInt16();
            ushort numSceneAnim = loader.ReadUInt16();
            ushort numExternalFile = loader.ReadUInt16();
            uint padding2 = loader.ReadUInt32();
            uint padding3 = loader.ReadUInt16();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            PreSave(); 
            
            saver.WriteSignature(_signature);
            saver.Write("    ");
            saver.Write(Version);
            saver.Write(ByteOrder, true);
            saver.Write(Alignment);
            saver.Write(TargetAddressSize);
            saver.Write(0); //Zero terminated filename
            saver.Write(Flag);
            saver.Write(BlockOffset);
            saver.Save(RelocationTable);
            saver.SaveFieldFileSize();
            saver.SaveString(Name);
            saver.SaveDict(Models);
            saver.SaveDict(SkeletalAnims);
            saver.SaveDict(MaterialAnim);
            saver.SaveDict(BoneVisibilityAnims);
            saver.SaveDict(ShapeAnims);
            saver.SaveDict(SceneAnims);
            saver.Save(BufferMemoryPool);
            saver.Save(BufferMemoryPoolInfo);
            saver.SaveDict(ExternalFiles);
            saver.SaveFieldStringPool();
            saver.Write((ushort)Models.Count);
            saver.Write((ushort)SkeletalAnims.Count);
            saver.Write((ushort)MaterialAnim.Count);
            saver.Write((ushort)BoneVisibilityAnims.Count);
            saver.Write((ushort)MatVisibilityAnims.Count);
            saver.Write((ushort)ShapeAnims.Count);
            saver.Write((ushort)SceneAnims.Count);
            saver.Write((ushort)ExternalFiles.Count);
            saver.Write((uint)0);
            saver.Write((ushort)0);
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
