using System;
using System.Collections.Generic;
using FixMath.NET;

namespace JackFrame.FPMath {

    internal interface Ptr_FPQuadTree {}

    public class FPQuadTree<T> : Ptr_FPQuadTree {

        // Config
        int maxDepth;
        public int MaxDepth => maxDepth;

        uint onlyIDRecord;

        FPQuadTreeNode<T> root;
        internal FPQuadTreeNode<T> Root => root;

        public FPQuadTree(FP64 worldWidth, FP64 worldHeight, int maxDepth) {
            if (maxDepth > 8) {
                throw new Exception("Max depth must be less than 8");
            }
            this.maxDepth = maxDepth;

            var bounds = new FPBounds2(FPVector2.Zero, new FPVector2(worldWidth, worldHeight));
            this.root = new FPQuadTreeNode<T>(this, bounds, 0);
            this.root.SetAsRoot();
        }

        // ==== API ====
        public void Traval(Action<FPQuadTreeNode<T>> action) {
            root.Traval(action);
        }

        public FPQuadTreeNode<T> Insert(T valuePtr, in FPBounds2 bounds) {
            return root.Insert(valuePtr, bounds);
        }

        public void Remove(ulong fullID) {
            this.root.RemoveNode(fullID);
        }

        public void GetCandidateNodes(in FPBounds2 bounds, HashSet<FPQuadTreeNode<T>> candidates) {
            this.root.GetCandidateNodes(bounds, candidates);
        }

        public void GetCandidateValues(in FPBounds2 bounds, HashSet<T> candidates) {
            this.root.GetCandidateValues(bounds, candidates);
        }

        // ==== Internal ====
        internal uint GenOnlyID() {
            onlyIDRecord += 1;
            return onlyIDRecord;
        }

    }

}