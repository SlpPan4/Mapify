import { Client, Collection, Events, GatewayIntentBits, MessageFlags } from 'discord.js';

// Локально подхватывает .env; в Docker переменные приходят от docker compose
try {
  process.loadEnvFile();
} catch {
  // .env отсутствует — берём переменные из окружения
}

const tokends = process.env.DISCORD_TOKEN;
const userid = process.env.DISCORD_CLIENT_ID;

if (!tokends || !userid) {
  console.error('DISCORD_TOKEN and DISCORD_CLIENT_ID must be set (via .env or environment).');
  process.exit(1);
}
// import request from 'request';

import fs from 'fs';

import {
    fileURLToPath,
    pathToFileURL
} from 'url';

// Dz
const client = new Client({ intents: [GatewayIntentBits.Guilds] });
const __dirname = dirname(fileURLToPath(import.meta.url));


/*
 * ========================================
 * AUTH
 * ========================================
 */

const tokends = AUTH.tokends;
const userid = AUTH.userid;


if (!tokends) {
    throw new Error(
        '❌ tokends is missing in authds.json'
    );
}

if (!userid) {
    throw new Error(
        '❌ userid is missing in authds.json'
    );
}


/*
 * ========================================
 * CLIENT
 * ========================================
 */

const client = new Client({

    intents: [
        GatewayIntentBits.Guilds
    ]

});


/*
 * ========================================
 * PATH
 * ========================================
 */

const __dirname = dirname(
    fileURLToPath(import.meta.url)
);


/*
 * ========================================
 * COMMAND COLLECTION
 * ========================================
 */

client.commands = new Collection();


/*
 * ========================================
 * START BOT
 * ========================================
 */

async function start() {

    /*
     * ====================================
     * LOAD COMMANDS
     * ====================================
     */

    const commands = [];

    const foldersPathCmds = join(
        __dirname,
        'cmds'
    );


    const commandFoldersCmds =
        fs.readdirSync(
            foldersPathCmds
        );


    for (
        const folder
        of commandFoldersCmds
    ) {

        const commandsPath = join(
            foldersPathCmds,
            folder
        );


        /*
         * Only process folders
         */

        if (
            !fs.statSync(
                commandsPath
            ).isDirectory()
        ) {

            continue;

        }


        const commandFiles =
            fs
                .readdirSync(
                    commandsPath
                )
                .filter(
                    file =>
                        file.endsWith('.js')
                );


        for (
            const file
            of commandFiles
        ) {

            const filePath = join(
                commandsPath,
                file
            );


            try {

                /*
                 * Convert filesystem path
                 * to a proper file:// URL.
                 */

                const fileUrl =
                    pathToFileURL(
                        filePath
                    ).href;


                const commandModule =
                    await import(
                        fileUrl
                    );


                const command =
                    commandModule.default ??
                    commandModule;


                /*
                 * Check command structure
                 */

                if (
                    !command ||
                    !command.data ||
                    typeof command.execute !== 'function'
                ) {

                    console.warn(
                        `[WARNING] Invalid command: ${filePath}`
                    );

                    continue;

                }


                const commandName =
                    command.data.name;


                /*
                 * Prevent duplicate commands
                 */

                if (
                    client.commands.has(
                        commandName
                    )
                ) {

                    console.error(
                        `[ERROR] Duplicate command: /${commandName}`
                    );

                    console.error(
                        `[ERROR] File: ${filePath}`
                    );

                    continue;

                }


                /*
                 * Save command
                 */

                client.commands.set(
                    commandName,
                    command
                );


                /*
                 * Prepare command for Discord API
                 */

                commands.push(
                    command.data.toJSON()
                );


                console.log(
                    `[COMMAND] Loaded /${commandName}`
                );


            } catch (error) {

                console.error(
                    `[ERROR] Failed to load command: ${filePath}`
                );

                console.error(error);

            }

        }

    }


    console.log(
        `[COMMANDS] Loaded ${commands.length} commands.`
    );


    /*
     * ====================================
     * REGISTER SLASH COMMANDS
     * ====================================
     */

    const rest = new REST({
        version: '10'
    });


    rest.setToken(
        tokends
    );


    try {

        console.log(
            'Started refreshing application (/) commands.'
        );


        await rest.put(

            Routes.applicationCommands(
                userid
            ),

            {
                body: commands
            }

        );


        console.log(
            `Successfully registered ${commands.length} application (/) commands.`
        );


    } catch (error) {

        console.error(
            'Failed to register application commands:',
            error
        );

    }


    /*
     * ====================================
     * LOAD EVENTS
     * ====================================
     */

    const eventsPath = join(
        __dirname,
        'Events'
    );


    if (
        !fs.existsSync(eventsPath)
    ) {

        console.warn(
            '[WARNING] Events folder not found.'
        );

    } else {

        const eventFiles =
            fs
                .readdirSync(
                    eventsPath
                )
                .filter(
                    file =>
                        file.endsWith('.js')
                );


        for (
            const file
            of eventFiles
        ) {

            const filePath = join(
                eventsPath,
                file
            );


            try {

                const fileUrl =
                    pathToFileURL(
                        filePath
                    ).href;


                const eventModule =
                    await import(
                        fileUrl
                    );


                const event =
                    eventModule.default ??
                    eventModule;


                /*
                 * Check event structure
                 */

                if (
                    !event ||
                    !event.name ||
                    typeof event.execute !== 'function'
                ) {

                    console.warn(
                        `[WARNING] Invalid event: ${filePath}`
                    );

                    console.warn(
                        '[WARNING] Event must export { name, execute, once? }'
                    );

                    continue;

                }


                /*
                 * Register event
                 */

                if (event.once) {

                    client.once(

                        event.name,

                        (...args) =>
                            event.execute(
                                ...args
                            )

                    );

                } else {

                    client.on(

                        event.name,

                        (...args) =>
                            event.execute(
                                ...args
                            )

                    );

                }


                console.log(
                    `[EVENT] Loaded ${event.name}`
                );


            } catch (error) {

                console.error(
                    `[ERROR] Failed to load event: ${filePath}`
                );

                console.error(error);

            }

        }

    }


    /*
     * ====================================
     * LOGIN
     * ====================================
     */

    try {

        await client.login(
            tokends
        );


    } catch (error) {

        console.error(
            '❌ Failed to login to Discord:',
            error
        );

    }

}


/*
 * ========================================
 * RUN
 * ========================================
 */

start().catch(
    error => {

        console.error(
            '❌ Fatal bot error:',
            error
        );

    }
);


/*
 * ========================================
 * EXPORT
 * ========================================
 */

export default client;
