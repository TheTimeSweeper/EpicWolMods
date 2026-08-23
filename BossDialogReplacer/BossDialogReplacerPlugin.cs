using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossDialogReplacer
{
    [BepInPlugin("TheTimeSweeper.BossDialogReplacer", "BossDialogReplacer", "0.1.0")]
    public class BossDialogReplacerPlugin : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;
        private string fire;
        private string wind;
        private string eart;
        private string ligt;
        private string watr;
        private string more;

        private List<DialogReplacement> replacements = new List<DialogReplacement>();

        void Awake()
        {
            pluginInfo = Info;

            Configger();
            DeConfigger();
            TestConfigger();

            On.DialogManager.InitDialogDictionary += DialogManager_InitDialogDictionary;
        }

        private void Configger()
        {
            string bossSection = "Boss";

            fire =
                Config.Bind(bossSection,
                    "FireBoss Text",
                    "FireBoss-ExampleText1~FireBoss-ExampleText2~FireBoss-ExampleText3").Value;
            wind =
                Config.Bind(bossSection,
                    "AirBoss Text",
                    "AirBoss-ExampleText1~AirBoss-ExampleText2~AirBoss-ExampleText3").Value;
            eart =
                Config.Bind(bossSection,
                    "EarthBoss Text",
                    "EarthBoss-ExampleText1~EarthBoss-ExampleText2~EarthBoss-ExampleText3").Value;
            ligt =
                Config.Bind(bossSection,
                    "LightningBoss Text",
                    "LightningBoss-ExampleText1~LightningBoss-ExampleText2~LightningBoss-ExampleText3").Value;
            watr =
                Config.Bind(bossSection,
                    "IceBoss Text",
                    "IceBoss-ExampleText1~IceBoss-ExampleText2~IceBoss-ExampleText3").Value;

            more =
                Config.Bind("More",
                    "Other Text",
                    "FireBoss-Win|FireBoss-ExampleText1~FireBoss-ExampleText2~FireBoss-ExampleText3_FireBoss-Ending|FireBoss-ExampleText1~FireBoss-ExampleText2~FireBoss-ExampleText3",
                    "Separate entries with _\n" +
                    "Format entries as such\n" +
                    "EntryID|EntryDialog1~EntryDialog2~Etc").Value;

        }

        private void DeConfigger()
        {
            replacements.Add(DeConfig("FireBoss-Intro", fire));
            replacements.Add(DeConfig("AirBoss-Intro", wind));
            replacements.Add(DeConfig("EarthBoss-Intro", eart));
            replacements.Add(DeConfig("LightningBoss-Intro", ligt));
            replacements.Add(DeConfig("Iceboss-Intro", watr));

            string[] mor = more.Split('_');
            for (int i = 0; i < mor.Length; i++)
            {
                replacements.Add(DeConfig(mor[i]));
            }
        }
        private DialogReplacement DeConfig(string id, string messages)
        {
            return new DialogReplacement(id, messages.Split('~'));
        }
        private DialogReplacement DeConfig(string fire)
        {
            string[] split1 = fire.Split('|');
            if(split1.Length != 2)
            {
                Logger.LogWarning("you fucked up. separate id and messages with |");
                return null;
            }
            string[] split2 = split1[1].Split('~');
            return new DialogReplacement(split1[0], split2);
        }

        private void TestConfigger()
        {
            for (int i = 0; i < replacements.Count; i++)
            {
                string log = replacements[i].ID;
                for (int j = 0; j < replacements[i].messages.Length; j++)
                {
                    log += "\n" + replacements[i].messages[j];
                }
                Logger.LogWarning(log);
            }
        }

        private void DialogManager_InitDialogDictionary(On.DialogManager.orig_InitDialogDictionary orig, DialogManager self, string givenFilePath)
        {
            orig(self, givenFilePath);
            for (int i = 0; i < replacements.Count; i++)
            {
                if (DialogManager.dialogDict.ContainsKey(replacements[i].ID))
                {
                    DialogReplacement dialogReplacement = replacements[i];

                    DialogEntry entry = DialogManager.dialogDict[dialogReplacement.ID];
                    DialogMessage fuckingRequiredCopy = entry.messages[0];

                    DialogMessage[] newMessages = new DialogMessage[dialogReplacement.messages.Length];
                    for (int j = 0; j < dialogReplacement.messages.Length; j++)
                    {
                        DialogMessage newMessage = new DialogMessage(fuckingRequiredCopy);
                        newMessage.message = dialogReplacement.messages[j];
                    }
                    entry.messages = newMessages;
                }
            }
        }
    }
    public class DialogReplacement
    {
        public string ID;
        public string[] messages;

        public DialogReplacement(string iD, params string[] messages)
        {
            this.ID = iD;
            this.messages = messages;
        }
    }
}
