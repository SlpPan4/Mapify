import {
    SlashCommandBuilder,
    EmbedBuilder,
    ActionRowBuilder,
    ButtonBuilder,
    ButtonStyle
} from 'discord.js';

import { getStrats, getMap } from '../../api/api.js';

const mapCache = new Map();

async function getCachedMapName(mapId) {
    if (mapCache.has(mapId)) {
        return mapCache.get(mapId);
    }

    let name = 'Unknown';

    try {
        const map = await getMap(mapId);
        name = map?.name || 'Unknown';
    } catch (error) {
        console.error(`Failed to get map ${mapId}:`, error);
    }

    mapCache.set(mapId, name);
    return name;
}

export default {
    data: new SlashCommandBuilder()
        .setName('strats-list')
        .setDescription('Show all strategies'),

    async execute(interaction) {
        await interaction.deferReply();

        try {
            const strats = (await getStrats()) || [];

            if (strats.length === 0) {
                return await interaction.editReply({
                    content: 'There are no strategies in the database.'
                });
            }

            let currentPage = 0;
            const totalPages = strats.length;

            async function getMapName(mapId) {
                return await getCachedMapName(mapId);
            }

            async function createEmbed(page) {
                const strat = strats[page];

                const mapName =
                    await getMapName(strat.mapId);

                let videoUrl =
                    strat.videoUrl || '';

                if (
                    videoUrl &&
                    !videoUrl.startsWith('http://') &&
                    !videoUrl.startsWith('https://')
                ) {
                    videoUrl =
                        'https://' + videoUrl;
                }

                const description =
                    strat.description ||
                    'No description set.';

                let youtubeVideoId = null;

                try {
                    const url =
                        new URL(videoUrl);

                    if (
                        url.hostname.includes('youtube.com') &&
                        url.searchParams.get('v')
                    ) {
                        youtubeVideoId =
                            url.searchParams.get('v');

                    } else if (
                        url.hostname === 'youtu.be'
                    ) {
                        youtubeVideoId =
                            url.pathname.substring(1);

                    } else if (
                        url.hostname.includes('youtube.com') &&
                        url.pathname.startsWith('/shorts/')
                    ) {
                        youtubeVideoId =
                            url.pathname.split('/')[2];

                    } else if (
                        url.hostname.includes('youtube.com') &&
                        url.pathname.startsWith('/embed/')
                    ) {
                        youtubeVideoId =
                            url.pathname.split('/')[2];
                    }

                } catch (error) {
                    console.log(
                        'Could not parse video URL:',
                        videoUrl
                    );
                }

                const embed =
                    new EmbedBuilder()
                        .setColor(0x0099ff)
                        .setTitle(`🎯 ${strat.name}`)
                        .setDescription(description)
                        .addFields(
                            {
                                name: '🆔 Strategy ID',
                                value: String(strat.id),
                                inline: true
                            },
                            {
                                name: '🗺️ Map',
                                value: mapName,
                                inline: true
                            },
                            {
                                name: '🎥 Video',
                                value: videoUrl
                                    ? `[Watch strategy video](${videoUrl})`
                                    : 'No video provided',
                                inline: false
                            }
                        )
                        .setFooter({
                            text:
                                `Strategy ${page + 1}/${totalPages}`
                        })
                        .setTimestamp();

                if (youtubeVideoId) {
                    embed.setThumbnail(
                        `https://img.youtube.com/vi/${youtubeVideoId}/hqdefault.jpg`
                    );
                }

                return embed;
            }

            function createButtons(page) {
                return new ActionRowBuilder()
                    .addComponents(
                        new ButtonBuilder()
                            .setCustomId(
                                'strats_list_previous'
                            )
                            .setLabel('Previous')
                            .setEmoji('⬅️')
                            .setStyle(
                                ButtonStyle.Secondary
                            )
                            .setDisabled(
                                page === 0
                            ),

                        new ButtonBuilder()
                            .setCustomId(
                                'strats_list_next'
                            )
                            .setLabel('Next')
                            .setEmoji('➡️')
                            .setStyle(
                                ButtonStyle.Primary
                            )
                            .setDisabled(
                                page === totalPages - 1
                            )
                    );
            }

            const message =
                await interaction.editReply({
                    embeds: [
                        await createEmbed(currentPage)
                    ],
                    components: [
                        createButtons(currentPage)
                    ],
                    fetchReply: true
                });

            const collector =
                message.createMessageComponentCollector({
                    time: 5 * 60 * 1000
                });

            collector.on(
                'collect',
                async buttonInteraction => {

                    if (
                        buttonInteraction.user.id !==
                        interaction.user.id
                    ) {
                        return await buttonInteraction.reply({
                            content:
                                '❌ You cannot control this strategy list.',
                            ephemeral: true
                        });
                    }

                    if (
                        buttonInteraction.customId ===
                        'strats_list_previous'
                    ) {
                        if (currentPage > 0) {
                            currentPage--;
                        }
                    }

                    if (
                        buttonInteraction.customId ===
                        'strats_list_next'
                    ) {
                        if (
                            currentPage <
                            totalPages - 1
                        ) {
                            currentPage++;
                        }
                    }

                    await buttonInteraction.update({
                        embeds: [
                            await createEmbed(currentPage)
                        ],
                        components: [
                            createButtons(currentPage)
                        ]
                    });
                }
            );

            collector.on(
                'end',
                async () => {

                    try {
                        const disabledButtons =
                            new ActionRowBuilder()
                                .addComponents(

                                    new ButtonBuilder()
                                        .setCustomId(
                                            'strats_list_previous_disabled'
                                        )
                                        .setLabel('Previous')
                                        .setEmoji('⬅️')
                                        .setStyle(
                                            ButtonStyle.Secondary
                                        )
                                        .setDisabled(true),

                                    new ButtonBuilder()
                                        .setCustomId(
                                            'strats_list_next_disabled'
                                        )
                                        .setLabel('Next')
                                        .setEmoji('➡️')
                                        .setStyle(
                                            ButtonStyle.Primary
                                        )
                                        .setDisabled(true)
                                );

                        await interaction.editReply({
                            components: [
                                disabledButtons
                            ]
                        });

                    } catch (error) {
                        console.error(
                            'Failed to disable strategy buttons:',
                            error
                        );
                    }
                }
            );

        } catch (error) {

            console.error(
                'strats-list error:',
                error
            );

            await interaction.editReply({
                content:
                    '❌ Something went wrong while loading strategies.'
            });
        }
    }
};

