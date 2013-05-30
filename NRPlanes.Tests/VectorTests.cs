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
            double angle = Vector.AngleBetween(new Vector(0, -1), new Vector(1, 1));

            Assert.AreEqual(-135, angle);
        }

        [TestMethod]
        public void AngleBetweenTest2()
        {
            double angle = Vector.AngleBetween(new Vector(1, 0), new Vector(1, 1));

            Assert.AreEqual(-45, angle);
        }

        [TestMethod]
        public void AngleBetweenTest3()
        {
            double angle = Vector.AngleBetween(new Vector(1, 0), new Vector(1, 0));

            Assert.AreEqual(0, angle);
        }

        [TestMethod]
        public void RelativeAngleBetweenPositions()
        {
            double angle1 = Helper.RelativeAngleBetweenPositions(new Vector(0, 0), new Vector(1, 1));
            double angle2 = Helper.RelativeAngleBetweenPositions(new Vector(0, 0), new Vector(1, -1));
            double angle3 = Helper.RelativeAngleBetweenPositions(new Vector(0, 0), new Vector(-1, -1));
            double angle4 = Helper.RelativeAngleBetweenPositions(new Vector(0, 0), new Vector(-1, 1));
            
            Assert.AreEqual(45, angle1);
            Assert.AreEqual(135, angle2);
            Assert.AreEqual(-135, angle3);
            Assert.AreEqual(-45, angle4);
        }


    }
}
