using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace EssentialsPlus
{
    [ApiVersion(2, 1)]
    public class EssentialsPlus : TerrariaPlugin
    {
        public static Config Config { get; private set; }
        public static IDbConnection Db { get; private set; }
        public static HomeManager Homes { get; private set; }
        public static MuteManager Mutes { get; private set; }

        public override string Author => "FrankV22, WhiteX等人，肝帝熙恩翻译"; // "WhiteX and others, translated by the devoted Xie En"

        public override string Description => "Essentials Commands"; // "Enhanced Essentials"

        public override string Name => "EssentialsPlus";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public EssentialsPlus(Main game)
            : base(game)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
                PlayerHooks.PlayerCommand -= OnPlayerCommand;

                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            PlayerHooks.PlayerCommand += OnPlayerCommand;

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }

        private async void OnReload(ReloadEventArgs e)
        {
            string path = Path.Combine(TShock.SavePath, "essentials.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }
            await Homes.ReloadAsync();
            e.Player.SendSuccessMessage("[EssentialsPlus] Configuración y casas recargadas!");
        }

        private List<string> teleportCommands = new List<string>
        {
            "tp", "tppos", "tpnpc", "warp", "spawn", "home"
        };

        private void OnPlayerCommand(PlayerCommandEventArgs e)
        {
            if (e.Handled || e.Player == null)
            {
                return;
            }

            Command command = e.CommandList.FirstOrDefault();
            if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => e.Player.Group.HasPermission(s))))
            {
                return;
            }

            if (e.Player.TPlayer.hostile &&
                command.Names.Select(s => s.ToLowerInvariant())
                    .Intersect(Config.DisabledCommandsInPvp.Select(s => s.ToLowerInvariant()))
                    .Any())
            {
                e.Player.SendErrorMessage("¡Este comando no se puede usar en PvP!");
                e.Handled = true;
                return;
            }

            if (e.Player.Group.HasPermission(Permissions.LastCommand) && command.CommandDelegate != Commands.RepeatLast)
            {
                e.Player.GetPlayerInfo().LastCommand = e.CommandText;
            }

            if (teleportCommands.Contains(e.CommandName) && e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.Player.TPlayer.position);
            }
        }

        private void OnInitialize(EventArgs e)
        {
            #region Config

            string path = Path.Combine(TShock.SavePath, "essentials.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }

            #endregion

            #region Database

            if (TShock.Config.Settings.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(Config.MySqlHost) ||
                    string.IsNullOrWhiteSpace(Config.MySqlDbName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Essentials+] MySQL está habilitado, pero la configuración de MySQL para Essentials+ no está establecida.");
                    Console.WriteLine("[Essentials+] Por favor, configura tu información del servidor MySQL en essentials.json y reinicia el servidor.");
                    Console.WriteLine("[Essentials+] Este plugin ahora se desactivará...");
                    Console.ResetColor();

                    GeneralHooks.ReloadEvent -= OnReload;
                    PlayerHooks.PlayerCommand -= OnPlayerCommand;

                    ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                    ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                    ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                    ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);

                    return;
                }

                string[] host = Config.MySqlHost.Split(':');
                Db = new MySqlConnection
                {
                    ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                        host[0],
                        host.Length == 1 ? "3306" : host[1],
                        Config.MySqlDbName,
                        Config.MySqlUsername,
                        Config.MySqlPassword)
                };
            }
            else if (TShock.Config.Settings.StorageType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
            {
                Db = new SqliteConnection(
                    "Data Source=" + Path.Combine(TShock.SavePath, "essentials.sqlite"));
            }
            else
            {
                throw new InvalidOperationException("¡Tipo de almacenamiento no válido!");
            }

            Mutes = new MuteManager(Db);

            #endregion

            #region Commands

            // Permitir sobrescribir comandos ya creados.
            Action<Command> Add = c =>
            {
                // Encontrar cualquier comando que coincida con el nombre o alias del nuevo comando y eliminarlo.
                TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Exists(s2 => c.Names.Contains(s2)));
                // Luego añadir el nuevo comando.
                TShockAPI.Commands.ChatCommands.Add(c);
            };

            Add(new Command(Permissions.Find, Commands.Find, "find", "Buscar")
            {
                HelpText = "Busca items y/o NPCs con el nombre especificado."
            });

            Add(new Command(Permissions.FreezeTime, Commands.FreezeTime, "freezetime", "Congelar Tiempo")
            {
                HelpText = "Alterna el congelamiento del tiempo."
            });

            Add(new Command(Permissions.HomeDelete, Commands.DeleteHome, "delhome", "Eliminar Casa")
            {
                AllowServer = false,
                HelpText = "Elimina uno de tus puntos de casa."
            });
            Add(new Command(Permissions.HomeSet, Commands.SetHome, "sethome", "Establecer Casa")
            {
                AllowServer = false,
                HelpText = "Establece uno de tus puntos de casa."
            });
            Add(new Command(Permissions.HomeTp, Commands.MyHome, "myhome", "Mi Casa")
            {
                AllowServer = false,
                HelpText = "Teletransporta a uno de tus puntos de casa."
            });

            Add(new Command(Permissions.KickAll, Commands.KickAll, "kickall", "Expulsar a Todos")
            {
                HelpText = "Expulsa a todos los jugadores en el servidor."
            });

            Add(new Command(Permissions.LastCommand, Commands.RepeatLast, "=", "Repetir Último Comando")
            {
                HelpText = "Permite repetir el último comando ejecutado."
            });

            Add(new Command(Permissions.More, Commands.More, "more", "Maximizar Apilamiento")
            {
                AllowServer = false,
                HelpText = "Maximiza la pila del item en la mano."
            });

            // Esto sobrescribirá el comando 'mute' de TShock.
            Add(new Command(Permissions.Mute, Commands.Mute, "mute", "Gestión de Silencio")
            {
                HelpText = "Gestiona el silencio de jugadores."
            });

            Add(new Command(Permissions.PvP, Commands.PvP, "pvpget2", "Cambiar Estado PvP")
            {
                AllowServer = false,
                HelpText = "Cambia tu estado de PvP."
            });

            Add(new Command(Permissions.Ruler, Commands.Ruler, "ruler", "Regla")
            {
                AllowServer = false,
                HelpText = "Permite medir la distancia entre dos bloques."
            });

            Add(new Command(Permissions.Send, Commands.Send, "send", "Enviar Mensaje")
            {
                HelpText = "Envía un mensaje personalizado a todos los jugadores."
            });

            Add(new Command(Permissions.Sudo, Commands.Sudo, "sudo", "Ejecutar Como")
            {
                HelpText = "Permite ejecutar comandos como si fueras otro jugador."
            });

            Add(new Command(Permissions.TimeCmd, Commands.TimeCmd, "timecmd", "Comando Temporal")
            {
                HelpText = "Ejecuta un comando después de un intervalo de tiempo."
            });

            Add(new Command(Permissions.TpBack, Commands.Back, "eback", "Regresar")
            {
                AllowServer = false,
                HelpText = "Teletransporta de regreso a la posición anterior después de morir o teletransportarse."
            });
            Add(new Command(Permissions.TpDown, Commands.Down, "down", "Bajar")
            {
                AllowServer = false,
                HelpText = "Teletransporta hacia abajo por una capa de bloques."
            });
            Add(new Command(Permissions.TpLeft, Commands.Left, "left", "Izquierda")
            {
                AllowServer = false,
                HelpText = "Teletransporta hacia la izquierda por una capa de bloques."
            });
            Add(new Command(Permissions.TpRight, Commands.Right, "right", "Derecha")
            {
                AllowServer = false,
                HelpText = "Teletransporta hacia la derecha por una capa de bloques."
            });
            Add(new Command(Permissions.TpUp, Commands.Up, "up", "Arriba")
            {
                AllowServer = false,
                HelpText = "Teletransporta hacia arriba por una capa de bloques."
            });

            #endregion
        }

        private void OnPostInitialize(EventArgs args)
        {
            Homes = new HomeManager(Db);
        }

        private async void OnJoin(JoinEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            TSPlayer player = TShock.Players[e.Who];
            if (player == null)
            {
                return;
            }

            DateTime muteExpiration = await Mutes.GetExpirationAsync(player);

            if (DateTime.UtcNow < muteExpiration)
            {
                player.mute = true;
                try
                {
                    await Task.Delay(muteExpiration - DateTime.UtcNow, player.GetPlayerInfo().MuteToken);
                    player.mute = false;
                    player.SendInfoMessage("Has sido desmuteado.");
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private void OnGetData(GetDataEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            TSPlayer tsplayer = TShock.Players[e.Msg.whoAmI];
            if (tsplayer == null)
            {
                return;
            }

            switch (e.MsgID)
            {
                #region Packet 118 - PlayerDeathV2

                case PacketTypes.PlayerDeathV2:
                    if (tsplayer.Group.HasPermission(Permissions.TpBack))
                    {
                        tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                    }
                    return;

                case PacketTypes.Teleport:
                    {
                        if (tsplayer.Group.HasPermission(Permissions.TpBack))
                        {
                            using (MemoryStream ms = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
                            {
                                BitsByte flags = (byte)ms.ReadByte();

                                int type = 0;
                                if (flags[1])
                                {
                                    type = 2;
                                }

                                if (type == 0 && tsplayer.Group.HasPermission(TShockAPI.Permissions.rod))
                                {
                                    tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                                }
                                else if (type == 2 && tsplayer.Group.HasPermission(TShockAPI.Permissions.wormhole))
                                {
                                    tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                                }
                            }
                        }
                    }
                    return;

                    #endregion
            }
        }
    }
}
