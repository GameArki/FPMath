using System;
using System.Collections.Generic;
using FixMath.NET;

namespace JackFrame.FPMath {

    public class FPOctreeNode<T> {

        // ==== Define ====
        static class LocationConfig {
            public const byte NONE = 0;
            public const byte BLF = 1 << 0;
            public const byte BRF = 1 << 1;
            public const byte TLF = 1 << 2;
            public const byte TRF = 1 << 3;
            public const byte BLB = 1 << 4;
            public const byte BRB = 1 << 5;
            public const byte TLB = 1 << 6;
            public const byte TRB = 1 << 7;
            public const byte FULL = 0xFF;
            public const int DEPTH_SHIFT = 8;
        }

        static Dictionary<byte, int> cornerToIndex = new Dictionary<byte, int>() {
            { LocationConfig.BLF, BLF_INDEX },
            { LocationConfig.BRF, BRF_INDEX },
            { LocationConfig.TLF, TLF_INDEX },
            { LocationConfig.TRF, TRF_INDEX },
            { LocationConfig.BLB, BLB_INDEX },
            { LocationConfig.BRB, BRB_INDEX },
            { LocationConfig.TLB, TLB_INDEX },
            { LocationConfig.TRB, TRB_INDEX },
        };

        const int BLF_INDEX = 0;
        const int BRF_INDEX = 1;
        const int TLF_INDEX = 2;
        const int TRF_INDEX = 3;
        const int BLB_INDEX = 4;
        const int BRB_INDEX = 5;
        const int TLB_INDEX = 6;
        const int TRB_INDEX = 7;

        // ==== External ====
        Ptr_FPOctree treePtr;
        FPOctree<T> Tree => treePtr as FPOctree<T>;

        // ==== Info ====
        ulong locationID;
        void SetLocationID(ulong value) => locationID = value;

        uint onlyID;
        void SetOnlyID(uint value) => onlyID = value;
        public uint128 GetFullID() => new uint128(locationID, onlyID);

        object valuePtr;
        public T Value => (T)valuePtr;

        FPBounds3 bounds;
        public FPBounds3 Bounds => bounds;

        int depth;
        void SetDepth(int value) => depth = value;

        bool isSplit;

        // 带有值的子节点数量
        Dictionary<uint, FPOctreeNode<T>> children;

        // 分割后的 8 个子节点
        FPOctreeNode<T>[] splitedArray; // len: 8

        internal FPOctreeNode(Ptr_FPOctree tree, in FPBounds3 bounds, int depth) {
            this.treePtr = tree;
            this.bounds = bounds;
            this.depth = depth;
            this.onlyID = Tree.GenOnlyID();
            this.children = new Dictionary<uint, FPOctreeNode<T>>();
            this.splitedArray = new FPOctreeNode<T>[8];
        }

        internal void SetAsRoot() {
            this.locationID = LocationConfig.FULL;
        }

        // ==== Generic ====
        ulong GenBranchLocationID(ulong parentLocationID, byte corner, int parentDepth) {
            ulong value = parentLocationID | ((ulong)corner << ((parentDepth + 1) * LocationConfig.DEPTH_SHIFT));
            return value;
        }

        ulong GenLeafLocationID(ulong parentLocationID, int parentDepth, byte cornerID) {
            return parentLocationID |= ((ulong)cornerID << ((parentDepth) * LocationConfig.DEPTH_SHIFT));
        }

        ulong GetLocationIDFromFullID(uint128 fullID) {
            return (ulong)(fullID & 0xFFFFFFFFFFFFFFFF);
        }

        byte GetCornerIDFromLoactionID(ulong locationID, int depth) {
            int shift = (depth) * LocationConfig.DEPTH_SHIFT;
            ulong loc = locationID & ((ulong)LocationConfig.FULL << shift);
            return (byte)(loc >> shift);
        }

        byte GetCornerIDFromFullID(uint128 fullID, int depth) {
            int shift = (depth) * LocationConfig.DEPTH_SHIFT;
            uint128 loc = fullID & ((uint128)LocationConfig.FULL << shift);
            return (byte)(loc >> shift);
        }

        void SetAsLeaf(T valuePtr) {
            this.valuePtr = valuePtr;
        }

        void SetAsBranch() {
            this.valuePtr = null;
        }

        bool IsLeaf() {
            return valuePtr != null;
        }

        bool IsIntersectOrContains(in FPBounds3 other) {
            return bounds.IsIntersect(other) || bounds.IsContains(other);
        }

