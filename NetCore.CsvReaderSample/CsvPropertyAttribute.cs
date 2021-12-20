using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.CsvReaderSample
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public CsvPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}
