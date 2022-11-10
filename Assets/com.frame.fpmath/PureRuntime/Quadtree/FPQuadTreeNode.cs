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
        List<FPQuadTreeNode<T>> children;

        // 分割 四块分支
        FPQuadTreeNode<T>[] splitedArray; // len: 4

        internal FPQuadTreeNode(Ptr_FPQuadTree tree, in FPBounds2 bounds, int depth) {
            this.treePtr = tree;
            this.isSplit = false;
            this.bounds = bounds;
            this.depth = depth;
            this.children = new List<FPQuadTreeNode<T>>();
            this.splitedArray = new FPQuadTreeNode<T>[4];

            if (depth == 0) {
                SetLocationID(GenLocationID(0, LocationConfig.FULL, -1));
                onlyID = Tree.GenOnlyID();
            }
        }

        // ==== Generic ====
        uint GenLocationID(uint parentLocationID, byte location, int parentDepth) {
            uint value = parentLocationID | ((uint)location << ((parentDepth + 1) * LocationConfig.DEPTH_SHIFT));
            return value;
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

        bool AndNotZero(byte a, byte b) {
            return (a & b) != 0;
        }

        int ChildrenCount() {
            return children.Count;
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

            children.ForEach(action);

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

            node.SetLocationID(GenLocationID(locationID, LocationConfig.FULL, depth));
            node.SetDepth(depth + 1);

            // 层级已满时, 不再分割, 直接添加到 children
            if (depth >= Tree.MaxDepth || node.depth >= Tree.MaxDepth) {
                children.Add(node);
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
                children.Add(node);
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
            bl.SetLocationID(GenLocationID(locationID, LocationConfig.BL, depth));
            bl.onlyID = Tree.GenOnlyID();
            splitedArray[BL_INDEX] = bl;

            var br = new FPQuadTreeNode<T>(treePtr, brBounds, nextDepth);
            br.SetLocationID(GenLocationID(locationID, LocationConfig.BR, depth));
            br.onlyID = Tree.GenOnlyID();
            splitedArray[BR_INDEX] = br;

            var tl = new FPQuadTreeNode<T>(treePtr, tlBounds, nextDepth);
            tl.SetLocationID(GenLocationID(locationID, LocationConfig.TL, depth));
            tl.onlyID = Tree.GenOnlyID();
            splitedArray[TL_INDEX] = tl;

            var tr = new FPQuadTreeNode<T>(treePtr, trBounds, nextDepth);
            tr.SetLocationID(GenLocationID(locationID, LocationConfig.TR, depth));
            tr.onlyID = Tree.GenOnlyID();
            splitedArray[TR_INDEX] = tr;

            children.ForEach(child => {
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

            });

            children.Clear();

            isSplit = true;

        }

        byte GetCornerID(ulong targetFullID, int depth) {

            int shift = (depth) * LocationConfig.DEPTH_SHIFT;

            ulong loc = targetFullID & ((ulong)LocationConfig.FULL << shift);

            return (byte)(loc >> shift);

        }

        // ==== Remove ====
        internal void Remove(ulong targetFullID, int depth) {

            byte targetCornerID = GetCornerID(targetFullID, depth);
            if (targetCornerID == LocationConfig.NONE || targetCornerID > LocationConfig.FULL) {
                return;
            }

            uint targetOnlyID = (uint)(targetFullID >> 32);

            if (isSplit) {
                for (int i = 0; i < splitedArray.Length; i += 1) {
                    var corner = splitedArray[i];
                    if (corner != null) {
                        if (corner.IsLeaf()) {
                            if (corner.onlyID == targetOnlyID) {
                                corner.SetAsBranch();
                            }
                        } else {
                            corner.Remove(targetFullID, depth + 1);
                        }
                    }
                }
            }

            int index = children.FindIndex(value => value.onlyID == targetOnlyID);
            if (index != -1) {
                children.RemoveAt(index);
            }

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
                for (int i = 0; i < children.Count; i += 1) {
                    var child = children[i];
                    child.GetCandidates(bounds, candidates);
                }
            }

        }

    }

}