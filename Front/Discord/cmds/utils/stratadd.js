import { SlashCommandBuilder } from 'discord.js';

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default {
    data: new SlashCommandBuilder()
        .setName('strat-add')
        .setDescription('Add new strategy')
        .addStringOption(option =>
            option.setName('name')
                .setDescription('Strategy name')
                .setRequired(true))
        .addStringOption(option =>
            option.setName('video')
                .setDescription('Video URL')
                .setRequired(true))
        .addIntegerOption(option =>
            option.setName('map')
                .setDescription('Map ID')
                .setRequired(true)
                .addChoices(
                    { name: 'Oregon', value: 1 },
                    { name: 'Coastline', value: 2 },
                    { name: 'Clubhouse', value: 3 },
                    { name: 'Bank', value: 4 }
                )
        ),

    async execute(interaction) {

        const name = interaction.options.getString('name');
        const video = interaction.options.getString('video');

        // ⚡ Важно: map это integer
        const mapId = interaction.options.getInteger('map');

        const res = await fetch(`${API_BASE}/api/strats/`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                name: name,
                videoUrl: video,
                mapId: mapId
            })
        });

        const data = await res.json();

        await interaction.reply(data.message || 'Strategy added');
    }
};