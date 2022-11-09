using System;
using System.Collections.Generic;
using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPMath.Sample {

    public class UnityTextWriter : System.IO.TextWriter {

        public override void WriteLine(string value) {
            Debug.Log(value);
        }

        public override System.Text.Encoding Encoding {
            get { return System.Text.Encoding.UTF8; }
        }

    }

    public class Sample_QuadTree : MonoBehaviour {

        FPQuadTree<string> tree;

        System.Random rd;
        int width = 1000;
        int height = 1000;

        int sizeMax = 100;
        int sizeMin = 1;

        FPVector2 mousePos;
        FPVector2 mouseSize = new FPVector2(20, 20);

        List<FPQuadTreeNode<string>> candidates;

        void Awake() {
            Console.SetOut(new UnityTextWriter());
            rd = new System.Random();
            tree = new FPQuadTree<string>(width, height, 12);
            candidates = new List<FPQuadTreeNode<string>>();
        }

        void Update() {

            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new FPVector2(FP64.ToFP64(worldPos.x), FP64.ToFP64(worldPos.y));
            if (Input.GetMouseButtonDown(1)) {
                tree.Insert("1", new FPBounds2(mousePos, new FPVector2(rd.Next(sizeMin, sizeMax), rd.Next(sizeMin, sizeMax))
                ));
            }

            candidates.Clear();
            tree.GetCandidates(new FPBounds2(mousePos, mouseSize), candidates);

        }

        void OnDrawGizmos() {
            if (tree == null) {
                return;
            }

            Gizmos.color = Color.red;
            tree.Traval(value => {
                var center = value.Bounds.center;
                var size = value.Bounds.size;
                Gizmos.DrawWireCube(new Vector3(center.x.AsFloat(), center.y.AsFloat()), new Vector3(size.x.AsFloat(), size.y.AsFloat()));
            });

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(new Vector3(mousePos.x.AsFloat(), mousePos.y.AsFloat()), new Vector3(mouseSize.x.AsFloat(), mouseSize.y.AsFloat()));

            if (candidates == null) {
                return;
            }

            Gizmos.color = Color.green;
            candidates.ForEach(value => {
                var center = value.Bounds.center;
                var size = value.Bounds.size;
                Gizmos.DrawWireCube(new Vector3(center.x.AsFloat(), center.y.AsFloat()), new Vector3(size.x.AsFloat(), size.y.AsFloat()));
            });

        }

    }

}