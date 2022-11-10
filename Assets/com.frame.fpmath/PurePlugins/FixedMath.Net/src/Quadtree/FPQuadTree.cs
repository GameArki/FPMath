using System;
using System.Collections.Generic;

namespace FixMath.NET {

    internal interface Ptr_FPQuadTree {}

    public class FPQuadTree<T> : Ptr_FPQuadTree {

        // Config
        int maxDepth;
        public int MaxDepth => maxDepth;

        uint onlyIDRecord;

        FPQuadTreeNode<T> root;
        public FPQuadTreeNode<T> Root => root;

        public FPQuadTree(FP64 worldWidth, FP64 worldHeight, int maxDepth) {
            if (maxDepth > 8) {
                throw new Exception("Max depth must be less than 8");
            }
            this.maxDepth = maxDepth;

            var bounds = new FPBounds2(FPVector2.Zero, new FPVector2(worldWidth, worldHeight));
            this.root = new FPQuadTreeNode<T>(this, bounds, 0);
        }

        internal uint GenOnlyID() {
            onlyIDRecord += 1;
            return onlyIDRecord;
        }

        public void Traval(Action<FPQuadTreeNode<T>> action) {
            root.Traval(action);
        }

        public void Insert(T valuePtr, in FPBounds2 bounds) {
            this.root.Insert(valuePtr, bounds);
        }

        public void Remove(ulong fullID) {
            this.root.Remove(fullID, 0);
        }

        public void GetCandidates(in FPBounds2 bounds, List<FPQuadTreeNode<T>> candidates) {
            this.root.GetCandidates(bounds, candidates);
        }

    }

}