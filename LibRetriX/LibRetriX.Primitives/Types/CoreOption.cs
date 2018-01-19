using System;
using System.Collections.Generic;

namespace LibRetriX
{
    public class CoreOption
    {
        public string Description { get; private set; }
        public IReadOnlyList<string> Values { get; private set; }

        private uint selectedValueIx;
        public uint SelectedValueIx
        {
            get => selectedValueIx;
            set
            {
                if (value >= Values.Count)
                {
                    throw new ArgumentException();
                }

                selectedValueIx = value;
            }
        }

        public CoreOption(string description, IReadOnlyList<string> values)
        {
            Description = description;
            Values = values;
        }
    }
}
