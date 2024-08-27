using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace EssentialsPlus
{
    public static class Commands
    {
        public static async void Find(CommandArgs e)
        {
            var regex = new Regex(@"^\w+ -(?<switch>\w+) (?<search>.+?) ?(?<page>\d*)$");
            Match match = regex.Match(e.Message);
            if (!match.Success)
            {
                e.Player.SendErrorMessage("¡Formato incorrecto! La sintaxis correcta es: {0}find <-switch> <nombre...> [número de página]",
                    TShock.Config.Settings.CommandSpecifier);
                e.Player.SendSuccessMessage("Switches válidos para {0}find:", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendInfoMessage("-command: Buscar comandos.");
                e.Player.SendInfoMessage("-item: Buscar objetos.");
                e.Player.SendInfoMessage("-npc: Buscar NPC.");
                e.Player.SendInfoMessage("-tile: Buscar bloques.");
                e.Player.SendInfoMessage("-wall: Buscar muros.");
                return;
            }

            int page = 1;
            if (!String.IsNullOrWhiteSpace(match.Groups["page"].Value) &&
                (!int.TryParse(match.Groups["page"].Value, out page) || page <= 0))
            {
                e.Player.SendErrorMessage("¡Número de página inválido '{0}'!", match.Groups["page"].Value);
                return;
            }

            switch (match.Groups["switch"].Value.ToLowerInvariant())
            {
                #region Command

                case "command":
                case "命令":
                    {
                        var commands = new List<string>();

                        await Task.Run(() =>
                        {
                            foreach (
                                Command command in
                                    TShockAPI.Commands.ChatCommands.FindAll(c => c.Names.Any(s => s.ContainsInsensitive(match.Groups[2].Value))))
                            {
                                commands.Add(String.Format("{0} (Permiso: {1})", command.Name, command.Permissions.FirstOrDefault()));
                            }
                        });

                        PaginationTools.SendPage(e.Player, page, commands,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Comandos encontrados ({0}/{1}):",
                                FooterFormat = String.Format("Escribe /find -command {0} {{0}} para ver más", match.Groups[2].Value),
                                NothingToDisplayString = "No se encontraron comandos."
                            });
                        return;
                    }

                #endregion

                #region Item

                case "item":
                case "物品":
                    {
                        var items = new List<string>();

                        await Task.Run(() =>
                        {
                            for (int i = -48; i < 0; i++)
                            {
                                var item = new Item();
                                item.netDefaults(i);
                                if (item.HoverName.ContainsInsensitive(match.Groups[2].Value))
                                {
                                    items.Add(String.Format("{0} (ID: {1})", item.HoverName, i));
                                }
                            }
                            for (int i = 0; i < ItemID.Count; i++)
                            {
                                if (Lang.GetItemNameValue(i).ContainsInsensitive(match.Groups[2].Value))
                                {
                                    items.Add(String.Format("{0} (ID: {1})", Lang.GetItemNameValue(i), i));
                                }
                            }
                        });

                        PaginationTools.SendPage(e.Player, page, items,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Objetos encontrados ({0}/{1}):",
                                FooterFormat = String.Format("Escribe /find -item {0} {{0}} para ver más", match.Groups[2].Value),
                                NothingToDisplayString = "No se encontraron objetos."
                            });
                        return;
                    }


                #endregion

                #region NPC

                case "npc":
                case "NPC":
                    {
                        var npcs = new List<string>();

                        await Task.Run(() =>
                        {
                            for (int i = -65; i < 0; i++)
                            {
                                var npc = new NPC();
                                npc.SetDefaults(i);
                                if (npc.FullName.ContainsInsensitive(match.Groups[2].Value))
                                {
                                    npcs.Add(String.Format("{0} (ID: {1})", npc.FullName, i));
                                }
                            }
                            for (int i = 0; i < NPCID.Count; i++)
                            {
                                if (Lang.GetNPCNameValue(i).ContainsInsensitive(match.Groups[2].Value))
                                {
                                    npcs.Add(String.Format("{0} (ID: {1})", Lang.GetNPCNameValue(i), i));
                                }
                            }
                        });

                        PaginationTools.SendPage(e.Player, page, npcs,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "NPCs encontrados ({0}/{1}):",
                                FooterFormat = String.Format("Escribe /find -npc {0} {{0}} para ver más", match.Groups[2].Value),
                                NothingToDisplayString = "No se encontraron NPCs.",
                            });
                        return;
                    }


                #endregion

                #region Tile

                case "tile":
                case "方块":
                    {
                        var tiles = new List<string>();

                        await Task.Run(() =>
                        {
                            foreach (FieldInfo fi in typeof(TileID).GetFields())
                            {
                                var sb = new StringBuilder();
                                for (int i = 0; i < fi.Name.Length; i++)
                                {
                                    if (Char.IsUpper(fi.Name[i]) && i > 0)
                                    {
                                        sb.Append(" ").Append(fi.Name[i]);
                                    }
                                    else
                                    {
                                        sb.Append(fi.Name[i]);
                                    }
                                }

                                string name = sb.ToString();
                                if (name.ContainsInsensitive(match.Groups[2].Value))
                                {
                                    tiles.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                                }
                            }
                        });

                        PaginationTools.SendPage(e.Player, page, tiles,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Bloques encontrados ({0}/{1}):",
                                FooterFormat = String.Format("Escribe /find -tile {0} {{0}} para ver más", match.Groups[2].Value),
                                NothingToDisplayString = "No se encontraron bloques.",
                            });
                        return;
                    }

                #endregion

                #region Wall

                case "wall":
                case "墙壁":
                    {
                        var walls = new List<string>();

                        await Task.Run(() =>
                        {
                            foreach (FieldInfo fi in typeof(WallID).GetFields())
                            {
                                var sb = new StringBuilder();
                                for (int i = 0; i < fi.Name.Length; i++)
                                {
                                    if (Char.IsUpper(fi.Name[i]) && i > 0)
                                    {
                                        sb.Append(" ").Append(fi.Name[i]);
                                    }
                                    else
                                    {
                                        sb.Append(fi.Name[i]);
                                    }
                                }

                                string name = sb.ToString();
                                if (name.ContainsInsensitive(match.Groups[2].Value))
                                {
                                    walls.Add(String.Format("{0} (ID: {1})", name, fi.GetValue(null)));
                                }
                            }
                        });

                        PaginationTools.SendPage(e.Player, page, walls,
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Muros encontrados ({0}/{1}):",
                                FooterFormat = String.Format("Escribe /find -wall {0} {{0}} para ver más", match.Groups[2].Value),
                                NothingToDisplayString = "No se encontraron muros.",
                            });
                        return;
                    }

                #endregion

                default:
                    {
                        e.Player.SendSuccessMessage("Switches válidos para {0}find:", TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendInfoMessage("-command: Buscar comandos.");
                        e.Player.SendInfoMessage("-item: Buscar objetos.");
                        e.Player.SendInfoMessage("-npc: Buscar NPC.");
                        e.Player.SendInfoMessage("-tile: Buscar bloques.");
                        e.Player.SendInfoMessage("-wall: Buscar muros.");
                        return;
                    }
            }
        }

        public static System.Timers.Timer FreezeTimer = new System.Timers.Timer(1000);

        public static void FreezeTime(CommandArgs e)
        {
            if (FreezeTimer.Enabled)
            {
                FreezeTimer.Stop();
                TSPlayer.All.SendInfoMessage("{0} ha deshecho la congelación del tiempo.", e.Player.Name);
            }
            else
            {
                bool dayTime = Main.dayTime;
                double time = Main.time;

                FreezeTimer.Dispose();
                FreezeTimer = new System.Timers.Timer(1000);
                FreezeTimer.Elapsed += (o, ee) =>
                {
                    Main.dayTime = dayTime;
                    Main.time = time;
                    TSPlayer.All.SendData(PacketTypes.TimeSet);
                };
                FreezeTimer.Start();
                TSPlayer.All.SendInfoMessage("{0} ha congelado el tiempo.", e.Player.Name);
            }
        }

        public static async void DeleteHome(CommandArgs e)
       {
    if (e.Parameters.Count > 1)
    {
        e.Player.SendErrorMessage("¡Error de sintaxis! La sintaxis correcta es: {0}delhome <nombre del hogar>", TShock.Config.Settings.CommandSpecifier);
        return;
    }

    string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
    Home home = await EssentialsPlus.Homes.GetAsync(e.Player, homeName);
    if (home != null)
    {
        if (await EssentialsPlus.Homes.DeleteAsync(e.Player, homeName))
        {
            e.Player.SendSuccessMessage("Hogar '{0}' eliminado con éxito.", homeName);
        }
        else
        {
            e.Player.SendErrorMessage("No se puede eliminar el hogar, revisa los registros para obtener más detalles.");
        }
    }
    else
    {
        e.Player.SendErrorMessage("Hogar inválido '{0}'!", homeName);
    }
}

