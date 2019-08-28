using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSHP section in a <see cref="Model"/> subfile.
    /// </summary>
    [DebuggerDisplay(nameof(Shape) + " {" + nameof(Name) + "}")]
    public class Shape : IResData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        public Shape()
        {
            Name = "";
            Flags = ShapeFlags.HasVertexBuffer;
            MaterialIndex = 0;
            BoneIndex = 0;
            VertexBufferIndex = 0;
            RadiusArray = new List<float>();
            VertexSkinCount = 0;
            TargetAttribCount = 0;
            Meshes = new List<Mesh>();
            SkinBoneIndices = new List<ushort>();
            KeyShapes = new ResDict<KeyShape>();
            SubMeshBoundings = new List<Bounding>();
            SubMeshBoundingNodes = new List<BoundingNode>();
            SubMeshBoundingIndices = new List<ushort>();
            VertexBuffer = new VertexBuffer();
        }

        public void CreateEmptyMesh()
        {
            var mesh = new Mesh();
            mesh.SetIndices(new uint[100], GX2.GX2IndexFormat.UInt16);
            mesh.SubMeshes.Add(new SubMesh() { Count = 100 });
            Meshes = new List<Mesh>();
            Meshes.Add(mesh);

            RadiusArray.Add(1.0f);

            //Set boundings for mesh
            SubMeshBoundings = new List<Bounding>();
            SubMeshBoundings.Add(new Bounding()
            {
                Center = new Maths.Vector3F(0, 0, 0),
                Extent = new Maths.Vector3F(50, 50, 50)
            });
            SubMeshBoundings.Add(new Bounding() //One more bounding for sub mesh
            {
                Center = new Maths.Vector3F(0, 0, 0),
                Extent = new Maths.Vector3F(50, 50, 50)
            });
            SubMeshBoundingIndices = new List<ushort>();
            SubMeshBoundingIndices.Add(0);
            SubMeshBoundingNodes = new List<BoundingNode>();
            SubMeshBoundingNodes.Add(new BoundingNode()
            {
                LeftChildIndex = 0,
                NextSibling = 0,
                SubMeshIndex = 0,
                RightChildIndex = 0,
                Unknown = 0,
                SubMeshCount = 1,
            });
        }

        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHP";
        
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the name with which the instance can be referenced uniquely in <see cref="ResDict{Shape}"/>
        /// instances.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets flags determining which data is available for this instance.
        /// </summary>
        public ShapeFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the index of the material to apply to the shapes surface in the owning
        /// <see cref="Model.Materials"/> list.
        /// </summary>
        public ushort MaterialIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the <see cref="Bone"/> to which this instance is directly attached to. The bone
        /// must be part of the skeleton referenced by the owning <see cref="Model.Skeleton"/> instance.
        /// </summary>
        public ushort BoneIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the <see cref="VertexBuffer"/> in the owning <see cref="Model.VertexBuffers"/>
        /// list.
        /// </summary>
        public ushort VertexBufferIndex { get; set; }

        /// <summary>
        /// Gets or sets the bounding radius/radii spanning the shape. BOTW uses multiple per LOD mesh.
        /// </summary>
        public IList<float> RadiusArray { get; set; }

        /// <summary>
        /// Gets or sets the number of bones influencing the vertices stored in this buffer. 0 influences equal
        /// rigidbodies (no skinning), 1 equal rigid skinning and 2 or more smooth skinning.
        /// </summary>
        public byte VertexSkinCount { get; set; }

        /// <summary>
        /// Gets or sets a value with unknown purpose.
        /// </summary>
        public byte TargetAttribCount { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="Meshes"/> which are used to represent different level of details of the
        /// shape.
        /// </summary>
        public IList<Mesh> Meshes { get; set; }
        
        public IList<ushort> SkinBoneIndices { get; set; }
        
        public ResDict<KeyShape> KeyShapes { get; set; }
        
        public IList<Bounding> SubMeshBoundings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BoundingNode"/> instances forming the bounding tree with which parts of a mesh
        /// are culled when not visible.
        /// </summary>
        public IList<BoundingNode> SubMeshBoundingNodes { get; set; }

        public IList<ushort> SubMeshBoundingIndices { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VertexBuffer"/> instance storing the data which forms the shape's surface. Saved
        /// depending on <see cref="VertexBufferIndex"/>.
        /// </summary>
        internal VertexBuffer VertexBuffer { get; set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        public void Import(string FileName, VertexBuffer vertexBuffer, ResFile ResFile)
        {
            using (ResFileLoader loader = new ResFileLoader(this, ResFile, FileName))
            {
                loader.ImportSection(vertexBuffer);
            }
        }

        public void Export(string FileName, VertexBuffer vertexBuffer, ResFile ResFile)
        {
            using (ResFileSaver saver = new ResFileSaver(this, ResFile, FileName))
            {
                saver.ExportSection(vertexBuffer);
            }
        }

        public Shape ShallowCopy()
        {
            return (Shape)this.MemberwiseClone();
        }

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            Name = loader.LoadString();
            Flags = loader.ReadEnum<ShapeFlags>(true);
            ushort idx = loader.ReadUInt16();
            MaterialIndex = loader.ReadUInt16();
            BoneIndex = loader.ReadUInt16();
            VertexBufferIndex = loader.ReadUInt16();
            ushort numSkinBoneIndex = loader.ReadUInt16();
            VertexSkinCount = loader.ReadByte();
            byte numMesh = loader.ReadByte();
            byte numKeyShape = loader.ReadByte();
            TargetAttribCount = loader.ReadByte();
            ushort numSubMeshBoundingNodes = loader.ReadUInt16(); // Padding in engine.

            if (loader.ResFile.Version >= 0x04050000)
            {
                RadiusArray = loader.LoadCustom(() => loader.ReadSingles(numMesh));
            }
            else
            {
                RadiusArray = loader.ReadSingles(1);
            }
            VertexBuffer = loader.Load<VertexBuffer>();
            Meshes = loader.LoadList<Mesh>(numMesh);
            SkinBoneIndices = loader.LoadCustom(() => loader.ReadUInt16s(numSkinBoneIndex));
            KeyShapes = loader.LoadDict<KeyShape>();



            // TODO: At least BotW has more data following the Boundings, or that are no boundings at all.
            if (numSubMeshBoundingNodes == 0 && loader.ResFile.Version >= 0x04050000)
            {
                // Compute the count differently if the node count was padding.
                if (numMesh == 1)      
                    SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count + numMesh));
                if (numMesh == 2)
                    SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count + Meshes[1].SubMeshes.Count + numMesh));
                if (numMesh == 3)
                    SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count + Meshes[1].SubMeshes.Count + Meshes[2].SubMeshes.Count + numMesh));
                if (numMesh == 4)
                    SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count + Meshes[1].SubMeshes.Count + Meshes[2].SubMeshes.Count + Meshes[3].SubMeshes.Count + numMesh));

                SubMeshBoundingNodes = new List<BoundingNode>();
            }
            else if (numSubMeshBoundingNodes == 0)
            {
                // Compute the count differently if the node count was padding.
                SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(Meshes[0].SubMeshes.Count + 1));
                SubMeshBoundingNodes = new List<BoundingNode>();
            }
            else
            {
                SubMeshBoundingNodes = loader.LoadList<BoundingNode>(numSubMeshBoundingNodes);
                SubMeshBoundings = loader.LoadCustom(() => loader.ReadBoundings(numSubMeshBoundingNodes));
                SubMeshBoundingIndices = loader.LoadCustom(() => loader.ReadUInt16s(numSubMeshBoundingNodes));
            }
            uint userPointer = loader.ReadUInt32();
        }

        internal long PosMeshArrayOffset;
        internal long PosSkinBoneIndicesOffset;
        internal long PosKeyShapesOffset;
        internal long PosSubMeshBoundingNodesOffset;
        internal long PosSubMeshBoundingsOffset;
        internal long PosSubMeshBoundingsIndicesOffset;
        internal long PosRadiusArrayOffset;

        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveString(Name);
            saver.Write(Flags, true);
            saver.Write((ushort)saver.CurrentIndex);
            saver.Write(MaterialIndex);
            saver.Write(BoneIndex);
            saver.Write(VertexBufferIndex);
            saver.Write((ushort)SkinBoneIndices.Count);
            saver.Write(VertexSkinCount);
            saver.Write((byte)Meshes.Count);
            saver.Write((byte)KeyShapes.Count);
            saver.Write(TargetAttribCount);
            saver.Write((ushort)SubMeshBoundingNodes?.Count);
            if (saver.ResFile.Version >= 0x04050000)
            {
                PosRadiusArrayOffset = saver.SaveOffsetPos();
            }
            else
            {
                saver.Write(RadiusArray);
            }
            saver.Write((uint)VertexBuffer.Position);
            PosMeshArrayOffset = saver.SaveOffsetPos();
            PosSkinBoneIndicesOffset = saver.SaveOffsetPos();
            PosKeyShapesOffset = saver.SaveOffsetPos();
            if (SubMeshBoundingNodes.Count == 0)
            {
                PosSubMeshBoundingNodesOffset = saver.SaveOffsetPos();
            }
            else
            {
                PosSubMeshBoundingNodesOffset = saver.SaveOffsetPos();
                PosSubMeshBoundingsOffset = saver.SaveOffsetPos();
                PosSubMeshBoundingsIndicesOffset = saver.SaveOffsetPos();
            }
            saver.Write(0); // UserPointer
        }
    }

    /// <summary>
    /// Represents flags determining which data is available for <see cref="Shape"/> instances.
    /// </summary>
    [Flags]
    public enum ShapeFlags : uint
    {
        /// <summary>
        /// The <see cref="Shape"/> instance references a <see cref="VertexBuffer"/>.
        /// </summary>
        HasVertexBuffer = 1 << 1,

        /// <summary>
        /// The boundings in all submeshes are consistent.
        /// </summary>
        SubMeshBoundaryConsistent = 1 << 2
    }
}