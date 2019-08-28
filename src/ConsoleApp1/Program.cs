using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.Bfres.Helpers;
using System.Timers;
using System.Windows;
using Syroot.BinaryData;
using Syroot.Maths;
using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.Wii.Animations;
using SharpYaml;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;
using ByamlExt.Byaml;

namespace ConsoleApp1
{
    class Program
    {
        private static DateTime startTime;
        public static string time;

        public static ResFile targetBFRES;

        static void Main(string[] args)
        {
            startTime = DateTime.Now;
            targetBFRES = new ResFile("Player_Animation.bfres");
            targetBFRES.SkeletalAnims[0].Export("AFile.yaml", targetBFRES);

            TimeSpan timeElapsed = DateTime.Now - startTime;
            Console.WriteLine($" Loaded BFRES in { timeElapsed.TotalSeconds.ToString("0.000")}");

            startTime = DateTime.Now;
            targetBFRES.Save("NewTest.bfres");
            timeElapsed = DateTime.Now - startTime;
            Console.WriteLine($" Saved BFRES in { timeElapsed.TotalSeconds.ToString("0.000")}");

       //     ParseYML("course_muunt.byaml");

            Console.Read();
        }

        private static void ParseYML(string FileName)
        {
            BymlFileData data = ByamlFile.LoadN(FileName, true);

            var serializer = new SharpYaml.Serialization.Serializer();
            var text = serializer.Serialize(data);

            Console.WriteLine(text);
        }

        private static void Chr0Convert(string[] args)
        {
            string openbfres = "";

            if (args.Length > 0)
                openbfres = args[0];
            else
                openbfres = "dv_Kaigan_Original.bfres";

            Console.WriteLine($"Loading bfres {openbfres}");
            ResFile resFile = new ResFile(openbfres);

            resFile.Textures[0].Export("dummy.bftex", resFile);

            Chr02Fska(CHR0Node.FromFile("walk.chr0"));

            Console.WriteLine($"Saving bfres");
            resFile.Save($"{openbfres}NEW.bfres");
            Console.WriteLine($"File Saved!");
            Console.Read();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            TimeSpan timeElapsed = DateTime.Now - startTime;
            time = timeElapsed.TotalSeconds.ToString("0.000");
        }
        public static SkeletalAnim Chr02Fska(CHR0Node chr0)
        {
            SkeletalAnim fska = new SkeletalAnim();
            fska.FrameCount = chr0.FrameCount;
            fska.Name = chr0.Name;
            fska.Path = chr0.FilePath;
            fska.UserData = new ResDict<Syroot.NintenTools.Bfres.UserData>();

            //Set flags
            if (chr0.Loop)
                fska.FlagsAnimSettings |= SkeletalAnimFlags.Looping;
            fska.FlagsRotate = SkeletalAnimFlagsRotate.EulerXYZ;
            fska.FlagsScale = SkeletalAnimFlagsScale.Maya;

            //Set bone anims and then calculate data after
            foreach (var entry in chr0.Children)
                fska.BoneAnims.Add(Chr0Entry2BoneAnim((CHR0EntryNode)entry));

            fska.BakedSize = CalculateBakeSize(fska);
            fska.BindIndices = SetIndices(fska);

            return fska;
        }
        private static BoneAnim Chr0Entry2BoneAnim(CHR0EntryNode entry)
        {
            BoneAnim boneAnim = new BoneAnim();
            boneAnim.Name = entry.Name;

            if (entry.UseModelTranslate)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Translate;
            if (entry.UseModelRotate)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Rotate;
            if (entry.UseModelScale)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Scale;

            var baseData = new BoneAnimData();
            baseData.Translate = new Vector3F();
            baseData.Rotate = new Vector4F();
            baseData.Scale = new Vector3F();
            baseData.Flags = 0;
            boneAnim.BaseData = baseData;

            boneAnim.BeginBaseTranslate = 0;
            boneAnim.BeginRotate = 0;
            boneAnim.BeginTranslate = 0;
            boneAnim.FlagsTransform = 0;

            KeyframeCollection c = entry.Keyframes;
            for (int index = 0; index < 9; index++)
            {
   
            }

            if (entry.UseModelTranslate)
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateX;
            if (entry.UseModelRotate)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Rotate;
            if (entry.UseModelScale)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Scale;

            return boneAnim;
        }
        private static ushort[] SetIndices(SkeletalAnim fska)
        {
            List<ushort> indces = new List<ushort>();
            foreach (var boneAnim in fska.BoneAnims)
                indces.Add(65535);

            return indces.ToArray();
        }
        private static uint CalculateBakeSize(SkeletalAnim fska)
        {
            return 0;
        }

