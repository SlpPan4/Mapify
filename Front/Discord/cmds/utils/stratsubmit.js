import { SlashCommandBuilder } from 'discord.js';

import { submitStrat } from '../../api/api.js';

const MAPS = [
    { name: 'Oregon', id: 1 },
    { name: 'Consulate', id: 2 },
    { name: 'Bank', id: 3 },
    { name: 'Clubhouse', id: 4 },
    { name: 'Border', id: 5 },
    { name: 'Fortress', id: 6 },
    { name: 'Coastline', id: 7 },
    { name: 'Chalet', id: 8 },
    { name: 'Kafe', id: 9 },
    { name: 'Outback', id: 10 },
    { name: 'Nighthaven Labs', id: 11 },
    { name: 'Lair', id: 12 },
    { name: 'Kanal', id: 13 },
    { name: 'Villa', id: 14 },
    { name: 'Skyscraper', id: 15 },
    { name: 'Theme park', id: 16 },
    { name: 'Emerald Plains', id: 17 },
    { name: 'Favela', id: 18 },
    { name: 'Tower', id: 19 },
    { name: 'Yacht', id: 20 },
    { name: 'Presidential Plane', id: 21 },
    { name: 'Stadium Bravo', id: 22 },
    { name: 'Stadium 2020', id: 23 }
];

export default {

    data: new SlashCommandBuilder()

        .setName('strat-submit')

        .setDescription('Submit a strategy for review')

        .addStringOption(option =>
            option
                .setName('name')
                .setDescription('Strategy name')
                .setRequired(true)
        )

        .addStringOption(option =>
            option
                .setName('video')
                .setDescription('Video URL')
                .setRequired(true)
        )

        .addIntegerOption(option =>
            option
                .setName('map')
                .setDescription('Map')
                .setRequired(true)
                .addChoices(
                    ...MAPS.map(map => ({
                        name: map.name,
                        value: map.id
                    }))
                )
        )

        .addStringOption(option =>
            option
                .setName('description')
                .setDescription('Strategy description')
                .setRequired(false)
        ),


    async execute(interaction) {

        await interaction.deferReply();

        try {

            const name =
                interaction.options.getString('name');

            const video =
                interaction.options.getString('video');

            const description =
                interaction.options.getString('description') || '';

            const mapId =
                interaction.options.getInteger('map');

            const mapName =
                MAPS.find(map => map.id === mapId)?.name;

            if (!mapName) {

                return await interaction.editReply({
                    content: '❌ Invalid map selected.'
                });

            }

            const data = await submitStrat({
                name,
                videoUrl: video,
                description,
                mapId
            });

            const submissionId =
                data?.submissionId ?? 'unknown';

            await interaction.editReply({
                content:
                    `✅ Strategy **${name}** submitted for review!\n\n` +
                    `🆔 Submission ID: ${submissionId}\n` +
                    `🗺️ Map: ${mapName}\n\n` +
                    `It will appear in the bot once approved in the admin panel.`
            });

        } catch (error) {

            console.error(
                'strat-submit error:',
                error
            );

            await interaction.editReply({
                content:
                    `❌ Failed to submit the strategy.\n\n${error.message}`
            });

        }

    }

};
