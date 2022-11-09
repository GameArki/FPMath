using System;
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

        void Awake() {
            Console.SetOut(new UnityTextWriter());
            rd = new System.Random();
            tree = new FPQuadTree<string>(width, height, 12);
            System.Console.WriteLine("yo");
        }

        void Update() {

            if (Input.GetMouseButtonDown(1)) {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tree.Insert("1", new FPBounds2(
                    new FPVector2(FP64.ToFP64(worldPos.x), FP64.ToFP64(worldPos.y)),
                    new FPVector2(rd.Next(sizeMin, sizeMax), rd.Next(sizeMin, sizeMax))
                ));
            }

            // System.Console.WriteLine(worldPos.ToString());

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

        }

    }

}