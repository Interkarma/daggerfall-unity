using UnityEngine;
using System.IO;

namespace CSharpCompiler
{
    class UnityLogTextWriter : TextWriter
    {
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.ASCII; }
        }
        public override void Write(string value)
        {
            Debug.Log(value);
        }
    }
}