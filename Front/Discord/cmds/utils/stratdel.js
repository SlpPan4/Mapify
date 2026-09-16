import {
    SlashCommandBuilder,
    EmbedBuilder,
    ActionRowBuilder,
    ButtonBuilder,
    ButtonStyle
} from 'discord.js';


/*
 * Get current strategies from backend
 * when the bot starts.
 */

let maxStratId = 0;

try {

    const res = await fetch(
        'http://localhost:5000/api/strats/'
    );

    const data = await res.json();

    if (res.ok && Array.isArray(data.data)) {

        const strategies = data.data;

        if (strategies.length > 0) {

            maxStratId = Math.max(
                ...strategies.map(strat => strat.id)
            );

        }

        console.log(
            `[STRATS] Found ${strategies.length} strategies. Max ID: ${maxStratId}`
        );

    } else {

        console.error(
            '[STRATS] Failed to get strategies.'
        );

    }

} catch (error) {

    console.error(
        '[STRATS] Failed to connect to backend:',
        error
    );

}


const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default {

    data: new SlashCommandBuilder()

        .setName('strat-delete')

        .setDescription('Delete strategy')

        .addIntegerOption(option =>
            option
                .setName('id')
                .setDescription(
                    `Strategy ID (1-${maxStratId})`
                )
                .setRequired(true)
                .setMinValue(1)
                .setMaxValue(
                    maxStratId > 0
                        ? maxStratId
                        : 1
                )
        ),


    async execute(interaction) {

        const id =
            interaction.options.getInteger('id');


        /*
         * Check if there are strategies
         */

        if (maxStratId === 0) {

            return await interaction.reply({

                content:
                    '❌ There are no strategies available.',

                ephemeral: true

            });

        }


        /*
         * Additional validation
         */

        if (id > maxStratId) {

            return await interaction.reply({

                content:
                    `❌ Invalid strategy ID.\n\n` +
                    `The maximum strategy ID is **${maxStratId}**.`,

                ephemeral: true

            });

        }


        /*
         * Confirmation Embed
         */

        const embed = new EmbedBuilder()

            .setColor(0xFF0000)

            .setTitle('⚠️ Delete Strategy')

            .setDescription(
                'Are you sure you want to delete this strategy?'
            )

            .addFields({

                name: '🆔 Strategy ID',

                value: `\`${id}\``,

                inline: false

            })

            .setFooter({

                text:
                    'This action cannot be undone.'

            });


        /*
         * Confirmation buttons
         */

        const buttons =
            new ActionRowBuilder()

                .addComponents(

                    new ButtonBuilder()

                        .setCustomId(
                            'strat_delete_confirm'
                        )

                        .setLabel('Confirm')

                        .setEmoji('✅')

                        .setStyle(
                            ButtonStyle.Danger
                        ),

                    new ButtonBuilder()

                        .setCustomId(
                            'strat_delete_cancel'
                        )

                        .setLabel('Cancel')

                        .setEmoji('❌')

                        .setStyle(
                            ButtonStyle.Secondary
                        )

                );


        /*
         * Send confirmation message
         */

        await interaction.reply({

            embeds: [
                embed
            ],

            components: [
                buttons
            ]

        const res = await fetch(`${API_BASE}/api/strats/${id}`, {
            method: 'DELETE'
        });


        /*
         * Get sent message
         */

        const message =
            await interaction.fetchReply();


        /*
         * Button collector
         */

        const collector =
            message.createMessageComponentCollector({

                time: 60 * 1000

            });


        /*
         * Button pressed
         */

        collector.on(
            'collect',
            async buttonInteraction => {


                /*
                 * Only command author
                 * can use buttons.
                 */

                if (
                    buttonInteraction.user.id !==
                    interaction.user.id
                ) {

                    return await buttonInteraction.reply({

                        content:
                            '❌ You cannot control this confirmation.',

                        ephemeral: true

                    });

                }


                /*
                 * CANCEL
                 */

                if (
                    buttonInteraction.customId ===
                    'strat_delete_cancel'
                ) {

                    const cancelledEmbed =
                        new EmbedBuilder()

                            .setColor(0x808080)

                            .setTitle(
                                '❌ Deletion Cancelled'
                            )

                            .setDescription(
                                'The strategy was not deleted.'
                            )

                            .addFields({

                                name:
                                    '🆔 Strategy ID',

                                value:
                                    `\`${id}\``,

                                inline:
                                    false

                            });


                    await buttonInteraction.update({

                        embeds: [
                            cancelledEmbed
                        ],

                        components: []

                    });


                    collector.stop();

                    return;

                }


                /*
                 * CONFIRM
                 */

                if (
                    buttonInteraction.customId ===
                    'strat_delete_confirm'
                ) {

                    try {

                        /*
                         * DELETE request
                         */

                        const res = await fetch(

                            `http://localhost:5000/api/strats/${id}`,

                            {

                                method: 'DELETE'

                            }

                        );


                        /*
                         * Backend response
                         */

                        const data =
                            await res.json();


                        console.log(
                            'DELETE /api/strats response:',
                            data
                        );


                        /*
                         * Backend error
                         */

                        if (
                            !res.ok ||
                            data.error
                        ) {

                            const errorEmbed =
                                new EmbedBuilder()

                                    .setColor(0xFF0000)

                                    .setTitle(
                                        '❌ Failed to Delete Strategy'
                                    )

                                    .setDescription(
                                        data.error ||
                                        'Unknown backend error.'
                                    )

                                    .addFields({

                                        name:
                                            '🆔 Strategy ID',

                                        value:
                                            `\`${id}\``,

                                        inline:
                                            false

                                    });


                            await buttonInteraction.update({

                                embeds: [
                                    errorEmbed
                                ],

                                components: []

                            });


                            collector.stop();

                            return;

                        }


                        /*
                         * Successful deletion
                         */

                        const successEmbed =
                            new EmbedBuilder()

                                .setColor(0x00AE86)

                                .setTitle(
                                    '✅ Strategy Deleted'
                                )

                                .setDescription(
                                    data.message ||
                                    'Strategy successfully deleted.'
                                )

                                .addFields({

                                    name:
                                        '🆔 Deleted Strategy ID',

                                    value:
                                        `\`${id}\``,

                                    inline:
                                        false

                                });


                        await buttonInteraction.update({

                            embeds: [
                                successEmbed
                            ],

                            components: []

                        });


                        collector.stop();


                    } catch (error) {

                        console.error(
                            'strat-delete error:',
                            error
                        );


                        const errorEmbed =
                            new EmbedBuilder()

                                .setColor(0xFF0000)

                                .setTitle(
                                    '❌ Error'
                                )

                                .setDescription(
                                    'Failed to connect to the backend.'
                                )

                                .addFields({

                                    name:
                                        '🆔 Strategy ID',

                                    value:
                                        `\`${id}\``,

                                    inline:
                                        false

                                });


                        await buttonInteraction.update({

                            embeds: [
                                errorEmbed
                            ],

                            components: []

                        });


                        collector.stop();

                    }

                }

            }
        );


        /*
         * Disable buttons after 60 seconds
         */

        collector.on(
            'end',
            async () => {

                try {

                    const disabledButtons =
                        new ActionRowBuilder()

                            .addComponents(

                                new ButtonBuilder()

                                    .setCustomId(
                                        'strat_delete_confirm_disabled'
                                    )

                                    .setLabel('Confirm')

                                    .setEmoji('✅')

                                    .setStyle(
                                        ButtonStyle.Danger
                                    )

                                    .setDisabled(true),

                                new ButtonBuilder()

                                    .setCustomId(
                                        'strat_delete_cancel_disabled'
                                    )

                                    .setLabel('Cancel')

                                    .setEmoji('❌')

                                    .setStyle(
                                        ButtonStyle.Secondary
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
                        'Failed to disable delete buttons:',
                        error
                    );

                }

            }
        );

    }

};