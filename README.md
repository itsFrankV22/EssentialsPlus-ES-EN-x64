# EssentialsPlus
## para TShock5 por FrankV2
## Traducción: [FrankV22](https://github.com/itsFrankV22)

If you speak another language please visit [README_ENGLISH](https://github.com/itsFrankV22/EssentialsPlus-ES-EN-x64/blob/main/README_ENGLISH.md)


Essentials+ es una combinación que mejora y optimiza algunas funciones de Essentials y MoreAdminCommands. Todos los comandos se ejecutan de forma asíncrona. No se incluyen comandos de banderas.

## Comandos ##

- **/find** -> Incluye varios subcomandos:
    - **-command** -> Busca comandos específicos según la entrada y devuelve los comandos coincidentes y sus permisos.
    - **-item** -> Busca objetos específicos según la entrada y devuelve los objetos coincidentes y sus ID.
    - **-tile** -> Busca bloques específicos según la entrada y devuelve los bloques coincidentes y sus ID.
    - **-wall** -> Busca paredes específicas según la entrada y devuelve las paredes coincidentes y sus ID.
- **/freezetime** -> Congela o descongela el tiempo.
- **/delhome** <nombre del hogar> -> Elimina uno de tus puntos de hogar.
- **/sethome** <nombre del hogar> -> Establece uno de tus puntos de hogar.
- **/myhome** <nombre del hogar> -> Teletranspórtate a uno de tus puntos de hogar.
- **/kickall** <razón> -> Expulsa a todos los jugadores del servidor.
- **/=** -> Repite el último comando que ingresaste (no incluye otras iteraciones de /=).
- **/more** -> Maximiza la pila de objetos en la mano. Subcomando:
    - **-all** -> Maximiza todos los objetos apilables en el inventario del jugador.
- **/mute** -> Reemplaza el comando /mute de TShock. Incluye subcomandos:
    - **add** <nombre> <tiempo> -> Añade un mute al usuario con nombre <nombre>, por un tiempo de <tiempo>.
    - **delete** <nombre> -> Elimina el mute del usuario con nombre <nombre>.
    - **help** -> Muestra la información del comando.
- **/pvpget2** -> Cambia tu estado de PvP.
- **/ruler** [1|2] -> Mide la distancia entre el punto 1 y el punto 2.
- **/send** -> Transmite un mensaje con un color personalizado.
- **/sudo** -> Intenta que <jugador> ejecute <comando>. Incluye subcomandos:
    - **-force** -> Ejecuta el comando obligatoriamente, sin restricciones de permisos del <jugador>.
- **/timecmd** -> Ejecuta un comando después de un intervalo de tiempo dado. Incluye subcomandos:
    - **-repeat** -> Repite la ejecución de <comando> cada <tiempo>.
- **/eback** [número de pasos] -> Te lleva de vuelta a la ubicación anterior. Si se proporciona [número de pasos], intenta llevarte de vuelta a la posición [número de pasos] antes.
- **/down** [niveles] -> Intenta mover tu posición en el mapa hacia abajo. Si se especifica [niveles], intenta mover hacia abajo [niveles] veces.
- **/left** [niveles] -> Similar a /down [niveles], pero mueve hacia la izquierda.
- **/right** [niveles] -> Similar a /down [niveles], pero mueve hacia la derecha.
- **/up** [niveles] -> Similar a /down [niveles], pero mueve hacia arriba.

## Permisos ##

- essentials.find -> Permite el uso del comando /find.
- essentials.freezetime -> Permite el uso del comando /freezetime.
- essentials.home.delete -> Permite el uso de los comandos /delhome y /sethome.
- essentials.home.tp -> Permite el uso del comando /myhome.
- essentials.kickall -> Permite el uso del comando /kickall.
- essentials.lastcommand -> Permite el uso del comando /=.
- essentials.more -> Permite el uso del comando /more.
- essentials.mute -> Permite el uso del comando /mute.
- essentials.pvp -> Permite el uso del comando /pvpget2.
- essentials.ruler -> Permite el uso del comando /ruler.
- essentials.send -> Permite el uso del comando /send.
- essentials.sudo -> Permite el uso del comando /sudo.
- essentials.timecmd -> Permite el uso del comando /timecmd.
- essentials.tp.eback -> Permite el uso del comando /eback.
- essentials.tp.down -> Permite el uso del comando /down.
- essentials.tp.left -> Permite el uso del comando /left.
- essentials.tp.right -> Permite el uso del comando /right.
- essentials.tp.up -> Permite el uso del comando /up.

#INFO
E compilado y traducido este plugin con el fin de permitir a otras personas novatas como yo en mi momento poder obtener esto mas facil


> [!NOTE]
> Esta traduccion es de google traductor viene del chino simplificado asi que puede que tenga errores en gramatica y ortografia

> [!WARNING]
> Estoy practicando en el lenguaje C# asi que puede que esto tenga errores

## Soporte y Comentarios
- Esta es una mejora del repostorio original [EssentialsPlus](https://github.com/THEXN/EssentialsPlus/)
- Si encuentras problemas o tienes sugerencias, no dudes en reportarlos en el foro oficial o en la comunidad.
- Repositorio de GitHub: https://github.com/THEXN/EssentialsPlus/
