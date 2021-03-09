using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNUnit.Attributes;

namespace Testing2
{

    public class TestingClass2
    {
        public static bool ignored;

        [Test(Ignore = "ignore")]
        public void IgnoredTest()
        {
            ignored = false;
        }
    }
}
