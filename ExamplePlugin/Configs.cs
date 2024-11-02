using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongstandingSolitudeFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Tutorials.ConfigurationTutorial", "ConfigurationTutorial", "1.0.0")]
    public class Configs : BaseUnityPlugin
    {
        private static ConfigFile CustomConfigFile { get; set; }
        public static ConfigEntry<bool> BreakIntoPearl { get; set; }

        internal static void Init()
        {
            CustomConfigFile = new ConfigFile(Paths.ConfigPath + "\\LongstandingSolitudeFix.cfg", true);

            BreakIntoPearl = CustomConfigFile.Bind<bool>(
                       "LongstandingSolitudeFix",
                       "BreakIntoPearlAtMaxLevel",
                       true,
                       "Whether or not to break Longstanding Solitude into a pearl at max level"
                   );
        }
    }
}
