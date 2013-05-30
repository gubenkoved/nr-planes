using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Tests
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void AngleBetweenTest1()
        {
            double angle = Vector.AngleBetween(new Vector(-1, -1), new Vector(1, 1));

            Assert.AreEqual(180, angle);
        }

        [TestMethod]
        public void AngleBetweenTest2()
        {
            double angle = Vector.AngleBetween(new Vector(1, 0), new Vector(1, 1) - new Vector(-1, -1));

            Assert.AreEqual(45, angle);
        }

        [TestMethod]
        public void AngleBetweenTest3()
        {
            double angle = Vector.AngleBetween(new Vector(1, 0), new Vector(1, 1) - new Vector(0, 1));

            Assert.AreEqual(0, angle);
        }

        [TestMethod]
        public void AngleBetweenTest4()
        {
            double angle = Vector.AngleBetween(new Vector(1, 0), new Vector(1, 1) - new Vector(1, 0));

            double angle2 = Helper.RelativeAngleBetweenPositions(new Vector(1, 0), new Vector(1, 1));

            Assert.AreEqual(angle2, angle);
            Assert.AreEqual(90, angle);
        }


    }
}
