using System;
using System.IO;
using Xunit;

namespace GDFParser.Tests
{
    public class MainTest
    {
        [Fact]
        public void WhenAValidFile_ShouldBuildGraph()
        {
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "pagenetwork_835133816563977_2018_01_17_22_03_35.gdf");

            var parser = new GDFParser();
            parser.LoadFile(path);
            parser.Parse();

            var graph = parser.GetGraph();
            Assert.NotNull(graph);
        }
    }
}
