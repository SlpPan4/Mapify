import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

export default {
    data: new SlashCommandBuilder()
        .setName('help')
        .setDescription('Lists all available commands'),

    async execute(interaction) {
        const commands = interaction.client.commands;

        const description = [...commands.values()]
            .map(command =>
                `**${command.data.name}**\n-# ${command.data.description}`
            )
            .join('\n\n');

        const embed = new EmbedBuilder()
            .setTitle('📖 Available Commands')
            .setDescription(description || 'No commands available.')
            .setColor(0x0099ff);

        await interaction.reply({
            embeds: [embed],
            ephemeral: true,
        });
    },
};
