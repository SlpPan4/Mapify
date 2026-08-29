import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default {
    data: new SlashCommandBuilder()
        .setName('strats-list')
        .setDescription('Show all available strats grouped by maps'),

    async execute(interaction) {

        const req = await fetch(`${API_BASE}/api/strats/`);
        const strats = await req.json();

        const grouped = {};

        for (const strat of strats) {
            const mapReq = await fetch(`${API_BASE}/api/strats/maps/${strat.mapId}`);
            const map = await mapReq.json();

            if (!grouped[map.name]) {
                grouped[map.name] = [];
            }

            grouped[map.name].push(`ID: ${strat.id} | ${strat.name}`);
        }

        const embed = new EmbedBuilder()
            .setColor(0x00AE86)
            .setTitle('All Strategies by Maps');

        for (const [mapName, list] of Object.entries(grouped)) {
            embed.addFields({
                name: mapName,
                value: list.join('\n'),
                inline: false
            });
        }

        await interaction.reply({ embeds: [embed] });
    }
};
