using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carlos.Media.Tests
{
    [TestClass()]
    public class MidiGeneratorTests
    {
        [TestMethod()]
        public void GenerateMidiFileTest()
        {
            (int noteNumber, int velocity, int duration)[] notes = new (int, int, int)[]
            {
                (60, 100, 480), // C4
                (62, 100, 480), // D4
                (64, 100, 480), // E4
                (65, 100, 480), // F4
                (67, 100, 480), // G4
                (69, 100, 480), // A4
                (71, 100, 480), // B4
                (72, 100, 480)  // C5
            };
            MidiGenerator.GenerateMidiFile(@"D:\test.mid", notes);
        }
    }
}
