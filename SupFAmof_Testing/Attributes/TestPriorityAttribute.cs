using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof_Testing.Attributes
{
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; set; }
        public TestPriorityAttribute(int priority) => Priority = priority;
    }
}
