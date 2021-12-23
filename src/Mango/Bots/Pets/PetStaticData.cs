using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Bots.Pets
{
    static class PetStaticData
    {
        public static readonly List<int> EXPERIENCE_LEVELS = new List<int>()
        {
            100, 200, 400, 600, 900, 1300, 1800, 2400, 3100, 3900, 4800, 5800, 6900, 8100, 9400, 10800, 12300, 14000, 15800, 17700
        };

        public static readonly List<int> ENERGY_LEVELS = new List<int>()
        {
            120, 140, 160, 180, 200, 220, 240, 260, 280, 300, 320, 340, 360, 380, 400, 420, 440, 460, 480, 500
        };
    }
}
