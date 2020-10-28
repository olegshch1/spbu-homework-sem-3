using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// annotation for before testing 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeClassAttribute : Attribute
    {
    }
}
