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
import { fileURLToPath } from 'url';
import {dirname} from 'path';
import path from 'path';


// Dz
const client = new Client({ intents: [GatewayIntentBits.Guilds] });
const __dirname = dirname(fileURLToPath(import.meta.url));

export default client;
// Dzend



// Filesystem (fs)

client.commands = new Collection();

// commands

const foldersPathCmds = path.join(__dirname, 'cmds');
const commandFoldersCmds = fs.readdirSync(foldersPathCmds);

(async () => {
  for (const folder of commandFoldersCmds) {
    const commandsPath = path.join(foldersPathCmds, folder);
    const commandFiles = fs.readdirSync(commandsPath).filter(file => file.endsWith('.js'));

    for (const file of commandFiles) {
      const filePath = path.join(commandsPath, file);

      const commandModule = await import(`file://${filePath}`);
      const command = commandModule.default ?? commandModule;

      if ('data' in command && 'execute' in command) {
        client.commands.set(command.data.name, command);
      } else {
        console.log(`[WARNING] The command at ${filePath} is missing a required "data" or "execute" property.`);
      }
    }
  }
})();

// Events

const eventsPath = path.join(__dirname, 'Events');
const eventFiles = fs.readdirSync(eventsPath).filter(file => file.endsWith('.js'));

(async () => {
  for (const file of eventFiles) {
    const filePath = path.join(eventsPath, file);

    const eventModule = await import(`file://${filePath}`);
    const event = eventModule.default ?? eventModule;

    if (event.once) {
      client.once(event.name, (...args) => event.execute(...args));
    } else {
      client.on(event.name, (...args) => event.execute(...args));
    }
  }
})();
// Fsend


// Eventsend

client.login(tokends);

import { REST, Routes } from 'discord.js';

import cmdss from './lists/commands.json' with {"type": "json"}
const commands = cmdss;

const rest = new REST({ version: '10' }).setToken(tokends);


try {
  console.log('Started refreshing application (/) commands.');

  await rest.put(Routes.applicationCommands(userid), { body: commands });

  console.log('Successfully reloaded application (/) commands.');
} catch (error) {
  console.error(error);
}