        bool AndNotZero(byte a, byte b) {
            return (a & b) != 0;
        }

        // ==== Traval ====
        internal void Traval(Action<FPOctreeNode<T>> action) {

            action.Invoke(this);

            for (int i = 0; i < splitedArray.Length; i += 1) {
                var corner = splitedArray[i];
                if (corner != null) {
                    corner.Traval(action);
                }
            }

            foreach (var kv in children) {
                action.Invoke(kv.Value);
            }

        }

        void VisitCorner(byte corner, Action<FPOctreeNode<T>> action) {

            if (corner == LocationConfig.NONE) {
                return;
            }

            if (corner == LocationConfig.FULL) {
                for (int i = 0; i < splitedArray.Length; i += 1) {
                    var cornerNode = splitedArray[i];
                    if (cornerNode != null) {
                        action.Invoke(cornerNode);
                    }
                }
                return;
            }

            bool has = cornerToIndex.TryGetValue(corner, out int index);
            if (has) {
                var cornerNode = splitedArray[index];
                if (cornerNode != null) {
                    action.Invoke(cornerNode);
                }
            } else {

                if (AndNotZero(corner, LocationConfig.BLF)) {
                    action.Invoke(splitedArray[BLF_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.BRF)) {
                    action.Invoke(splitedArray[BRF_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.TLF)) {
                    action.Invoke(splitedArray[TLF_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.TRF)) {
                    action.Invoke(splitedArray[TRF_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.BLB)) {
                    action.Invoke(splitedArray[BLB_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.BRB)) {
                    action.Invoke(splitedArray[BRB_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.TLB)) {
                    action.Invoke(splitedArray[TLB_INDEX]);
                }

                if (AndNotZero(corner, LocationConfig.TRB)) {
                    action.Invoke(splitedArray[TRB_INDEX]);
                }

            }
        }

        // ==== Insert ====
        internal FPOctreeNode<T> Insert(T valuePtr, in FPBounds3 bounds) {

            int nextDepth = depth + 1;

            var node = new FPOctreeNode<T>(treePtr, bounds, nextDepth);
            node.onlyID = Tree.GenOnlyID();
            node.SetAsLeaf(valuePtr);

            InsertNode(node, locationID, LocationConfig.NONE, depth);
            return node;
        }

        void InsertNode(FPOctreeNode<T> node, ulong parentLocationID, byte cornerID, int parentDepth) {

            SetAsBranch();

            node.SetLocationID(GenLeafLocationID(parentLocationID, parentDepth, cornerID));
            node.SetDepth(depth + 1);

            // 层级已满时, 不再分割, 直接添加到 children
            if (depth >= Tree.MaxDepth || node.depth >= Tree.MaxDepth) {
                children.Add(node.onlyID, node);
                return;
            }

            if (!isSplit) {
                InsertNodeWhenNotSplit(node);
            } else {
                InsertNodeWhenSplit(node);
            }

        }

        void InsertNodeWhenSplit(FPOctreeNode<T> node) {

            var nodeBounds = node.bounds;

            var corner = splitedArray[BLF_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.BLF, corner.depth);
            }

            corner = splitedArray[BRF_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.BRF, corner.depth);
            }

            corner = splitedArray[TLF_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.TLF, corner.depth);
            }

            corner = splitedArray[TRF_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.TRF, corner.depth);
            }

            corner = splitedArray[BLB_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.BLB, corner.depth);
            }

            corner = splitedArray[BRB_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.BRB, corner.depth);
            }

            corner = splitedArray[TLB_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.TLB, corner.depth);
            }

            corner = splitedArray[TRB_INDEX];
            if (corner.IsIntersectOrContains(nodeBounds)) {
                corner.InsertNode(node, node.locationID, LocationConfig.TRB, corner.depth);
            }

        }

        void InsertNodeWhenNotSplit(FPOctreeNode<T> node) {

            // Children 小于 4 个时, 插入
            if (children.Count < 4) {
                children.Add(node.onlyID, node);
                return;
            }

            // Children 等于 4 个时, 首次分割
            if (children.Count == 4) {
                Split();
                InsertNodeWhenSplit(node);
            }

        }

