using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;
using Syroot.NintenTools.Bfres.GX2;
using System.ComponentModel;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ResFile"/>, storing multi-dimensional texture data.
    /// </summary>
    [DebuggerDisplay(nameof(Texture) + " {" + nameof(Name) + "}")]
    public class Texture : IResData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        public Texture()
        {
            CompSelR = GX2CompSel.ChannelR;
            CompSelG = GX2CompSel.ChannelG;
            CompSelB = GX2CompSel.ChannelB;
            CompSelA = GX2CompSel.ChannelA;

            Name = "";
            Path = "";
            Width = 0;
            Height = 0;
            Depth = 1;
            Swizzle = 0;
            Alignment = 4096;
            ArrayLength = 1;
            Pitch = 32;
            TileMode = GX2TileMode.Mode2dTiledThin1;
            AAMode = GX2AAMode.Mode1X;
            Dim = GX2SurfaceDim.Dim2D;
            Format = GX2SurfaceFormat.T_BC1_SRGB;

            Data = new byte[0];
            MipData = new byte[0];
            Regs = new uint[5];

            UserData = new ResDict<UserData>();
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FTEX";

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the source channel to map to the R (red) channel.
        /// </summary>
        [Browsable(true)]
        [Description("The source channel to map to the R (red) channel.")]
        [Category("Channels")]
        [DisplayName("Red Channel")]
        public GX2CompSel CompSelR { get; set; }

        /// <summary>
        /// Gets or sets the source channel to map to the G (green) channel.
        /// </summary>
        [Browsable(true)]
        [Description("The source channel to map to the G (green) channel.")]
        [Category("Channels")]
        [DisplayName("Green Channel")]
        public GX2CompSel CompSelG { get; set; }

        /// <summary>
        /// Gets or sets the source channel to map to the B (blue) channel.
        /// </summary>
        [Browsable(true)]
        [Description("The source channel to map to the B (blue) channel.")]
        [Category("Channels")]
        [DisplayName("Blue Channel")]
        public GX2CompSel CompSelB { get; set; }

        /// <summary>
        /// Gets or sets the source channel to map to the A (alpha) channel.
        /// </summary>
        [Browsable(true)]
        [Description("The source channel to map to the A (alpha) channel.")]
        [Category("Channels")]
        [DisplayName("Alpha Channel")]
        public GX2CompSel CompSelA { get; set; }

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{Texture}"/>
        /// instances.
        /// </summary>
        [Browsable(true)]
        [Description("Name")]
        [Category("Image Info")]
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the file which originally supplied the data of this instance.
        /// </summary>
        [Browsable(true)]
        [Description("The path the file was originally located.")]
        [Category("Image Info")]
        [DisplayName("Path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the width of the texture.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Width of the image")]
        [Category("Image Info")]
        [DisplayName("Width")]
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the texture.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Height of the image")]
        [Category("Image Info")]
        [DisplayName("Height")]
        public uint Height { get; set; }

        /// <summary>
        /// Gets or sets the depth of the texture.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Depth")]
        [DisplayName("Depth")]
        public uint Depth { get; set; }

        /// <summary>
        /// Gets or sets the number of mipmaps stored in the <see cref="MipData"/>.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Number of mip maps")]
        [Category("Image Info")]
        [DisplayName("Mip Count")]
        public uint MipCount { get; set; }

        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Number of array images")]
        [Category("Image Info")]
        [DisplayName("Array Count")]
        public uint ArrayLength { get; set; }

        /// <summary>
        /// Gets or sets the swizzling value.
        /// </summary>
        [Browsable(true)]
        [Description("Swizzle")]
        [DisplayName("Swizzle")]
        public uint Swizzle { get; set; }

        /// <summary>
        /// Gets or sets the swizzling alignment.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Alignment")]
        [DisplayName("Alignment")]
        public uint Alignment { get; set; }

        /// <summary>
        /// Gets or sets the pixel swizzling stride.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("The pixel swizzling stride")]
        [DisplayName("Pitch")]
        public uint Pitch { get; set; }

        /// <summary>
        /// Gets or sets the desired texture data buffer format.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Format")]
        [Category("Image Info")]
        [DisplayName("Format")]
        public GX2SurfaceFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the shape of the texture.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Dims of the texture")]
        [DisplayName("Dims")]
        public GX2SurfaceDim Dim { get; set; }

        /// <summary>
        /// Gets or sets the number of samples for the texture.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Anti Alias Mode")]
        [DisplayName("Anti Alias Mode")]
        public GX2AAMode AAMode { get; set; }

        /// <summary>
        /// Gets or sets the texture data usage hint.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("The way the surface is used")]
        [DisplayName("Use")]
        public GX2SurfaceUse Use { get; set; }

        /// <summary>
        /// Gets or sets the tiling mode.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Description("Tiling mode")]
        [DisplayName("Tile Mode")]
        public GX2TileMode TileMode { get; set; }

        /// <summary>
        /// Gets or sets the offsets in the <see cref="MipData"/> array to the data of the mipmap level corresponding
        /// to the array index.
        /// </summary>
        [Browsable(false)]
        public uint[] MipOffsets { get; set; }

        [Browsable(false)]
        public uint ViewMipFirst { get; set; }

        [Browsable(false)]
        public uint ViewMipCount { get; set; }

        [Browsable(false)]
        public uint ViewSliceFirst { get; set; }

        [Browsable(false)]
        public uint ViewSliceCount { get; set; }

        [Browsable(false)]
        public uint[] Regs { get; set; }

        /// <summary>
        /// Gets or sets the raw texture data bytes.
        /// </summary>
        [Browsable(false)]
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the raw mipmap level data bytes for all levels.
        /// </summary>
        [Browsable(false)]
        public byte[] MipData { get; set; }

        /// <summary>
        /// Gets or sets customly attached <see cref="UserData"/> instances.
        /// </summary>
        [Browsable(false)]
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
            Dim = loader.ReadEnum<GX2SurfaceDim>(true);
            Width = loader.ReadUInt32();
            Height = loader.ReadUInt32();
            Depth = loader.ReadUInt32();
            MipCount = loader.ReadUInt32();
            Format = loader.ReadEnum<GX2SurfaceFormat>(true);
            AAMode = loader.ReadEnum<GX2AAMode>(true);
            Use = loader.ReadEnum<GX2SurfaceUse>(true);
            uint sizData = loader.ReadUInt32();
            uint imagePointer = loader.ReadUInt32();
            uint sizMipData = loader.ReadUInt32();
            uint mipPointer = loader.ReadUInt32();
            TileMode = loader.ReadEnum<GX2TileMode>(true);
            Swizzle = loader.ReadUInt32();
            Alignment = loader.ReadUInt32();
            Pitch = loader.ReadUInt32();
            MipOffsets = loader.ReadUInt32s(13);
            ViewMipFirst = loader.ReadUInt32();
            ViewMipCount = loader.ReadUInt32();
            ViewSliceFirst = loader.ReadUInt32();
            ViewSliceCount = loader.ReadUInt32();
            CompSelR = loader.ReadEnum<GX2CompSel>(true);
            CompSelG = loader.ReadEnum<GX2CompSel>(true);
            CompSelB = loader.ReadEnum<GX2CompSel>(true);
            CompSelA = loader.ReadEnum<GX2CompSel>(true);
            Regs = loader.ReadUInt32s(5);
            uint handle = loader.ReadUInt32();
            ArrayLength = loader.ReadByte(); // Possibly just a byte.
            loader.Seek(3, System.IO.SeekOrigin.Current);
            Name = loader.LoadString();
            Path = loader.LoadString();

            // Load texture data.
            bool? isMainTextureFile
                = loader.ResFile.Name.Contains(".Tex1") ? new bool?(true)
                : loader.ResFile.Name.Contains(".Tex2") ? new bool?(false)
                : null;

            switch (isMainTextureFile)
            {
                case true:
                    Data = loader.LoadCustom(() => loader.ReadBytes((int)sizData));
                    loader.ReadOffset(); // MipData not used.
                    break;
                case false:
                    MipData = loader.LoadCustom(() => loader.ReadBytes((int)sizMipData));
                    loader.ReadOffset(); // Data not used.
                    break;
                default:
                    Data = loader.LoadCustom(() => loader.ReadBytes((int)sizData));
                    MipData = loader.LoadCustom(() => loader.ReadBytes((int)sizMipData));
                    break;
            }

            UserData = loader.LoadDict<UserData>();
            ushort numUserData = loader.ReadUInt16();
            loader.Seek(2);
        }

        internal long PosUserDataOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write(Dim, true);
            saver.Write(Width);
            saver.Write(Height);
            saver.Write(Depth);
            saver.Write(MipCount);
            saver.Write(Format, true);
            saver.Write(AAMode, true);
            saver.Write(Use, true);

            bool? isMainTextureFile
         = saver.ResFile.Name.Contains(".Tex1") ? new bool?(true)
         : saver.ResFile.Name.Contains(".Tex2") ? new bool?(false)
         : null;

            switch (isMainTextureFile)
            {
                case false:
                    saver.Write(0);
                    saver.Write(0); // ImagePointer
                    saver.Write(MipData == null ? 0 : MipData.Length);
                    saver.Write(0); // MipPointer
                    break;
                default:
                    saver.Write(Data == null ? 0 : Data.Length);
                    saver.Write(0); // ImagePointer
                    saver.Write(MipData == null ? 0 : MipData.Length);
                    saver.Write(0); // MipPointer
                    break;
            }


            saver.Write(TileMode, true);
            saver.Write(Swizzle);
            saver.Write(Alignment);
            saver.Write(Pitch);
            saver.Write(MipOffsets);
            saver.Write(ViewMipFirst);
            saver.Write(ViewMipCount);
            saver.Write(ViewSliceFirst);
            saver.Write(ViewSliceCount);
            saver.Write(CompSelR, true);
            saver.Write(CompSelG, true);
            saver.Write(CompSelB, true);
            saver.Write(CompSelA, true);
            saver.Write(Regs);
            saver.Write(0); // Handle
            saver.Write((byte)ArrayLength);
            saver.Seek(3);
            saver.SaveString(Name);
            saver.SaveString(Path);

            switch (isMainTextureFile)
            {
                case true:
                    saver.SaveBlock(Data, saver.ResFile.Alignment, () => saver.Write(Data));
                    saver.Write(0); // MipData not used.
                    break;
                case false:
                    saver.SaveBlock(MipData, saver.ResFile.Alignment, () => saver.Write(MipData));
                    saver.Write(0);  // Data not used.
                    break;
                default:
                    saver.SaveBlock(Data, saver.ResFile.Alignment, () => saver.Write(Data));
                    saver.SaveBlock(MipData, saver.ResFile.Alignment, () => saver.Write(MipData));
                    break;
            }

            PosUserDataOffset = saver.SaveOffsetPos();
            saver.Write((ushort)UserData.Count);
            saver.Seek(2);
        }
    }
}