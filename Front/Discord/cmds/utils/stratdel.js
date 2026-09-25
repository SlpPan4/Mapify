import {
    SlashCommandBuilder,
    EmbedBuilder,
    ActionRowBuilder,
    ButtonBuilder,
    ButtonStyle,
    PermissionFlagsBits
} from 'discord.js';

import { deleteStrat } from '../../api/api.js';

export default {

    data: new SlashCommandBuilder()

        .setName('strat-delete')

        .setDescription('Delete strategy')

        .setDefaultMemberPermissions(PermissionFlagsBits.ManageGuild)

        .addIntegerOption(option =>
            option
                .setName('id')
                .setDescription(
                    'Strategy ID (see /strats-list)'
                )
                .setRequired(true)
                .setMinValue(1)
        ),


    async execute(interaction) {

        const id =
            interaction.options.getInteger('id');


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


        const buttons =
            new ActionRowBuilder()

                .addComponents(

                    new ButtonBuilder()
                        .setCustomId('strat_delete_confirm')
                        .setLabel('Confirm')
                        .setEmoji('✅')
                        .setStyle(ButtonStyle.Danger),

                    new ButtonBuilder()
                        .setCustomId('strat_delete_cancel')
                        .setLabel('Cancel')
                        .setEmoji('❌')
                        .setStyle(ButtonStyle.Secondary)

                );


        await interaction.reply({
            embeds: [embed],
            components: [buttons]
        });

        const message =
            await interaction.fetchReply();


        const collector =
            message.createMessageComponentCollector({
                time: 60 * 1000
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
                            .setTitle('❌ Deletion Cancelled')
                            .setDescription(
                                'The strategy was not deleted.'
                            )
                            .addFields({
                                name: '🆔 Strategy ID',
                                value: `\`${id}\``,
                                inline: false
                            });

                    await buttonInteraction.update({
                        embeds: [cancelledEmbed],
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

                    await buttonInteraction.deferUpdate();

                    try {

                        await deleteStrat(id);

                        const successEmbed =
                            new EmbedBuilder()
                                .setColor(0x00AE86)
                                .setTitle('✅ Strategy Deleted')
                                .setDescription(
                                    'Strategy successfully deleted.'
                                )
                                .addFields({
                                    name: '🆔 Deleted Strategy ID',
                                    value: `\`${id}\``,
                                    inline: false
                                });

                        await buttonInteraction.editReply({
                            embeds: [successEmbed],
                            components: []
                        });

                    } catch (error) {

                        console.error(
                            'strat-delete error:',
                            error
                        );

                        const errorEmbed =
                            new EmbedBuilder()
                                .setColor(0xFF0000)
                                .setTitle('❌ Failed to Delete Strategy')
                                .setDescription(
                                    error.message ||
                                    'Unknown backend error.'
                                )
                                .addFields({
                                    name: '🆔 Strategy ID',
                                    value: `\`${id}\``,
                                    inline: false
                                });

                        await buttonInteraction.editReply({
                            embeds: [errorEmbed],
                            components: []
                        });

                    }

                    collector.stop();

                }

            }
        );


        /*
         * Disable buttons after 60 seconds
         */

        collector.on(
            'end',
            async (collected, reason) => {

                if (reason === 'user') {
                    return;
                }

                try {

                    const disabledButtons =
                        new ActionRowBuilder()

                            .addComponents(

                                new ButtonBuilder()
                                    .setCustomId('strat_delete_confirm_disabled')
                                    .setLabel('Confirm')
                                    .setEmoji('✅')
                                    .setStyle(ButtonStyle.Danger)
                                    .setDisabled(true),

                                new ButtonBuilder()
                                    .setCustomId('strat_delete_cancel_disabled')
                                    .setLabel('Cancel')
                                    .setEmoji('❌')
                                    .setStyle(ButtonStyle.Secondary)
                                    .setDisabled(true)

                            );

                    await interaction.editReply({
                        components: [disabledButtons]
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
