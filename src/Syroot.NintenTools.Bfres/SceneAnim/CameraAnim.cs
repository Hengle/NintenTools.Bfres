﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FCAM section in a <see cref="SceneAnim"/> subfile, storing animations controlling camera settings.
    /// </summary>
    [DebuggerDisplay(nameof(CameraAnim) + " {" + nameof(Name) + "}")]
    public class CameraAnim : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public CameraAnim(ResFileLoader loader)
            : base(loader)
        {
            CameraAnimHead head = new CameraAnimHead(loader);
            Flags = head.Flags;
            FrameCount = head.NumFrame;
            BakedSize = head.SizBaked;
            Name = loader.GetName(head.OfsName);
            Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);
            Result = loader.Load<CameraAnimResult>(head.OfsResult);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public CameraAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public uint BakedSize { get; }

        public string Name { get; set; }

        public IList<AnimCurve> Curves { get; }

        public CameraAnimResult Result { get; set; }

        public IList<UserData> UserData { get; }
    }

    /// <summary>
    /// Represents the header of a <see cref="CameraAnim"/> instance.
    /// </summary>
    internal class CameraAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FCAM";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal CameraAnimFlags Flags;
        internal int NumFrame;
        internal byte NumCurve;
        internal ushort NumUserData;
        internal uint SizBaked;
        internal uint OfsName;
        internal uint OfsCurveList;
        internal uint OfsResult;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public CameraAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            Flags = loader.ReadEnum<CameraAnimFlags>(true);
            loader.Seek(2);
            NumFrame = loader.ReadInt32();
            NumCurve = loader.ReadByte();
            loader.Seek(1);
            NumUserData = loader.ReadUInt16();
            SizBaked = loader.ReadUInt32();
            OfsName = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsResult = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum CameraAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2,
        EulerZXY = 1 << 8,
        Perspective = 1 << 10
    }

    public class CameraAnimResult : ResContent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        public CameraAnimResult(ResFileLoader loader)
            : base(loader)
        {
            ClipNear = loader.ReadSingle();
            ClipFar = loader.ReadSingle();
            AspectRatio = loader.ReadSingle();
            FieldOfView = loader.ReadSingle();
            Position = loader.ReadVector3F();
            Rotation = loader.ReadVector3F();
            Twist = loader.ReadSingle();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public float ClipNear { get; set; }

        public float ClipFar { get; set; }

        public float AspectRatio { get; set; }

        public float FieldOfView { get; set; }

        public Vector3F Position { get; set; }

        public Vector3F Rotation { get; set; }

        public float Twist { get; set; }
    }
}