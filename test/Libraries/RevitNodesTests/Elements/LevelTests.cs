using System;
using System.Linq;
using Autodesk.Revit.DB;
using NUnit.Framework;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using Revit.Elements.InternalUtilities;
using Level = Revit.Elements.Level;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class LevelTests : RevitNodeTestBase
    {
        public static double InternalElevation(Level level)
        {
            return ((Autodesk.Revit.DB.Level)level.InternalElement).Elevation;
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByElevationAndName_ShouldProduceLevelAtCorrectElevation()
        {
            // construct the extrusion
            const double elevation = 100.0;
            const string name = "Ham";
            var level = Level.ByElevationAndName(elevation, name);
            Assert.NotNull(level);

            level.Elevation.ShouldBeApproximately(elevation);
            level.ProjectElevation.ShouldBeApproximately(elevation);

            Assert.AreEqual(name, level.Name);

            // without unit conversion
            InternalElevation(level)
                .ShouldBeApproximately(elevation * UnitConverter.DynamoToHostFactor(UnitType.UT_Length));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByElevationAndName_NullArgument()
        {
            const int elevation = 100;

            Assert.Throws(typeof(ArgumentNullException), () => Level.ByElevationAndName(elevation, null));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByElevationAndName_DuplicatedNames()
        {
            //Create a new level with the name of "Ham" and the
            //elevation of 100
            const double elevation = 100.0;
            const string name = "Old Level";
            var level = Level.ByElevationAndName(elevation, name);
            Assert.NotNull(level);

            level.Elevation.ShouldBeApproximately(elevation);
            Assert.AreEqual(name, level.Name);

            //Create a new level with the same name and elevation
            level = Level.ByElevationAndName(elevation, name);

            level.Elevation.ShouldBeApproximately(elevation);
            Assert.AreEqual(name + "(1)", level.Name);

            //Once again create a new level with the same name and elevation
            level = Level.ByElevationAndName(elevation, name);

            level.Elevation.ShouldBeApproximately(elevation);
            Assert.AreEqual(name + "(2)", level.Name);

            //Create a new level with a name of lowercase letters
            //and the same elevation
            const string name3 = "old level";
            var level3 = Level.ByElevationAndName(elevation, name3);
            Assert.IsNotNull(level3);

            level3.Elevation.ShouldBeApproximately(elevation);
            Assert.AreEqual(name3, level3.Name);

            //Create a new level with a totally different name
            const string name4 = "New level";
            var level4 = Level.ByElevationAndName(elevation, name4);
            Assert.NotNull(level4);

            level4.Elevation.ShouldBeApproximately(elevation);
            Assert.AreEqual(name4, level4.Name);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByElevation_ShouldProduceLevelAtCorrectElevation()
        {
            const double elevation = 100.0;
            var level = Level.ByElevation(elevation);
            Assert.NotNull(level);

            level.Elevation.ShouldBeApproximately(elevation);
            level.ProjectElevation.ShouldBeApproximately(elevation);

            // without unit conversion
            InternalElevation(level)
                .ShouldBeApproximately(elevation * UnitConverter.DynamoToHostFactor(UnitType.UT_Length));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLevelAndOffset_ValidArgs()
        {
            const double elevation = 100.0;
            const double offset = 100.0;
            var level = Level.ByElevation(elevation);

            var level2 = Level.ByLevelAndOffset(level, offset);
            Assert.NotNull(level2);

            level2.Elevation.ShouldBeApproximately(elevation + offset);
            level2.ProjectElevation.ShouldBeApproximately(elevation + offset);

            // without unit conversion
            InternalElevation(level2)
                .ShouldBeApproximately((elevation + offset) * UnitConverter.DynamoToHostFactor(UnitType.UT_Length));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLevelAndOffset_NullArgument()
        {
            const int offset = 100;
            Assert.Throws(typeof(ArgumentNullException), () => Level.ByLevelAndOffset(null, offset));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLevelOffsetAndName_ShouldProduceLevelAtCorrectElevation()
        {
            const int elevation = 100;
            const int offset = 100;
            const string name = "TortoiseTime";
            var level = Level.ByElevation(elevation);

            var level2 = Level.ByLevelOffsetAndName(level, offset, name);
            Assert.NotNull(level2);

            level2.Elevation.ShouldBeApproximately(elevation + offset);
            level2.ProjectElevation.ShouldBeApproximately(elevation + offset);

            // without unit conversion
            InternalElevation(level2)
                .ShouldBeApproximately((elevation + offset) * UnitConverter.DynamoToHostFactor(UnitType.UT_Length));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLevelOffsetAndName_NullArgument()
        {
            const int elevation = 100;
            const int offset = 100;
            const string name = "Ham";
            var level = Level.ByElevation(elevation);

            Assert.Throws(typeof(ArgumentNullException), () => Level.ByLevelOffsetAndName(null, offset, name));
            Assert.Throws(typeof(ArgumentNullException), () => Level.ByLevelOffsetAndName(level, offset, null));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void GetAllLevels()
        {
            var levels = ElementQueries.GetAllLevels();
            Assert.AreEqual(levels.Count(), 1.0);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void LevelByName_ValidArgs()
        {
            const double elevation = 100;
            var level = Level.ByElevation(elevation);
            Assert.IsNotNull(level);

            var selected = Level.GetByName(level.Name);
            Assert.IsNotNull(selected);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void LevelByName_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => Level.GetByName(null));
            Assert.Throws(typeof(Exception), () => Level.GetByName("InvalidLevelName"));
        }
    }
}

