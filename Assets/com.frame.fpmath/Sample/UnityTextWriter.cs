using System.Text;
using System.IO;
using UnityEngine;

namespace JackFrame.FPMath.Sample {

    public class UnityTextWriter : TextWriter {

        public override void WriteLine(string value) {
            Debug.Log(value);
        }

        public override Encoding Encoding {
            get { return Encoding.UTF8; }
        }

    }

}