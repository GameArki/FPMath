using System;
using System.Collections.Generic;
using FixMath.NET;

namespace JackFrame.FPMath {

    // 插入: 按 Bounds 插入
    // 移除: 按 FullID 移除
    // 查询: 按 Bounds 查询
    // 遍历: 全遍历
    public class FPQuadTreeNode<T> {

        // ==== Define ====
        static class LocationConfig {
            // 3 4
            // 1 2
            public const byte NONE = 0b0000;
            public const byte BL = 0b0001;
            public const byte BR = 0b0010;
            public const byte TL = 0b0100;
            public const byte TR = 0b1000;
            public const byte FULL = 0b1111;

            public const int DEPTH_SHIFT = 4;
        }

        static Dictionary<byte, int> cornerToIndex = new Dictionary<byte, int>() {
            { LocationConfig.BL, BL_INDEX },
            { LocationConfig.BR, BR_INDEX },
            { LocationConfig.TL, TL_INDEX },
            { LocationConfig.TR, TR_INDEX },
        };

        const int BL_INDEX = 0;
        const int BR_INDEX = 1;
        const int TL_INDEX = 2;
        const int TR_INDEX = 3;

        // ==== External ====
        Ptr_FPQuadTree treePtr;
        FPQuadTree<T> Tree => treePtr as FPQuadTree<T>;

        // ==== Info ====
        uint locationID;
        public void SetLocationID(uint value) => locationID = value;

        uint onlyID;
        public ulong GetFullID() => ((ulong)onlyID << 32) | (ulong)locationID;

        object valuePtr;
        public T Value => (T)valuePtr;

        FPBounds2 bounds;
        public FPBounds2 Bounds => bounds;

        int depth;
        public int Depth => depth;
        public void SetDepth(int value) => depth = value;

        bool isSplit;

        // 存储叶
        Dictionary<uint, FPQuadTreeNode<T>> children;

        // 分割 四块分支
        FPQuadTreeNode<T>[] splitedArray; // len: 4

        internal FPQuadTreeNode(Ptr_FPQuadTree tree, in FPBounds2 bounds, int depth) {
            this.treePtr = tree;
            this.isSplit = false;
            this.bounds = bounds;
            this.depth = depth;
            this.children = new Dictionary<uint, FPQuadTreeNode<T>>();
            this.splitedArray = new FPQuadTreeNode<T>[4];
        }

        internal void SetAsRoot() {
            SetLocationID(GenBranchLocationID(0, LocationConfig.FULL, -1));
            onlyID = Tree.GenOnlyID();
        }

        // ==== Generic ====
        uint GenBranchLocationID(uint parentLocationID, byte corner, int parentDepth) {
            uint value = parentLocationID | ((uint)corner << ((parentDepth + 1) * LocationConfig.DEPTH_SHIFT));
            return value;
        }

        uint GenLeafLocationID(uint parentLocationID, int parentDepth) {
            byte parentCorner = GetCornerIDFromLoactionID(parentLocationID, parentDepth);
            uint value = parentLocationID | ((uint)parentCorner << ((parentDepth) * LocationConfig.DEPTH_SHIFT));
            value = parentLocationID | ((uint)LocationConfig.FULL << ((parentDepth + 1) * LocationConfig.DEPTH_SHIFT));
            return value;
        }

        byte GetCornerIDFromLoactionID(uint locationID, int depth) {
            int shift = (depth) * LocationConfig.DEPTH_SHIFT;
            uint loc = locationID & ((uint)LocationConfig.FULL << shift);
            return (byte)(loc >> shift);
        }

