using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// annotation for after testing 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClassAttribute : Attribute
    {
    }
}
