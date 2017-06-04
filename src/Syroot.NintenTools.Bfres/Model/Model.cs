﻿using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FMDL subfile in a <see cref="ResFile"/>, storing model vertex data, skeletons and used materials.
    /// </summary>
    [DebuggerDisplay(nameof(Model) + " {" + nameof(Name) + "}")]
    public class Model : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public Model(ResFileLoader loader)
            : base(loader)
        {
            ModelHead head = new ModelHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            Skeleton = loader.Load<Skeleton>(head.OfsSkeleton);
            VertexBuffers = loader.LoadList<VertexBuffer>(head.OfsVertexBufferList, head.NumVertexBuffer);
            Shapes = loader.LoadDictList<Shape>(head.OfsShapeDict);
            Materials = loader.LoadDictList<Material>(head.OfsMaterialDict);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public string Path { get; set; }

        public Skeleton Skeleton { get; set; }

        public IList<VertexBuffer> VertexBuffers { get; }

        public IList<Shape> Shapes { get; }

        public IList<Material> Materials { get; }

        public IList<UserData> UserData { get; }
    }

    /// <summary>
    /// Represents the header of a <see cref="Model"/> instance.
    /// </summary>
    internal class ModelHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FMDL";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal uint OfsSkeleton;
        internal uint OfsVertexBufferList;
        internal uint OfsShapeDict;
        internal uint OfsMaterialDict;
        internal uint OfsUserDataDict;
        internal ushort NumVertexBuffer;
        internal ushort NumShape;
        internal ushort NumMaterial;
        internal ushort NumUserData;
        internal uint TotalVertices;
        internal uint UserPointer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ModelHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            OfsSkeleton = loader.ReadOffset();
            OfsVertexBufferList = loader.ReadOffset();
            OfsShapeDict = loader.ReadOffset();
            OfsMaterialDict = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
            NumVertexBuffer = loader.ReadUInt16();
            NumShape = loader.ReadUInt16();
            NumMaterial = loader.ReadUInt16();
            NumUserData = loader.ReadUInt16();
            TotalVertices = loader.ReadUInt32();
            UserPointer = loader.ReadUInt32();
        }
    }
}