import { SlashCommandBuilder } from 'discord.js';

export default {
    data: new SlashCommandBuilder()
        .setName('ping')
        .setDescription('Responds'),

    async execute(interaction) {
        const ping = Math.floor(Math.random() * (520 - 90 + 1)) + 90;

        await interaction.reply(
            `**Your ping to the Rainbow Six Siege servers is: ${ping} ms. Check [server status](<https://www.youtube.com/watch?v=zswT92VzOYM>)**`
        );
    },
};