public static async void MyHome(CommandArgs e)
{
    if (e.Parameters.Count > 1)
    {
        e.Player.SendErrorMessage("¡Error de sintaxis! La sintaxis correcta es: {0}myhome <nombre del hogar>", TShock.Config.Settings.CommandSpecifier);
        return;
    }

    if (Regex.Match(e.Message, @"^\w+ -l(?:ist)?$").Success)
    {
        List<Home> homes = await EssentialsPlus.Homes.GetAllAsync(e.Player);
        e.Player.SendInfoMessage(homes.Count == 0 ? "No has establecido ningún hogar." : "Lista de hogares: {0}", string.Join(", ", homes.Select(h => h.Name)));
    }
    else
    {
        string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
        Home home = await EssentialsPlus.Homes.GetAsync(e.Player, homeName);
        if (home != null)
        {
            e.Player.Teleport(home.X, home.Y);
            e.Player.SendSuccessMessage("Te has teletransportado a tu hogar '{0}'.", homeName);
        }
        else
        {
            e.Player.SendErrorMessage("Hogar inválido '{0}'!", homeName);
        }
    }
}

public static async void SetHome(CommandArgs e)
{
    if (e.Parameters.Count > 1)
    {
        e.Player.SendErrorMessage("¡Error de sintaxis! La sintaxis correcta es: {0}sethome <nombre del hogar>", TShock.Config.Settings.CommandSpecifier);
        return;
    }

            string homeName = e.Parameters.Count == 1 ? e.Parameters[0] : "home";
            if (await EssentialsPlus.Homes.GetAsync(e.Player, homeName) != null)
            {
                if (await EssentialsPlus.Homes.UpdateAsync(e.Player, homeName, e.Player.X, e.Player.Y))
                {
                    e.Player.SendSuccessMessage("Se ha actualizado tu hogar '{0}'.", homeName);
                }
                else
                {
                    e.Player.SendErrorMessage("No se puede actualizar el hogar, revisa los registros para obtener más detalles.");
                }
                return;
            }

            if ((await EssentialsPlus.Homes.GetAllAsync(e.Player)).Count >= e.Player.Group.GetDynamicPermission(Permissions.HomeSet))
            {
                e.Player.SendErrorMessage("¡Has alcanzado el límite de hogares configurados!");
                return;
            }

            if (await EssentialsPlus.Homes.AddAsync(e.Player, homeName, e.Player.X, e.Player.Y))
            {
                e.Player.SendSuccessMessage("Se ha configurado tu hogar '{0}'.", homeName);
            }
            else
            {
                e.Player.SendErrorMessage("No se puede configurar el hogar, revisa los registros para obtener más detalles.");
            }
        }


        public static async void KickAll(CommandArgs e)
        {
            var regex = new Regex(@"^\w+(?: -(\w+))* ?(.*)$");
            Match match = regex.Match(e.Message);
            bool noSave = false;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                switch (capture.Value.ToLowerInvariant())
                {
                    case "nosave":
                        noSave = true;
                        continue;
                    default:
                        e.Player.SendSuccessMessage("Opciones válidas para {0}kickall:", TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendInfoMessage("-nosave: No guardar datos de SSC al expulsar.");
                        return;
                }
            }

            int kickLevel = e.Player.Group.GetDynamicPermission(Permissions.KickAll);
            string reason = String.IsNullOrWhiteSpace(match.Groups[2].Value) ? "Sin motivo." : match.Groups[2].Value;
            await Task.WhenAll(TShock.Players.Where(p => p != null && p.Group.GetDynamicPermission(Permissions.KickAll) < kickLevel).Select(p => Task.Run(() =>
            {
                if (!noSave && p.IsLoggedIn)
                {
                    p.SaveServerCharacter();
                }
                p.Disconnect("Motivo de expulsión: " + reason);
            })));
            e.Player.SendSuccessMessage("Se ha expulsado a todos con éxito. Motivo: '{0}'.", reason);
        }


        public static async void RepeatLast(CommandArgs e)
        {
            string lastCommand = e.Player.GetPlayerInfo().LastCommand;
            if (String.IsNullOrWhiteSpace(lastCommand))
            {
                e.Player.SendErrorMessage("¡No tienes el último comando!");
                return;
            }

            e.Player.SendSuccessMessage("¡Se ha repetido el último comando '{0}{1}'!", TShock.Config.Settings.CommandSpecifier, lastCommand);
            await Task.Run(() => TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.Settings.CommandSpecifier + lastCommand));
        }

        public static async void More(CommandArgs e)
        {
            await Task.Run(() =>
            {
                if (e.Parameters.Count > 0 && e.Parameters[0].ToLower() == "all")
                {
                    bool full = true;
                    foreach (Item item in e.TPlayer.inventory)
                    {
                        if (item == null || item.stack == 0) continue;
                        int amtToAdd = item.maxStack - item.stack;
                        if (amtToAdd > 0 && item.stack > 0 && !item.Name.ToLower().Contains("coin"))
                        {
                            full = false;
                            e.Player.GiveItem(item.type, amtToAdd);
                        }
                    }
                    if (!full)
                        e.Player.SendSuccessMessage("Se ha llenado tu inventario.");
                    else
                        e.Player.SendErrorMessage("Tu inventario ya está lleno.");
                }
                else
                {
                    Item item = e.Player.TPlayer.inventory[e.TPlayer.selectedItem];
                    int amtToAdd = item.maxStack - item.stack;
                    if (amtToAdd == 0)
                        e.Player.SendErrorMessage("Tu {0} ya está lleno.", item.Name);
                    else if (amtToAdd > 0 && item.stack > 0)
                        e.Player.GiveItem(item.type, amtToAdd);
                    e.Player.SendSuccessMessage("Se ha aumentado la cantidad de {0} en tu inventario.", item.Name);
                }
            });
        }

        public static async void Mute(CommandArgs e)
        {
            string subCmd = e.Parameters.FirstOrDefault() ?? "help";
            switch (subCmd.ToLowerInvariant())
            {
                #region Añadir mute

                case "add":
                    {
                        var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]+?))(?: (.+))?$");
                        Match match = regex.Match(e.Message);
                        if (!match.Success)
                        {
                            e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: /mute add <nombre> [tiempo]");
                            return;
                        }

                        int seconds = Int32.MaxValue / 1000;
                        if (!String.IsNullOrWhiteSpace(match.Groups[3].Value) &&
                            (!TShock.Utils.TryParseTime(match.Groups[3].Value, out seconds) || seconds <= 0 ||
                             seconds > Int32.MaxValue / 1000))
                        {
                            e.Player.SendErrorMessage("Tiempo inválido '{0}'!", match.Groups[3].Value);
                            return;
                        }

                        string playerName = String.IsNullOrWhiteSpace(match.Groups[2].Value)
                            ? match.Groups[1].Value
                            : match.Groups[2].Value;
                        List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
                        if (players.Count == 0)
                        {
                            UserAccount user = TShock.UserAccounts.GetUserAccountByName(playerName);
                            if (user == null)
                                e.Player.SendErrorMessage("Jugador o cuenta inválido '{0}'!", playerName);
                            else
                            {
                                if (TShock.Groups.GetGroupByName(user.Group).GetDynamicPermission(Permissions.Mute) >=
                                    e.Player.Group.GetDynamicPermission(Permissions.Mute))
                                {
                                    e.Player.SendErrorMessage("¡No puedes silenciar a {0}!", user.Name);
                                    return;
                                }

                                if (await EssentialsPlus.Mutes.AddAsync(user, DateTime.UtcNow.AddSeconds(seconds)))
                                {
                                    TSPlayer.All.SendInfoMessage("{0} ha silenciado a {1}.", e.Player.Name, user.Name);
                                }
                                else
                                {
                                    e.Player.SendErrorMessage("No se puede silenciar, revisa los registros para obtener más información.");
                                }
                            }
                        }
                        else if (players.Count > 1)
                        {
                            e.Player.SendErrorMessage("Se han encontrado múltiples jugadores: {0}", String.Join(", ", players.Select(p => p.Name)));
                        }
                        else
                        {
                            if (players[0].Group.GetDynamicPermission(Permissions.Mute) >=
                                e.Player.Group.GetDynamicPermission(Permissions.Mute))
                            {
                                e.Player.SendErrorMessage("¡No puedes silenciar a {0}!", players[0].Name);
                                return;
                            }

                            if (await EssentialsPlus.Mutes.AddAsync(players[0], DateTime.UtcNow.AddSeconds(seconds)))
                            {
                                TSPlayer.All.SendInfoMessage("{0} ha silenciado a {1}.", e.Player.Name, players[0].Name);

                                players[0].mute = true;
                                try
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(seconds), players[0].GetPlayerInfo().MuteToken);
                                    players[0].mute = false;
                                    players[0].SendInfoMessage("Se ha levantado el silencio.");
                                }
                                catch (TaskCanceledException)
                                {
                                }
                            }
                            else
                                e.Player.SendErrorMessage("No se puede silenciar, revisa los registros para obtener más información.");
                        }
                    }
                    return;

                #endregion

                #region Eliminar mute

                case "del":
                case "delete":
                    {
                        var regex = new Regex(@"^\w+ \w+ (?:""(.+?)""|([^\s]*?))$");
                        Match match = regex.Match(e.Message);
                        if (!match.Success)
                        {
                            e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: /mute del <nombre>");
                            return;
                        }

                        string playerName = String.IsNullOrWhiteSpace(match.Groups[2].Value)
                            ? match.Groups[1].Value
                            : match.Groups[2].Value;
                        List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
                        if (players.Count == 0)
                        {
                            UserAccount user = TShock.UserAccounts.GetUserAccountByName(playerName);
                            if (user == null)
                                e.Player.SendErrorMessage("Jugador o cuenta inválido '{0}'!", playerName);
                            else
                            {
                                if (await EssentialsPlus.Mutes.DeleteAsync(user))
                                    TSPlayer.All.SendInfoMessage("{0} ha levantado el silencio a {1}.", e.Player.Name, user.Name);
                                else
                                    e.Player.SendErrorMessage("No se puede levantar el silencio, revisa los registros para obtener más información.");
                            }
                        }
                        else if (players.Count > 1)
                            e.Player.SendErrorMessage("Se han encontrado múltiples jugadores: {0}", String.Join(", ", players.Select(p => p.Name)));
                        else
                        {
                            if (await EssentialsPlus.Mutes.DeleteAsync(players[0]))
                            {
                                players[0].mute = false;
                                TSPlayer.All.SendInfoMessage("{0} ha levantado el silencio a {1}.", e.Player.Name, players[0].Name);
                            }
                            else
                                e.Player.SendErrorMessage("No se puede levantar el silencio, revisa los registros para obtener más información.");
                        }
                    }
                    return;

                #endregion

                #region Ayuda

                default:
                    e.Player.SendSuccessMessage("Subcomandos de mute:");
                    e.Player.SendInfoMessage("add <nombre> [tiempo] - Silenciar jugador o cuenta.");
                    e.Player.SendInfoMessage("del <nombre> - Levantar silencio a jugador o cuenta.");
                    return;

                    #endregion
            }
        }


        public static void PvP(CommandArgs e)
        {
            e.TPlayer.hostile = !e.TPlayer.hostile;
            string hostile = Language.GetTextValue(e.TPlayer.hostile ? "LegacyMultiplayer.11" : "LegacyMultiplayer.12", e.Player.Name);
            TSPlayer.All.SendData(PacketTypes.TogglePvp, "", e.Player.Index);
            TSPlayer.All.SendMessage(hostile, Main.teamColor[e.Player.Team]);
        }

        public static void Ruler(CommandArgs e)
        {
            if (e.Parameters.Count == 0)
            {
                if (e.Player.TempPoints.Any(p => p == Point.Zero))
                {
                    e.Player.SendErrorMessage("¡Puntos de regla no configurados!");
                    return;
                }

                Point p1 = e.Player.TempPoints[0];
                Point p2 = e.Player.TempPoints[1];
                int x = Math.Abs(p1.X - p2.X);
                int y = Math.Abs(p1.Y - p2.Y);
                double cartesian = Math.Sqrt(x * x + y * y);
                e.Player.SendInfoMessage("Distancia: Eje X: {0}, Eje Y: {1}, Distancia recta: {2:N3}", x, y, cartesian);
            }
            else if (e.Parameters.Count == 1)
            {
                if (e.Parameters[0] == "1")
                {
                    e.Player.AwaitingTempPoint = 1;
                    e.Player.SendInfoMessage("Modifica un bloque para establecer el primer punto de la regla.");
                }
                else if (e.Parameters[0] == "2")
                {
                    e.Player.AwaitingTempPoint = 2;
                    e.Player.SendInfoMessage("Modifica un bloque para establecer el segundo punto de la regla.");
                }
                else
                    e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}ruler [1/2]", TShock.Config.Settings.CommandSpecifier);
            }
            else
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}ruler [1/2]", TShock.Config.Settings.CommandSpecifier);
        }


        public static void Send(CommandArgs e)
        {
            var regex = new Regex(@"^\w+(?: (\d+),(\d+),(\d+))? (.+)$");
            Match match = regex.Match(e.Message);
            if (!match.Success)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}send [r,g,b] <texto...>", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            byte r = e.Player.Group.R;
            byte g = e.Player.Group.G;
            byte b = e.Player.Group.B;
            if (!String.IsNullOrWhiteSpace(match.Groups[1].Value) && !String.IsNullOrWhiteSpace(match.Groups[2].Value) && !String.IsNullOrWhiteSpace(match.Groups[3].Value) &&
                (!byte.TryParse(match.Groups[1].Value, out r) || !byte.TryParse(match.Groups[2].Value, out g) || !byte.TryParse(match.Groups[3].Value, out b)))
            {
                e.Player.SendErrorMessage("¡Color inválido!");
                return;
            }
            TSPlayer.All.SendMessage(match.Groups[4].Value, new Color(r, g, b));
        }


        public static async void Sudo(CommandArgs e)
        {
            var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (?:""(.+?)""|([^\s]*?)) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
            Match match = regex.Match(e.Message);
            if (!match.Success)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}sudo [-switches...] <jugador> <comando...>", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendSuccessMessage("Switches válidos para {0}sudo:", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendInfoMessage("-f, -force: Forzar sudo, ignorar restricciones de permisos.");
                return;
            }

            bool force = false;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                switch (capture.Value.ToLowerInvariant())
                {
                    case "f":
                    case "force":
                        if (!e.Player.Group.HasPermission(Permissions.SudoForce))
                        {
                            e.Player.SendErrorMessage("¡No tiene permiso para el switch '-{0}'!", capture.Value);
                            return;
                        }
                        force = true;
                        continue;
                    default:
                        e.Player.SendSuccessMessage("Switches válidos para {0}sudo:", TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendInfoMessage("-f, -force: Forzar sudo, ignorar restricciones de permisos.");
                        return;
                }
            }

            string playerName = String.IsNullOrWhiteSpace(match.Groups[3].Value) ? match.Groups[2].Value : match.Groups[3].Value;
            string command = match.Groups[4].Value;

            List<TSPlayer> players = TShock.Players.FindPlayers(playerName);
            if (players.Count == 0)
                e.Player.SendErrorMessage("¡Jugador inválido '{0}'!", playerName);
            else if (players.Count > 1)
                e.Player.SendErrorMessage("Se encontraron varios jugadores: {0}", String.Join(", ", players.Select(p => p.Name)));
            else
            {
                if ((e.Player.Group.GetDynamicPermission(Permissions.Sudo) <= players[0].Group.GetDynamicPermission(Permissions.Sudo))
                    && !e.Player.Group.HasPermission(Permissions.SudoSuper))
                {
                    e.Player.SendErrorMessage("¡No puede forzar a {0} a ejecutar {1}{2}!", players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
                    return;
                }

                e.Player.SendSuccessMessage("强制 {0} 执行 {1}{2}。", players[0].Name, TShock.Config.Settings.CommandSpecifier, command);
                if (!e.Player.Group.HasPermission(Permissions.SudoInvisible))
                    players[0].SendInfoMessage("{0} 强制您执行 {1}{2}。", e.Player.Name, TShock.Config.Settings.CommandSpecifier, command);

                var fakePlayer = new TSPlayer(players[0].Index)
                {
                    AwaitingName = players[0].AwaitingName,
                    AwaitingNameParameters = players[0].AwaitingNameParameters,
                    AwaitingTempPoint = players[0].AwaitingTempPoint,
                    Group = force ? new SuperAdminGroup() : players[0].Group,
                    TempPoints = players[0].TempPoints
                };
                await Task.Run(() => TShockAPI.Commands.HandleCommand(fakePlayer, TShock.Config.Settings.CommandSpecifier + command));

                players[0].AwaitingName = fakePlayer.AwaitingName;
                players[0].AwaitingNameParameters = fakePlayer.AwaitingNameParameters;
                players[0].AwaitingTempPoint = fakePlayer.AwaitingTempPoint;
                players[0].TempPoints = fakePlayer.TempPoints;
            }
        }


        public static async void TimeCmd(CommandArgs e)
        {
            var regex = new Regex(String.Format(@"^\w+(?: -(\w+))* (\w+) (?:{0})?(.+)$", TShock.Config.Settings.CommandSpecifier));
            Match match = regex.Match(e.Message);
            if (!match.Success)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}timecmd [-switches...] <tiempo> <comando...>", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendSuccessMessage("Switches válidos para {0}timecmd:", TShock.Config.Settings.CommandSpecifier);
                e.Player.SendInfoMessage("-r, -repeat: Repetir la ejecución del comando de tiempo.");
                return;
            }

            bool repeat = false;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                switch (capture.Value.ToLowerInvariant())
                {
                    case "r":
                    case "repeat":
                        repeat = true;
                        break;
                    default:
                        e.Player.SendSuccessMessage("Switches válidos para {0}timecmd:", TShock.Config.Settings.CommandSpecifier);
                        e.Player.SendInfoMessage("-r, -repeat: Repetir la ejecución del comando de tiempo.");
                        return;
                }
            }

            if (!TShock.Utils.TryParseTime(match.Groups[2].Value, out int seconds) || seconds <= 0 || seconds > Int32.MaxValue / 1000)
            {
                e.Player.SendErrorMessage("¡Tiempo inválido '{0}'!", match.Groups[2].Value);
                return;
            }

            if (repeat)
                e.Player.SendSuccessMessage("Comando '{0}{1}' en la cola para ejecución. ¡Usa /cancelar para cancelar!", TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
            else
                e.Player.SendSuccessMessage("Comando '{0}{1}' en la cola para ejecución. ¡Usa /cancelar para cancelar!", TShock.Config.Settings.CommandSpecifier, match.Groups[3].Value);
            e.Player.AddResponse("cancel", o =>
            {
                e.Player.GetPlayerInfo().CancelTimeCmd();
                e.Player.SendSuccessMessage("¡Todos los comandos de tiempo cancelados!");
            });

            CancellationToken token = e.Player.GetPlayerInfo().TimeCmdToken;
            try
            {
                await Task.Run(async () =>
                {
                    do
                    {
                        await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                        TShockAPI.Commands.HandleCommand(e.Player, TShock.Config.Settings.CommandSpecifier + match.Groups[3].Value);
                    }
                    while (repeat);
                }, token);
            }
            catch (TaskCanceledException)
            {
            }
        }


        public static void Back(CommandArgs e)
        {
            if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}eback [número de pasos]", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            int steps = 1;
            if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out steps) || steps <= 0))
            {
                e.Player.SendErrorMessage("¡Número de pasos inválido '{0}'!", e.Parameters[0]);
                return;
            }

            PlayerInfo info = e.Player.GetPlayerInfo();
            if (info.BackHistoryCount == 0)
            {
                e.Player.SendErrorMessage("¡No se puede teletransportar al último lugar!");
                return;
            }

            steps = Math.Min(steps, info.BackHistoryCount);
            e.Player.SendSuccessMessage("Teletransportado {0} pasos.", steps);
            Vector2 vector = info.PopBackHistory(steps);
            e.Player.Teleport(vector.X, vector.Y);
        }

        public static async void Down(CommandArgs e)
        {
            if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}down [niveles]", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            int levels = 1;
            if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
            {
                e.Player.SendErrorMessage("¡Número de niveles inválido '{0}'!", levels);
                return;
            }

            int currentLevel = 0;
            bool empty = false;
            int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
            int y = Math.Max(0, Math.Min(e.Player.TileY + 3, Main.maxTilesY - 3));

            await Task.Run(() =>
            {
                for (int j = y; currentLevel < levels && j < Main.maxTilesY - 2; j++)
                {
                    if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
                        Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
                        Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
                    {
                        empty = true;
                    }
                    else if (empty)
                    {
                        empty = false;
                        currentLevel++;
                        y = j;
                    }
                }
            });

            if (currentLevel == 0)
                e.Player.SendErrorMessage("¡No se puede teletransportar hacia abajo!");
            else
            {
                if (e.Player.Group.HasPermission(Permissions.TpBack))
                    e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
                e.Player.Teleport(16 * x, 16 * y - 10);
                e.Player.SendSuccessMessage("Teletransportado {0} niveles hacia abajo.", currentLevel);
            }
        }

        public static async void Left(CommandArgs e)
        {
            if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}left [niveles]", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            int levels = 1;
            if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
            {
                e.Player.SendErrorMessage("¡Número de niveles inválido '{0}'!", levels);
                return;
            }

            int currentLevel = 0;
            bool solid = false;
            int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
            int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

            await Task.Run(() =>
            {
                for (int i = x; currentLevel < levels && i >= 0; i--)
                {
                    if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
                        Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
                        Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
                    {
                        if (solid)
                        {
                            solid = false;
                            currentLevel++;
                            x = i;
                        }
                    }
                    else
                        solid = true;
                }
            });

            if (currentLevel == 0)
                e.Player.SendErrorMessage("¡No se puede teletransportar hacia la izquierda!");
            else
            {
                if (e.Player.Group.HasPermission(Permissions.TpBack))
                    e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
                e.Player.Teleport(16 * x + 12, 16 * y);
                e.Player.SendSuccessMessage("Teletransportado {0} niveles hacia la izquierda.", currentLevel);
            }
        }

        public static async void Right(CommandArgs e)
        {
            if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}right [niveles]", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            int levels = 1;
            if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
            {
                e.Player.SendErrorMessage("¡Número de niveles inválido '{0}'!", levels);
                return;
            }

            int currentLevel = 0;
            bool solid = false;
            int x = Math.Max(0, Math.Min(e.Player.TileX + 1, Main.maxTilesX - 2));
            int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

            await Task.Run(() =>
            {
                for (int i = x; currentLevel < levels && i < Main.maxTilesX - 1; i++)
                {
                    if (Main.tile[i, y].IsEmpty() && Main.tile[i + 1, y].IsEmpty() &&
                        Main.tile[i, y + 1].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
                        Main.tile[i, y + 2].IsEmpty() && Main.tile[i + 1, y + 2].IsEmpty())
                    {
                        if (solid)
                        {
                            solid = false;
                            currentLevel++;
                            x = i;
                        }
                    }
                    else
                        solid = true;
                }
            });

            if (currentLevel == 0)
                e.Player.SendErrorMessage("¡No se puede teletransportar a la derecha!");
            else
            {
                if (e.Player.Group.HasPermission(Permissions.TpBack))
                    e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
                e.Player.Teleport(16 * x, 16 * y);
                e.Player.SendSuccessMessage("Teletransportado a la derecha {0} niveles.", currentLevel);
            }
        }

        public static async void Up(CommandArgs e)
        {
            if (e.Parameters.Count > 1)
            {
                e.Player.SendErrorMessage("¡Sintaxis inválida! La sintaxis correcta es: {0}up [niveles]", TShock.Config.Settings.CommandSpecifier);
                return;
            }

            int levels = 1;
            if (e.Parameters.Count > 0 && (!int.TryParse(e.Parameters[0], out levels) || levels <= 0))
            {
                e.Player.SendErrorMessage("¡Número de niveles inválido '{0}'!", levels);
                return;
            }

            int currentLevel = 0;
            bool solid = false;
            int x = Math.Max(0, Math.Min(e.Player.TileX, Main.maxTilesX - 2));
            int y = Math.Max(0, Math.Min(e.Player.TileY, Main.maxTilesY - 3));

            await Task.Run(() =>
            {
                for (int j = y; currentLevel < levels && j >= 0; j--)
                {
                    if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
                        Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
                        Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
                    {
                        if (solid)
                        {
                            solid = false;
                            currentLevel++;
                            y = j;
                        }
                    }
                    else
                        solid = true;
                }
            });

            if (currentLevel == 0)
                e.Player.SendErrorMessage("¡No se puede teletransportar hacia arriba!");
            else
            {
                if (e.Player.Group.HasPermission(Permissions.TpBack))
                    e.Player.GetPlayerInfo().PushBackHistory(e.TPlayer.position);
                e.Player.Teleport(16 * x, 16 * y + 6);
                e.Player.SendSuccessMessage("Teletransportado {0} niveles hacia arriba.", currentLevel);
            }
        }

    }
}
