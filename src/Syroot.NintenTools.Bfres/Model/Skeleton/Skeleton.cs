﻿using System;
using System.Collections.Generic;
using System.Text;
using Syroot.Maths;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSKL section in a <see cref="Model"/> subfile, storing armature data.
    /// </summary>
    public class Skeleton : IResData
    {
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
            set { _flags &= ~_flagsScalingMask | (uint)value; }
        }

        public SkeletonFlagsRotation FlagsRotation
        {
            get { return (SkeletonFlagsRotation)(_flags & _flagsRotationMask); }
            set { _flags &= ~_flagsRotationMask | (uint)value; }
        }
        
        public INamedResDataList<Bone> Bones { get; private set; }

        public IList<ushort> MatrixToBoneTable { get; private set; }

        public IList<Matrix3x4> InverseModelMatrices { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            loader.CheckSignature(_signature);
            _flags = loader.ReadUInt32();
            ushort numBone = loader.ReadUInt16();
            ushort numSmoothMatrix = loader.ReadUInt16();
            ushort numRigidMatrix = loader.ReadUInt16();
            loader.Seek(2);
            Bones = loader.LoadDictList<Bone>();
            uint ofsBoneList = loader.ReadOffset(); // Only load dict.
            MatrixToBoneTable = loader.LoadCustom(() => loader.ReadUInt16s((numSmoothMatrix + numRigidMatrix)));
            InverseModelMatrices = loader.LoadCustom(() => loader.ReadMatrix3x4s(numSmoothMatrix));
            uint userPointer = loader.ReadUInt32();
        }
        
        void IResData.Save(ResFileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.Write(_flags);
            saver.Write((ushort)Bones.Count);
            saver.Write((ushort)InverseModelMatrices.Count); // NumSmoothMatrix
            saver.Write((ushort)(MatrixToBoneTable.Count - InverseModelMatrices.Count)); // NumRigidMatrix
            saver.Seek(2);
            saver.SaveDictList(Bones);
            saver.SaveList(Bones);
            saver.SaveData(MatrixToBoneTable, () => saver.Write(MatrixToBoneTable));
            saver.SaveData(InverseModelMatrices, () => saver.Write(InverseModelMatrices));
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

    public enum SkeletonFlagsRotation : uint
    {
        Quaternion,
        EulerXYZ = 1 << 12
    }
}
