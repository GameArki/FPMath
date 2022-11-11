using System;
using System.Collections.Generic;
using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPMath.Sample {

    public class Sample_Octree : MonoBehaviour {

        FPOctree<string> tree;

        System.Random rd;
        int width = 100;
        int height = 100;
        int length = 100;

        int sizeMax = 10;
        int sizeMin = 1;

        GameObject cube;

        List<FPOctreeNode<string>> candidates;

        void Awake() {
            Console.SetOut(new UnityTextWriter());

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            rd = new System.Random();
            tree = new FPOctree<string>(width, height, length, 8);
            candidates = new List<FPOctreeNode<string>>();

        }

        void Update() {

            MoveCube();

            var worldPos = cube.transform.position;
            var worldFPPos = worldPos.ToFPVector3();
            var size = cube.transform.localScale.ToFPVector3();

            if (Input.GetMouseButtonDown(1)) {
                var randomSize = new FPVector3(rd.Next(sizeMin, sizeMax), rd.Next(sizeMin, sizeMax), rd.Next(sizeMin, sizeMax));
                tree.Insert("1", new FPBounds3(worldFPPos, randomSize));
            }

            if (Input.GetMouseButtonDown(2)) {
                var node = candidates.Find(value => (value.Bounds.Center - worldFPPos).LengthSquared() < 50);
                if (node != null) {
                    tree.Remove(node.GetFullID());
                }
            }

            candidates.Clear();
            tree.GetCandidates(new FPBounds3(worldFPPos, size), candidates);

        }

        void MoveCube() {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            float z = Input.mouseScrollDelta.y;

            cube.transform.position += new Vector3(x, y, z);
        }

        void OnDrawGizmos() {

            if (tree == null) {
                return;
            }

            var worldPos = cube.transform.position;
            var worldFPPos = worldPos.ToFPVector3();
            var size = cube.transform.localScale;

            Gizmos.color = Color.red;
            tree.Traval(value => {
                var center = value.Bounds.Center;
                var size = value.Bounds.Size;
                Gizmos.DrawWireCube(center.ToVector3(), size.ToVector3());
            });

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(worldPos, size);

            if (candidates == null) {
                return;
            }

            Gizmos.color = Color.green;
            candidates.ForEach(value => {
                var center = value.Bounds.Center;
                var size = value.Bounds.Size;
                Gizmos.DrawWireCube(center.ToVector3(), size.ToVector3());
            });

        }

    }

}