import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

import { getStrats, getMap } from '../../api/api.js';

export default {
    data: new SlashCommandBuilder()
        .setName('strats')
        .setDescription('Show a strategy by ID')
        .addIntegerOption(option =>
            option
                .setName('id')
                .setDescription('Strategy ID (see /strats-list)')
                .setRequired(true)
                .setMinValue(1)
        ),

    async execute(interaction) {
        await interaction.deferReply();

        try {
            const id = interaction.options.getInteger('id');

            const strats = await getStrats();
            const strat = (Array.isArray(strats) ? strats : []).find(item => item.id === id);

            if (!strat) {
                return await interaction.editReply({
                    content: 'Strategy not found on our server! Try checking the ID of the strategy using /strats-list!'
                });
            }

            let mapName = 'Unknown';

            try {
                const map = await getMap(strat.mapId);
                mapName = map?.name || 'Unknown';
            } catch (error) {
                console.error(`Failed to get map ${strat.mapId}:`, error);
            }

            let videoUrl = strat.videoUrl || '';

            if (videoUrl && !/^https?:\/\//i.test(videoUrl)) {
                videoUrl = 'https://' + videoUrl;
            }

            const embed = new EmbedBuilder()
                .setColor(0x0099ff)
                .setTitle(strat.name)
                .setDescription(strat.description || 'No description set.')
                .addFields(
                    { name: 'Strategy ID', value: String(strat.id), inline: true },
                    { name: 'Map', value: mapName, inline: true }
                )
                .setTimestamp();

            if (videoUrl) {
                embed.setURL(videoUrl);
            }

            await interaction.editReply({ embeds: [embed] });
        } catch (error) {
            console.error('strats error:', error);

            await interaction.editReply({
                content: '❌ Failed to load the strategy. Please try again later.'
            });
        }
    },
};
