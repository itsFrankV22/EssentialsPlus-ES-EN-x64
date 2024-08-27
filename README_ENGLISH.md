# EssentialsPlus
## for TShock5 by FrankV2
## Translation: [FrankV22](https://github.com/itsFrankV22)

> [!NOTE]
>Si hablas otro idioma por favor visita [README_SPANISH](https://github.com/itsFrankV22/EssentialsPlus-ES-EN-x64/blob/main/README.md)


Essentials+ is a combination that improves and optimizes some functions from Essentials and MoreAdminCommands. All commands are executed asynchronously. Flag commands are not included.

## Commands ##

- **/find** -> Includes several subcommands:
    - **-command** -> Searches for specific commands based on input and returns matching commands and their permissions.
    - **-item** -> Searches for specific items based on input and returns matching items and their IDs.
    - **-tile** -> Searches for specific tiles based on input and returns matching tiles and their IDs.
    - **-wall** -> Searches for specific walls based on input and returns matching walls and their IDs.
- **/freezetime** -> Freezes or unfreezes time.
- **/delhome** <home name> -> Deletes one of your home points.
- **/sethome** <home name> -> Sets one of your home points.
- **/myhome** <home name> -> Teleports you to one of your home points.
- **/kickall** <reason> -> Kicks all players from the server.
- **/=** -> Repeats the last command you entered (does not include other iterations of /=).
- **/more** -> Maximizes the stack of the item in your hand. Subcommand:
    - **-all** -> Maximizes all stackable items in the player's inventory.
- **/mute** -> Overrides TShock's /mute command. Includes subcommands:
    - **add** <name> <time> -> Adds a mute for the user named <name>, for a time of <time>.
    - **delete** <name> -> Removes the mute for the user named <name>.
    - **help** -> Shows command information.
- **/pvpget2** -> Toggles your PvP status.
- **/ruler** [1|2] -> Measures the distance between point 1 and point 2.
- **/send** -> Broadcasts a message with a custom color.
- **/sudo** -> Attempts to make <player> execute <command>. Includes subcommands:
    - **-force** -> Executes the command forcibly, without the <player>'s permission restrictions.
- **/timecmd** -> Executes a command after a given time interval. Includes subcommands:
    - **-repeat** -> Repeats the execution of <command> every <time>.
- **/eback** [steps] -> Takes you back to the previous location. If [steps] is provided, tries to take you back to the position [steps] before.
- **/down** [levels] -> Attempts to move your position on the map downward. If [levels] is specified, tries to move downward [levels] times.
- **/left** [levels] -> Similar to /down [levels], but moves left.
- **/right** [levels] -> Similar to /down [levels], but moves right.
- **/up** [levels] -> Similar to /down [levels], but moves upward.

## Permissions ##

- essentials.find -> Allows use of the /find command.
- essentials.freezetime -> Allows use of the /freezetime command.
- essentials.home.delete -> Allows use of the /delhome and /sethome commands.
- essentials.home.tp -> Allows use of the /myhome command.
- essentials.kickall -> Allows use of the /kickall command.
- essentials.lastcommand -> Allows use of the /= command.
- essentials.more -> Allows use of the /more command.
- essentials.mute -> Allows use of the /mute command.
- essentials.pvp -> Allows use of the /pvpget2 command.
- essentials.ruler -> Allows use of the /ruler command.
- essentials.send -> Allows use of the /send command.
- essentials.sudo -> Allows use of the /sudo command.
- essentials.timecmd -> Allows use of the /timecmd command.
- essentials.tp.eback -> Allows use of the /eback command.
- essentials.tp.down -> Allows use of the /down command.
- essentials.tp.left -> Allows use of the /left command.
- essentials.tp.right -> Allows use of the /right command.
- essentials.tp.up -> Allows use of the /up command.

#INFO
I compiled and translated this plugin to help others who are new, like I was at the time, to access this more easily.


> [!NOTE]
> This translation is from Google Translate and comes from Simplified Chinese, so it may have grammatical and spelling errors.

> [!WARNING]
> I am practicing C# so this may have errors.

## Support and Feedback
- This is an enhancement of the original repository [EssentialsPlus](https://github.com/THEXN/EssentialsPlus/)
- If you encounter issues or have suggestions, feel free to report them on the official forum or community.
- GitHub repository: https://github.com/THEXN/EssentialsPlus/