        void Split() {

            int nextDepth = depth + 1;
            var size = bounds.Size * FP64.Half;
            var halfSize = size * FP64.Half;
            var center = bounds.Center;

            var blfBounds = new FPBounds3(center - halfSize, size);
            var brfBounds = new FPBounds3(center + new FPVector3(halfSize.x, -halfSize.y, -halfSize.z), size);
            var tlfBounds = new FPBounds3(center + new FPVector3(-halfSize.x, halfSize.y, -halfSize.z), size);
            var trfBounds = new FPBounds3(center + new FPVector3(halfSize.x, halfSize.y, -halfSize.z), size);
            var blbBounds = new FPBounds3(center + new FPVector3(-halfSize.x, -halfSize.y, halfSize.z), size);
            var brbBounds = new FPBounds3(center + new FPVector3(halfSize.x, -halfSize.y, halfSize.z), size);
            var tlbBounds = new FPBounds3(center + new FPVector3(-halfSize.x, halfSize.y, halfSize.z), size);
            var trbBounds = new FPBounds3(center + halfSize, size);

            var blf = new FPOctreeNode<T>(treePtr, blfBounds, nextDepth);
            blf.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BLF, depth));
            splitedArray[BLF_INDEX] = blf;

            var brf = new FPOctreeNode<T>(treePtr, brfBounds, nextDepth);
            brf.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BRF, depth));
            splitedArray[BRF_INDEX] = brf;

            var tlf = new FPOctreeNode<T>(treePtr, tlfBounds, nextDepth);
            tlf.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TLF, depth));
            splitedArray[TLF_INDEX] = tlf;

            var trf = new FPOctreeNode<T>(treePtr, trfBounds, nextDepth);
            trf.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TRF, depth));
            splitedArray[TRF_INDEX] = trf;

            var blb = new FPOctreeNode<T>(treePtr, blbBounds, nextDepth);
            blb.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BLB, depth));
            splitedArray[BLB_INDEX] = blb;

            var brb = new FPOctreeNode<T>(treePtr, brbBounds, nextDepth);
            brb.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BRB, depth));
            splitedArray[BRB_INDEX] = brb;

            var tlb = new FPOctreeNode<T>(treePtr, tlbBounds, nextDepth);
            tlb.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TLB, depth));
            splitedArray[TLB_INDEX] = tlb;

            var trb = new FPOctreeNode<T>(treePtr, trbBounds, nextDepth);
            trb.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TRB, depth));
            splitedArray[TRB_INDEX] = trb;

            foreach (var kv in children) {
                var child = kv.Value;
                InsertNodeWhenSplit(child);
            }

            children.Clear();

            isSplit = true;

        }

        // ==== Remove ====
        internal void RemoveNode(uint128 targetFullID) {

            byte targetCornerID = GetCornerIDFromFullID(targetFullID, depth + 1);
            if (targetCornerID > LocationConfig.FULL) {
                return;
            }

            uint targetOnlyID = (uint)(targetFullID >> 64);

            _ = children.Remove(targetOnlyID);

            VisitCorner(targetCornerID, corner => {
                if (corner != null) {
                    corner.RemoveNode(targetFullID);
                }
            });

        }

        // ==== Query ====
        internal void GetCandidateNodes(in FPBounds3 bounds, HashSet<FPOctreeNode<T>> candidates) {

            if (IsLeaf()) {
                candidates.Add(this);
                return;
            } else {
                if (!IsIntersectOrContains(bounds)) {
                    return;
                }
            }

            if (isSplit) {
                for (int i = 0; i < splitedArray.Length; i += 1) {
                    var corner = splitedArray[i];
                    if (corner != null) {
                        corner.GetCandidateNodes(bounds, candidates);
                    }
                }
            }

            if (children.Count > 0) {
                foreach (var kv in children) {
                    var child = kv.Value;
                    child.GetCandidateNodes(bounds, candidates);
                }
            }

        }

        internal void GetCandidateValues(in FPBounds3 bounds, HashSet<T> candidates) {

            if (IsLeaf()) {
                candidates.Add(Value);
                return;
            } else {
                if (!IsIntersectOrContains(bounds)) {
                    return;
                }
            }

            if (isSplit) {
                for (int i = 0; i < splitedArray.Length; i += 1) {
                    var corner = splitedArray[i];
                    if (corner != null) {
                        corner.GetCandidateValues(bounds, candidates);
                    }
                }
            }

            if (children.Count > 0) {
                foreach (var kv in children) {
                    var child = kv.Value;
                    child.GetCandidateValues(bounds, candidates);
                }
            }

        }

        // TODO: GetCandidates By FullID

    }

}