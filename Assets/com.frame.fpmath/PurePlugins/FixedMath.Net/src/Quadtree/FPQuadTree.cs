using System;
using System.Collections.Generic;

namespace FixMath.NET {

    internal interface Ptr_FPQuadTree {}

    public class FPQuadTree<T> : Ptr_FPQuadTree {

        // Config
        int maxDepth;
        public int MaxDepth => maxDepth;

        FPQuadTreeNode<T> root;
        public FPQuadTreeNode<T> Root => root;

        public FPQuadTree(FP64 worldWidth, FP64 worldHeight, int maxDepth) {
            this.maxDepth = maxDepth;

            var bounds = new FPBounds2(FPVector2.Zero, new FPVector2(worldWidth, worldHeight));
            this.root = new FPQuadTreeNode<T>(this, bounds, 0);
        }

        public void Traval(Action<FPQuadTreeNode<T>> action) {
            root.Traval(action);
        }

        public void Insert(T valuePtr, in FPBounds2 bounds) {
            this.root.Insert(valuePtr, bounds);
        }

        public void GetCandidates(in FPBounds2 bounds, List<T> candidates) {
            this.root.GetCandidates(bounds, candidates);
        }

    }

}