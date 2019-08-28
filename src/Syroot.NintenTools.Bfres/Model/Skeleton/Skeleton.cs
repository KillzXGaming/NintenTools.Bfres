using System.Collections.Generic;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;
using System.IO;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSKL section in a <see cref="Model"/> subfile, storing armature data.
    /// </summary>
    public class Skeleton : IResData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Skeleton"/> class.
        /// </summary>
        public Skeleton()
        {
            MatrixToBoneList = new List<ushort>();
            InverseModelMatrices = new List<Matrix3x4>();
            Bones = new ResDict<Bone>();
            FlagsRotation = SkeletonFlagsRotation.EulerXYZ;
            FlagsScaling = SkeletonFlagsScaling.Maya;
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSKL";

        private const uint _flagsScalingMask = 0b00000000_00000000_00000011_00000000;
        private const uint _flagsRotationMask = 0b00000000_00000000_01110000_00000000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private uint _flags;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public SkeletonFlagsScaling FlagsScaling
        {
            get { return (SkeletonFlagsScaling)(_flags & _flagsScalingMask); }
            set { _flags = _flags & ~_flagsScalingMask | (uint)value; }
        }

        /// <summary>
        /// Gets or sets the rotation method used to store bone rotations.
        /// </summary>
        public SkeletonFlagsRotation FlagsRotation
        {
            get { return (SkeletonFlagsRotation)(_flags & _flagsRotationMask); }
            set { _flags = _flags & ~_flagsRotationMask | (uint)value; }
        }

        /// <summary>
        /// Gets or sets the list of <see cref="Bone"/> instances forming the skeleton.
        /// </summary>
        public ResDict<Bone> Bones { get; set; }

        public IList<ushort> MatrixToBoneList { get; set; }

        public IList<Matrix3x4> InverseModelMatrices { get; set; }

        public IList<ushort> GetSmoothIndices()
        {
            List<ushort> indices = new List<ushort>();
            foreach (Bone bone in Bones.Values)
            {
                if (bone.SmoothMatrixIndex != -1)
                    indices.Add((ushort)bone.SmoothMatrixIndex);
            }
            return indices;
        }

        public IList<ushort> GetRigidIndices()
        {
            List<ushort> indices = new List<ushort>();
            foreach (Bone bone in Bones.Values)
            {
                if (bone.RigidMatrixIndex != -1)
                    indices.Add((ushort)bone.RigidMatrixIndex);
            }
            return indices;
        }

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
            _flags = loader.ReadUInt32();
            ushort numBone = loader.ReadUInt16();
            ushort numSmoothMatrix = loader.ReadUInt16();
            ushort numRigidMatrix = loader.ReadUInt16();
            loader.Seek(2);
            Bones = loader.LoadDict<Bone>();
            uint ofsBoneList = loader.ReadOffset(); // Only load dict.
            MatrixToBoneList = loader.LoadCustom(() => loader.ReadUInt16s((numSmoothMatrix + numRigidMatrix)));
            if (loader.ResFile.Version >= 0x03040000)
                InverseModelMatrices = loader.LoadCustom(() => loader.ReadMatrix3x4s(numSmoothMatrix));
            uint userPointer = loader.ReadUInt32();
        }

        internal long PosBoneDictOffset;
        internal long PosBoneArrayOffset;
        internal long PosMatrixToBoneListOffset;
        internal long PosInverseModelMatricesOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write(_flags);
            saver.Write((ushort)Bones.Count);
            if (saver.ResFile.Version >= 0x03040000)
            {
                saver.Write((ushort)InverseModelMatrices.Count); // NumSmoothMatrix
                saver.Write((ushort)(MatrixToBoneList.Count - InverseModelMatrices.Count)); // NumRigidMatrix
            }
            else
            {
                int numRididMatrix = 0;
                foreach (Bone bn in Bones.Values)
                {
                    if (bn.RigidMatrixIndex != -1)
                        numRididMatrix++;
                }

                saver.Write((ushort)(MatrixToBoneList.Count - numRididMatrix)); // NumRigidMatrix
                saver.Write((ushort)(numRididMatrix)); // NumRigidMatrix
            }
            saver.Seek(2);
            PosBoneDictOffset = saver.SaveOffsetPos();
            PosBoneArrayOffset = saver.SaveOffsetPos();
            PosMatrixToBoneListOffset = saver.SaveOffsetPos();
            if (saver.ResFile.Version >= 0x03040000)
                PosInverseModelMatricesOffset = saver.SaveOffsetPos();
            saver.Write(0); // UserPointer
        }
    }

    public enum SkeletonFlagsScaling : uint
    {
        None,
        Standard = 1 << 8,
        Maya = 2 << 8,
        Softimage = 3 << 8
    }

    /// <summary>
    /// Represents the rotation method used to store bone rotations.
    /// </summary>
    public enum SkeletonFlagsRotation : uint
    {
        /// <summary>
        /// A quaternion represents the rotation.
        /// </summary>
        Quaternion,

        /// <summary>
        /// A <see cref="Vector3F"/> represents the Euler rotation in XYZ order.
        /// </summary>
        EulerXYZ = 1 << 12
    }
}
