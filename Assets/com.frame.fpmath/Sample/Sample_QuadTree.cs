using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPMath.Sample {

    public class Sample_QuadTree : MonoBehaviour {

        FPQuadTree<string> tree;

        System.Random rd;
        int width = 1000;
        int height = 1000;

        int sizeMax = 100;
        int sizeMin = 1;

        FPVector2 mousePos;
        FPVector2 mouseSize = new FPVector2(20, 20);

        HashSet<FPQuadTreeNode<string>> candidates;

        void Awake() {
            Console.SetOut(new UnityTextWriter());
            rd = new System.Random();
            tree = new FPQuadTree<string>(width, height, 8);
            candidates = new HashSet<FPQuadTreeNode<string>>();
        }

        void Update() {

            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = worldPos.ToFPVector2();
            if (Input.GetMouseButtonDown(1)) {
                var randomSize = new FPVector2(rd.Next(sizeMin, sizeMax), rd.Next(sizeMin, sizeMax));
                tree.Insert("1", new FPBounds2(mousePos, randomSize));
            }

            if (Input.GetMouseButtonDown(2)) {
                var node = candidates.First(value => (value.Bounds.Center - mousePos).LengthSquared() < 1000);
                if (node != null) {
                    tree.Remove(node.GetFullID());
                    // tree.Insert(node.Value, new FPBounds2(mousePos, node.Bounds.Size));
                }

            }

            candidates.Clear();
            tree.GetCandidateNodes(new FPBounds2(mousePos, mouseSize), candidates);

        }

        void OnDrawGizmos() {
            if (tree == null) {
                return;
            }

            Gizmos.color = Color.red;
            tree.Traval(value => {
                var center = value.Bounds.Center;
                var size = value.Bounds.Size;
                Gizmos.DrawWireCube(center.ToVector2(), size.ToVector2());
            });

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(mousePos.ToVector2(), mouseSize.ToVector2());

            if (candidates == null) {
                return;
            }

            Gizmos.color = Color.green;
            foreach (var value in candidates) {
                var center = value.Bounds.Center;
                var size = value.Bounds.Size;
                Gizmos.DrawWireCube(center.ToVector2(), size.ToVector2());
            }
        }

    }

}