        /*   public class CHR0
           {
               public uint dataOffset { get; set; }
               public string Name { get; set; }
               public RRESDictChr0 Dictionary { get; set;}

               public CHR0(string FileName)
               {
                   CHR0Node chr0 = CHR0Node.FromFile(FileName);
                   foreach (var entry in chr0.Children)
                   {
                       CHR0EntryNode node = (CHR0EntryNode)entry;
                       for (int frame = 0; frame < node.FrameCount; frame++)
                       {
                           Console.WriteLine(node.GetAnimFrame(frame));

                       }
                       Console.WriteLine(node.Name);

                   }

                   var reader = new BinaryDataReader(new FileStream(
                           FileName,
                           FileMode.Open,
                           FileAccess.Read));
                   reader.ByteOrder = ByteOrder.BigEndian;

                  // Read(reader);
               }
               public void Read(BinaryDataReader reader)
               {
                   string signature = reader.ReadString(sizeof(uint), Encoding.ASCII);
                   if (signature != "CHR0")
                       throw new Exception();

                   uint FileLength = reader.ReadUInt32();
                   uint Version = reader.ReadUInt32();
                   uint BrresOffset = reader.ReadUInt32();
                   dataOffset = reader.ReadUInt32();
                   uint UserData = reader.ReadUInt32();
                   Name = LoadString(reader, 0);
                   uint OriginalPathOffset = reader.ReadUInt32();
                   uint FrameCount = reader.ReadUInt32();
                   uint EntryCount = reader.ReadUInt32();
                   uint IsLooping = reader.ReadUInt32();
                   uint ScalingRule = reader.ReadUInt32();

                   reader.Seek(dataOffset, SeekOrigin.Begin);

                   Dictionary = new RRESDictChr0();
                   Dictionary.Read(reader, this);
               }
           }
           public class RRESDictChr0
           {
               public string Name;
               public CHR0Entry chr0Entry;

               public void Read(BinaryDataReader reader, CHR0 header)
               {
                   uint tableSize = reader.ReadUInt32();
                   uint tableCount = reader.ReadUInt32();

                   for (int i = 0; i <= tableCount; i++)
                   {
                       uint key        = reader.ReadUInt32();
                       ushort left     = reader.ReadUInt16();
                       ushort right    = reader.ReadUInt16();
                       Name            = LoadString(reader, header.dataOffset);
                       uint DataOffset = reader.ReadUInt32();
                       long Pos        = reader.Position;

                       if (DataOffset != 0)
                       {
                           reader.Seek(DataOffset + header.dataOffset, SeekOrigin.Begin);
                           chr0Entry = new CHR0Entry();
                           chr0Entry.Read(reader);
                       }
                       reader.Seek(Pos, SeekOrigin.Begin);
                   }
               }
           }
           public class CHR0EntryTest
           {
               #region enums

               public AnimFlags animFlags = new AnimFlags();
               public class AnimFlags
               {
                   public int flags;

                   public TrackFormat TranslationType      { get { return (TrackFormat)((flags >> 0x1e) & 0x3); } }
                   public TrackFormat RotationType         { get { return (TrackFormat)((flags >> 0x1b) & 0x7); } }
                   public TrackFormat ScaleType            { get { return (TrackFormat)((flags >> 0x19) & 0x3); } }

                   public bool HasTranslation       { get { return ((flags >> 0x18) & 0x1) != 0; } }
                   public bool HasRotation          { get { return ((flags >> 0x17) & 0x1) != 0; } }
                   public bool HasScale             { get { return ((flags >> 0x16) & 0x1) != 0; } }
                   public bool IsTranslationXFixed  { get { return ((flags >> 0x13) & 0x1) != 0; } }
                   public bool IsTranslationYFixed  { get { return ((flags >> 0x14) & 0x1) != 0; } }
                   public bool IsTranslationZFixed  { get { return ((flags >> 0x15) & 0x1) != 0; } }
                   public bool IsRotationXFixed     { get { return ((flags >> 0x10) & 0x1) != 0; } }
                   public bool IsRotationYFixed     { get { return ((flags >> 0x11) & 0x1) != 0; } }
                   public bool IsRotationZFixed     { get { return ((flags >> 0x12) & 0x1) != 0; } }
                   public bool IsScaleXFixed        { get { return ((flags >> 0xd) & 0x1) != 0; } }
                   public bool IsScaleYFixed        { get { return ((flags >> 0xe) & 0x1) != 0; } }
                   public bool IsScaleZFixed        { get { return ((flags >> 0xf) & 0x1) != 0; } }
                   public bool IsTranslateIsotropic { get { return ((flags >> 0x6) & 0x1) != 0; } }
                   public bool IsScaleIsotropic     { get { return ((flags >> 0x5) & 0x1) != 0; } }
                   public bool IsRotationIsotropic  { get { return ((flags >> 0x4) & 0x1) != 0; } }
               }

               public enum TrackFormat : int
               {
                   None = 0,
                   I4 = 1,
                   I6 = 2,
                   I12 = 3,
                   L1 = 4,
                   L2 = 5,
                   L4 = 6
               };

               #endregion

               public void Read(BinaryDataReader reader)
               {

                   uint Position = (uint)reader.Position;
                   string Name = LoadString(reader, Position);
                   animFlags.flags = reader.ReadInt32();
               }

               private void Debug()
               {
                   Console.WriteLine("----------------------------------------------");
                   Console.WriteLine($"TranslationType {animFlags.TranslationType}");
                   Console.WriteLine($"RotationType {animFlags.RotationType}");
                   Console.WriteLine($"ScaleType {animFlags.ScaleType}");
                   Console.WriteLine($"IsTranslationXFixed {animFlags.IsTranslationXFixed}");
                   Console.WriteLine($"IsTranslationYFixed {animFlags.IsTranslationYFixed}");
                   Console.WriteLine($"IsTranslationZFixed {animFlags.IsTranslationZFixed}");
                   Console.WriteLine($"IsRotationXFixed {animFlags.IsRotationXFixed}");
                   Console.WriteLine($"IsRotationYFixed {animFlags.IsRotationYFixed}");
                   Console.WriteLine($"IsRotationZFixed {animFlags.IsRotationZFixed}");
                   Console.WriteLine($"IsScaleXFixed {animFlags.IsScaleXFixed}");
                   Console.WriteLine($"IsScaleYFixed {animFlags.IsScaleYFixed}");
                   Console.WriteLine($"IsScaleZFixed {animFlags.IsScaleZFixed}");
                   Console.WriteLine($"IsTranslateIsotropic {animFlags.IsTranslateIsotropic}");
                   Console.WriteLine($"IsRotationIsotropic {animFlags.IsRotationIsotropic}");
                   Console.WriteLine($"IsScaleIsotropic {animFlags.IsScaleIsotropic}");
                   Console.WriteLine($"HasTranslation {animFlags.HasTranslation}");
                   Console.WriteLine($"HasScale {animFlags.HasScale}");
                   Console.WriteLine($"HasRotation {animFlags.HasRotation}");
                   Console.WriteLine("----------------------------------------------");
               }
           }
           internal static string LoadString(BinaryDataReader reader, uint StartOffset, Encoding encoding = null)
           {
               uint offset = reader.ReadUInt32();
               if (offset == 0) return null;

               encoding = encoding ?? Encoding.Default;
               using (reader.TemporarySeek(offset + StartOffset, System.IO.SeekOrigin.Begin))
               {
                   return reader.ReadString(BinaryStringFormat.ZeroTerminated, encoding);
               }
           }*/
    }

}
