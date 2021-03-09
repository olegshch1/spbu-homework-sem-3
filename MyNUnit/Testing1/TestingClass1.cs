using MyNUnit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing1
{
    public class TestingClass1
    {
        public static int count;

        [Test]
        public void Incrementing()
        {
            count++;
        }
    }
}
