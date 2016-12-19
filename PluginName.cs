﻿using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LjMjollnir
{
    public class MessageTargets
    {
        public ulong MessageTo;
        public ulong ReplyTo;
        public MessageTargets()
        {
            MessageTo = 0;
            ReplyTo = 0;
        }
    }

    public class PluginName : RocketPlugin<PluginConfig>
    {
        Dictionary<ulong, MessageTargets> PlayerList;
        /// Run after Plugin Load
        protected override void Load()
        {
            Logger.Log("Plugin Loaded");
            PlayerList = new Dictionary<ulong, MessageTargets>();
        }
        /// Run on Plugin Shutdown
        protected override void Unload()
        {
            Logger.Log("Awwww but i was having fun... Bye!!!");
            base.Unload();
        }

        // (Command, Help string, Syntax, Allowed Caller Console/Both/Player

        [RocketCommand("w", "Whisper a player", "/w <name> <message>", AllowedCaller.Player)]
        public void ExecuteWhisper(IRocketPlayer caller, string[] para)
        {
            String Message = "";
            UnturnedPlayer plyr = (UnturnedPlayer)caller;
            if (para.Length == 0) { Logger.Log("Whisper Para too small"); return; } // Display Help
            UnturnedPlayer Target = UnturnedPlayer.FromName(para[0]);
            if (Target == null)
            {
                if (PlayerList.ContainsKey(plyr.CSteamID.m_SteamID))
                {
                    Target = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(PlayerList[plyr.CSteamID.m_SteamID].MessageTo));
                    if (Target == null)
                        Target = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(PlayerList[plyr.CSteamID.m_SteamID].ReplyTo));
                }
                if (Target == null) { Logger.Log("No Target"); return; }// Display Help
                foreach (var s in para)
                {
                    Message = Message + s;
                }
            }
            else
            {
                int i = 0;
                foreach (var s in para)
                {
                    i++;
                    if (i == 1) continue;
                    Message = Message + s;
                }
            }
            UnturnedChat.Say(Target, String.Format("From:{0} {1}", plyr.CharacterName, Message), UnityEngine.Color.magenta);
            UnturnedChat.Say(plyr, String.Format("To:{0} {1}", Target.DisplayName, Message), UnityEngine.Color.magenta);

            SetPlayerData(plyr, Target);
        }

        private void SetPlayerData(UnturnedPlayer plyr, UnturnedPlayer Target)
        {
            if (!PlayerList.ContainsKey(Target.CSteamID.m_SteamID))
                PlayerList[Target.CSteamID.m_SteamID] = new MessageTargets();
            if (!PlayerList.ContainsKey(plyr.CSteamID.m_SteamID))
                PlayerList[plyr.CSteamID.m_SteamID] = new MessageTargets();
            PlayerList[Target.CSteamID.m_SteamID].ReplyTo = plyr.CSteamID.m_SteamID;
            PlayerList[plyr.CSteamID.m_SteamID].MessageTo = Target.CSteamID.m_SteamID;
        }
        [RocketCommand("r", "Replys to a player", "/r <message>", AllowedCaller.Player)]
        public void ExecuteReply(IRocketPlayer caller, string[] para)
        {
            UnturnedPlayer plyr = (UnturnedPlayer)caller;
            UnturnedPlayer Target = null;
            String Message = "";
            if (!PlayerList.ContainsKey(plyr.CSteamID.m_SteamID)) { Logger.Log("Reply No Record availble"); return; } // No record available.. (no Reply or Message Target) Display Help
            if (para.Length < 1) { Logger.Log("Reply Para too small"); return; } // Display Help
            if (PlayerList[plyr.CSteamID.m_SteamID].MessageTo != 0)
                Target = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(PlayerList[plyr.CSteamID.m_SteamID].MessageTo));
            if (PlayerList[plyr.CSteamID.m_SteamID].ReplyTo != 0)
                Target = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(PlayerList[plyr.CSteamID.m_SteamID].ReplyTo));
            if (Target == null) { Logger.Log("Reply Unexpected Error"); return; }// Some unexpected Error
            foreach (var s in para)
            {
                Message = Message + s;
            }
            UnturnedChat.Say(Target, String.Format("From:{0} {1}", plyr.DisplayName, Message), UnityEngine.Color.magenta);
            UnturnedChat.Say(plyr, String.Format("To:{0} {1}", Target.DisplayName, Message), UnityEngine.Color.magenta);
            SetPlayerData(plyr, Target);
        }
    }
}
