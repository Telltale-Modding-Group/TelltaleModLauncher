using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    class GameVersion
    {
        /// <summary>
        /// The supported Game Versions.
        /// <para>Add supported game versions in here</para>
        /// <para>NOTE: this is only a name identifier, it needs to be supported by the code too!</para>
        /// </summary>
        public enum GameVersions
        {
            Other = 0,
            The_Walking_Dead_Definitive_Edition = 1
        };
    }
}
