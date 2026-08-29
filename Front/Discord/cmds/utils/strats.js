import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default {
    data: new SlashCommandBuilder()
        .setName('strats')
        .setDescription('test')
        .addIntegerOption(option =>
            option
                .setName('id')
                .setDescription('Choose strat ID')
                .setRequired(true)
                .setMinValue(1)
                .setMaxValue(8)
        ),

    async execute(interaction) {

        const id = interaction.options.getInteger('id');

        const strats = await fetch(`${API_BASE}/api/strats/`);
        const data = await strats.json();

        const strat = data.find(item => item.id === id);

        if (!strat) {
            return interaction.reply({
                content: 'Strategy not found on our server! Try checking the ID of the strategy using /strats-list!',
                ephemeral: true
            });
        }

        const map = await fetch(`${API_BASE}/api/strats/maps/${strat.mapId}`);
        const maps = await map.json();

        let dickins = strat.description || 'No description set.';

        const exampleEmbed = new EmbedBuilder()
            .setColor(0x0099ff)
            .setTitle(strat.name)
            .setURL('https://' + strat.videoUrl)
            .setDescription(dickins)
            .addFields(
                { name: 'Strategy ID', value: String(strat.id) },
                { name: 'Map', value: String(maps.name), inline: true }
            )
            .setTimestamp()
            .setFooter({ text: 'made with rainbow sex api' });

        await interaction.reply({ embeds: [exampleEmbed] });
    },
};