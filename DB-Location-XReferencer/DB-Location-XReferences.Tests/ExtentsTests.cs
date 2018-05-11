using System;
using System.Collections.Generic;
using DB_Location_XReferencer.Core;
using NUnit.Framework;
using SharpKml.Base;

namespace DB_Location_XReferences.Tests
{
    [TestFixture]
    public class ExtentsTests
    {
        /// <summary>
        /// A 5-point star that spans from pole-to-pole.
        /// </summary>
        public static List<Vector> StarGlobalPoles = new List<Vector>()
        {
            new Vector(90, 0),
            new Vector(24, 20),
            new Vector(24, 90),
            new Vector(-20, 40),
            new Vector(-90, 60),
            new Vector(-45, 0),
            new Vector(-90, -60),
            new Vector(-20, -40),
            new Vector(24, -90),
            new Vector(24, -20),
        };

        [Test]
        public void Add_Beyond_Max_Limits_To_Max()
        {
            Extents extents = new Extents();
            Assert.That(extents.MaxLatitude, Is.EqualTo(-90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(-180));
            Assert.That(extents.MinLongitude, Is.EqualTo(180));

            extents.Add(new Vector(300, 10));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(10));
            Assert.That(extents.MinLongitude, Is.EqualTo(10));

            extents.Add(new Vector(10, 300));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(10));
            Assert.That(extents.MaxLongitude, Is.EqualTo(180));
            Assert.That(extents.MinLongitude, Is.EqualTo(10));

            extents.Add(new Vector(-10, -300));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-10));
            Assert.That(extents.MaxLongitude, Is.EqualTo(180));
            Assert.That(extents.MinLongitude, Is.EqualTo(-180));

            extents.Add(new Vector(-300, -10));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(180));
            Assert.That(extents.MinLongitude, Is.EqualTo(-180));
        }


        [Test]
        public void Add_Points()
        {
            Extents extents = new Extents();
            Assert.That(extents.MaxLatitude, Is.EqualTo(-90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(-180));
            Assert.That(extents.MinLongitude, Is.EqualTo(180));

            // Pt 1
            int point = 1;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(0));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 2
            point = 2;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(24));
            Assert.That(extents.MaxLongitude, Is.EqualTo(20));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 3
            point = 3;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(24));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 4
            point = 4;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-20));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 5
            point = 5;
            extents.Add(new Vector(StarGlobalPoles[0].Latitude, StarGlobalPoles[0].Longitude));
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 6
            point = 6;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(0));

            // Pt 7
            point = 7;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-60));

            // Pt 8
            point = 8;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-60));

            // Pt 9
            point = 9;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-90));

            // Pt 10
            point = 10;
            extents.Add(new Vector(StarGlobalPoles[point - 1].Latitude, StarGlobalPoles[point - 1].Longitude));
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-90));
        }

        [Test]
        public void Add_IEnumerable()
        {
            Extents extents = new Extents();
            Assert.That(extents.MaxLatitude, Is.EqualTo(-90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(-180));
            Assert.That(extents.MinLongitude, Is.EqualTo(180));
            
            extents.Add(StarGlobalPoles);
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-90));
        }

        [Test]
        public void Add_Extents()
        {
            Extents extents = new Extents();
            Assert.That(extents.MaxLatitude, Is.EqualTo(-90));
            Assert.That(extents.MinLatitude, Is.EqualTo(90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(-180));
            Assert.That(extents.MinLongitude, Is.EqualTo(180));

            extents.Add(StarGlobalPoles);
            Assert.That(extents.MaxLatitude, Is.EqualTo(90));
            Assert.That(extents.MinLatitude, Is.EqualTo(-90));
            Assert.That(extents.MaxLongitude, Is.EqualTo(90));
            Assert.That(extents.MinLongitude, Is.EqualTo(-90));

            Extents newExtents = new Extents();
            newExtents.Add(extents);
            Assert.That(newExtents.MaxLatitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLatitude, Is.EqualTo(-90));
            Assert.That(newExtents.MaxLongitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLongitude, Is.EqualTo(-90));

            Extents wideExtents = new Extents();
            wideExtents.Add(new Vector(45, 120));
            newExtents.Add(wideExtents);
            Assert.That(newExtents.MaxLatitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLatitude, Is.EqualTo(-90));
            Assert.That(newExtents.MaxLongitude, Is.EqualTo(120));
            Assert.That(newExtents.MinLongitude, Is.EqualTo(-90));
            
            Extents wideNegativeExtents = new Extents();
            wideNegativeExtents.Add(new Vector(-35, -140));
            newExtents.Add(wideNegativeExtents);
            Assert.That(newExtents.MaxLatitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLatitude, Is.EqualTo(-90));
            Assert.That(newExtents.MaxLongitude, Is.EqualTo(120));
            Assert.That(newExtents.MinLongitude, Is.EqualTo(-140));
        }

        [Test]
        public void IsWithinExtents_Within_Extents_Returns_True()
        {
            Extents extents = new Extents();
            extents.Add(StarGlobalPoles);

            Assert.IsTrue(extents.IsWithinExtents(25, 10));
        }

        [Test]
        public void IsWithinExtents_Not_Within_Extents_Returns_False()
        {
            Extents extents = new Extents();
            extents.Add(StarGlobalPoles);

            Assert.IsFalse(extents.IsWithinExtents(25, 100));   // Partially within extents
            Assert.IsFalse(extents.IsWithinExtents(95, 10));    // Partial within extents
            Assert.IsFalse(extents.IsWithinExtents(95, 100));   // Totally within extents
        }

        [Test]
        public void GetExtents_Returns_New_Extents()
        {
            Extents newExtents = Extents.GetExtents(StarGlobalPoles);
            Assert.That(newExtents.MaxLatitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLatitude, Is.EqualTo(-90));
            Assert.That(newExtents.MaxLongitude, Is.EqualTo(90));
            Assert.That(newExtents.MinLongitude, Is.EqualTo(-90));
        }
    }
}
