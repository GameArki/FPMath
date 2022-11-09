using System;
using System.Collections.Generic;

namespace FixMath.NET {

    public class FPQuadTreeNode<T> {

        const int CHILDREN_COUNT = 4;

        Ptr_FPQuadTree treePtr;
        FPQuadTree<T> Tree => treePtr as FPQuadTree<T>;

        object valuePtr;
        public T Value => (T)valuePtr;

        FPBounds2 bounds;
        public FPBounds2 Bounds => bounds;

        int depth;
        public int Depth => depth;

        bool isSplit;

        // 分割前, 无序
        List<FPQuadTreeNode<T>> children;

        // 分割后, 有序
        FPQuadTreeNode<T> bl;
        FPQuadTreeNode<T> br;
        FPQuadTreeNode<T> tl;
        FPQuadTreeNode<T> tr;

        internal FPQuadTreeNode(Ptr_FPQuadTree tree, in FPBounds2 bounds, int depth) {
            this.treePtr = tree;
            this.isSplit = false;
            this.bounds = bounds;
            this.depth = depth;
            this.children = new List<FPQuadTreeNode<T>>(CHILDREN_COUNT);
        }

        internal void Traval(Action<FPQuadTreeNode<T>> action) {

            action.Invoke(this);

            if (isSplit) {
                bl.Traval(action);
                br.Traval(action);
                tl.Traval(action);
                tr.Traval(action);
            } else {
                children.ForEach(action);
            }
        }

        void SetAsLeaf(T valuePtr) {
            this.valuePtr = valuePtr;
        }

        void SetAsBranch() {
            this.valuePtr = null;
        }

        public bool IsLeaf() {
            return valuePtr != null;
        }

        // ==== Insert ====
        internal void Insert(T valuePtr, in FPBounds2 bounds) {

            int nextDepth = depth + 1;

            var node = new FPQuadTreeNode<T>(treePtr, bounds, nextDepth);
            node.SetAsLeaf(valuePtr);

            SetAsBranch();

            InsertNode(node);

        }

        void InsertNode(FPQuadTreeNode<T> node) {

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

            if (bl.IsIntersectOrContains(nodeBounds)) {
                bl.InsertNode(node);
            }

            if (br.IsIntersectOrContains(nodeBounds)) {
                br.InsertNode(node);
            }

            if (tl.IsIntersectOrContains(nodeBounds)) {
                tl.InsertNode(node);
            }

            if (tr.IsIntersectOrContains(nodeBounds)) {
                tr.InsertNode(node);
            }
        }

        void InsertNodeWhenNotSplit(FPQuadTreeNode<T> node) {

            // Children 小于 4 个时, 插入
            if (children.Count < CHILDREN_COUNT) {
                children.Add(node);
                return;
            }

            // Children 等于 4 个时, 首次分割
            if (children.Count == CHILDREN_COUNT) {

                int nextDepth = depth + 1;
                var size = bounds.size * FP64.Half;
                var halfSize = size * FP64.Half;
                var center = bounds.center;

                var blBounds = new FPBounds2(center - halfSize, size);
                var brBounds = new FPBounds2(new FPVector2(center.x + halfSize.x, center.y - halfSize.y), size);
                var tlBounds = new FPBounds2(new FPVector2(center.x - halfSize.x, center.y + halfSize.y), size);
                var trBounds = new FPBounds2(center + halfSize, size);

                bl = new FPQuadTreeNode<T>(treePtr, blBounds, nextDepth);
                br = new FPQuadTreeNode<T>(treePtr, brBounds, nextDepth);
                tl = new FPQuadTreeNode<T>(treePtr, tlBounds, nextDepth);
                tr = new FPQuadTreeNode<T>(treePtr, trBounds, nextDepth);

                children.ForEach(child => {
                    var childBounds = child.bounds;
                    if (bl.IsIntersectOrContains(childBounds)) {
                        bl.InsertNode(child);
                    }
                    if (br.IsIntersectOrContains(childBounds)) {
                        br.InsertNode(child);
                    }
                    if (tl.IsIntersectOrContains(childBounds)) {
                        tl.InsertNode(child);
                    }
                    if (tr.IsIntersectOrContains(childBounds)) {
                        tr.InsertNode(child);
                    }
                });

                children.Clear();

                isSplit = true;

                InsertNodeWhenSplit(node);

            }

        }

        bool IsIntersectOrContains(in FPBounds2 other) {
            return bounds.IsIntersect(other) || bounds.IsContains(other);
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
                bl.GetCandidates(bounds, candidates);
                br.GetCandidates(bounds, candidates);
                tl.GetCandidates(bounds, candidates);
                tr.GetCandidates(bounds, candidates);
            } else {
                for (int i = 0; i < children.Count; i += 1) {
                    var child = children[i];
                    child.GetCandidates(bounds, candidates);
                }
            }

        }

    }

}