        byte GetCornerIDFromFullID(ulong fullID, int depth) {
            int shift = (depth) * LocationConfig.DEPTH_SHIFT;
            ulong loc = fullID & ((ulong)LocationConfig.FULL << shift);
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

        bool IsIntersectOrContains(in FPBounds2 other) {
            return bounds.IsIntersect(other) || bounds.IsContains(other);
        }

        // ==== Traval ====
        internal void Traval(Action<FPQuadTreeNode<T>> action) {

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

        // ==== Insert ====
        internal void Insert(T valuePtr, in FPBounds2 bounds) {

            int nextDepth = depth + 1;

            var node = new FPQuadTreeNode<T>(treePtr, bounds, nextDepth);
            node.onlyID = Tree.GenOnlyID();
            node.SetAsLeaf(valuePtr);

            InsertNode(node);

        }

        void InsertNode(FPQuadTreeNode<T> node) {

            SetAsBranch();

            node.SetLocationID(GenLeafLocationID(locationID, depth));
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

        void InsertNodeWhenSplit(FPQuadTreeNode<T> node) {
            ref var nodeBounds = ref node.bounds;

            for (int i = 0; i < splitedArray.Length; i += 1) {
                var corner = splitedArray[i];
                if (corner != null && corner.IsIntersectOrContains(nodeBounds)) {
                    corner.InsertNode(node);
                }
            }

        }

        void InsertNodeWhenNotSplit(FPQuadTreeNode<T> node) {

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

            var blBounds = new FPBounds2(center - halfSize, size);
            var brBounds = new FPBounds2(new FPVector2(center.x + halfSize.x, center.y - halfSize.y), size);
            var tlBounds = new FPBounds2(new FPVector2(center.x - halfSize.x, center.y + halfSize.y), size);
            var trBounds = new FPBounds2(center + halfSize, size);

            var bl = new FPQuadTreeNode<T>(treePtr, blBounds, nextDepth);
            bl.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BL, depth));
            bl.onlyID = Tree.GenOnlyID();
            splitedArray[BL_INDEX] = bl;

            var br = new FPQuadTreeNode<T>(treePtr, brBounds, nextDepth);
            br.SetLocationID(GenBranchLocationID(locationID, LocationConfig.BR, depth));
            br.onlyID = Tree.GenOnlyID();
            splitedArray[BR_INDEX] = br;

            var tl = new FPQuadTreeNode<T>(treePtr, tlBounds, nextDepth);
            tl.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TL, depth));
            tl.onlyID = Tree.GenOnlyID();
            splitedArray[TL_INDEX] = tl;

            var tr = new FPQuadTreeNode<T>(treePtr, trBounds, nextDepth);
            tr.SetLocationID(GenBranchLocationID(locationID, LocationConfig.TR, depth));
            tr.onlyID = Tree.GenOnlyID();
            splitedArray[TR_INDEX] = tr;

            foreach (var kv in children) {

                var child = kv.Value;
                var childBounds = child.bounds;

                var corner = splitedArray[BL_INDEX];
                if (corner.IsIntersectOrContains(childBounds)) {
                    corner.InsertNode(child);
                }

                corner = splitedArray[BR_INDEX];
                if (corner.IsIntersectOrContains(childBounds)) {
                    corner.InsertNode(child);
                }

                corner = splitedArray[TL_INDEX];
                if (corner.IsIntersectOrContains(childBounds)) {
                    corner.InsertNode(child);
                }

                corner = splitedArray[TR_INDEX];
                if (corner.IsIntersectOrContains(childBounds)) {
                    corner.InsertNode(child);
                }

            }

            children.Clear();

            isSplit = true;

        }

        // ==== Remove ====
        internal void RemoveNode(ulong targetFullID, int depth) {

            byte targetCornerID = GetCornerIDFromFullID(targetFullID, depth);
            if (targetCornerID == LocationConfig.NONE || targetCornerID > LocationConfig.FULL) {
                return;
            }

            uint targetOnlyID = (uint)(targetFullID >> 32);

            if (isSplit) {
                // TODO: Only Remove One Of Corner
                for (int i = 0; i < splitedArray.Length; i += 1) {
                    var corner = splitedArray[i];
                    if (corner == null) {
                        continue;
                    }
                    corner.RemoveNode(targetFullID, depth + 1);
                }
            }

            bool hasRemove = children.Remove(targetOnlyID);
            // if (hasRemove) {
            //     System.Console.WriteLine(onlyID + "Remove Full: " + targetOnlyID + ", Depth: " + depth + ", Corner:" + targetCornerID + ", index: " + splitIndex);
            // }

        }

        // ==== Query ====
        internal void GetCandidates(in FPBounds2 bounds, List<FPQuadTreeNode<T>> candidates) {

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
                        corner.GetCandidates(bounds, candidates);
                    }
                }
            } else {
                foreach (var kv in children) {
                    var child = kv.Value;
                    child.GetCandidates(bounds, candidates);
                }
            }

        }

    }

}