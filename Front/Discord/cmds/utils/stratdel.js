import { SlashCommandBuilder } from 'discord.js';

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default {
    data: new SlashCommandBuilder()
        .setName('strat-delete')
        .setDescription('Delete strategy')
        .addIntegerOption(option =>
            option.setName('id')
                .setDescription('Strategy ID')
                .setRequired(true)
        ),

    async execute(interaction) {

        const id = interaction.options.getInteger('id');

        const res = await fetch(`${API_BASE}/api/strats/${id}`, {
            method: 'DELETE'
        });

        const data = await res.json();

        await interaction.reply(data.message || 'Done');
    }
};