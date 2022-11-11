using System;
using System.Collections.Generic;
using FixMath.NET;

namespace JackFrame.FPMath {

    internal interface Ptr_FPOctree {}

    public class FPOctree<T> : Ptr_FPOctree {

        int maxDepth;
        public int MaxDepth => maxDepth;

        uint onlyIDRecord;

        FPOctreeNode<T> root;

        public FPOctree(FP64 worldWidth, FP64 worldHeight, FP64 worldLength, int maxDepth) {
            if (maxDepth > 8) {
                throw new Exception("Max depth must be less than 8");
            }

            this.maxDepth = maxDepth;
            this.root = new FPOctreeNode<T>(this, new FPBounds3(FPVector3.Zero, new FPVector3(worldWidth, worldHeight, worldLength)), 0);
            this.root.SetAsRoot();
        }

        internal uint GenOnlyID() {
            onlyIDRecord += 1;
            return onlyIDRecord;
        }

        public FPOctreeNode<T> Insert(T value, FPBounds3 bounds) {
            return root.Insert(value, bounds);
        }

        public void Remove(uint128 fullID) {
            root.RemoveNode(fullID);
        }

        public void GetCandidateNodes(in FPBounds3 bounds, HashSet<FPOctreeNode<T>> candidates) {
            root.GetCandidateNodes(bounds, candidates);
        }

        public void GetCandidateValues(in FPBounds3 bounds, HashSet<T> candidates) {
            root.GetCandidateValues(bounds, candidates);
        }

        public void Traval(Action<FPOctreeNode<T>> action) {
            root.Traval(action);
        }

    }

}