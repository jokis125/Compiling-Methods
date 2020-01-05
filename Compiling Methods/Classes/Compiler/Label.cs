using System;
using System.Collections.Generic;

namespace CompilingMethods.Classes.Compiler
{
    public class Label
    {
        private List<int> offsets = new List<int>();
        private int value = -1;

        public List<int> Offsets
        {
            get => offsets;
            set => offsets = value;
        }

        public int Value
        {
            get => value;
            set => this.value = value;
        }
    }
}