import {
    Events,
    EmbedBuilder
} from 'discord.js';

export default {

    name: Events.GuildDelete,

    once: false,

    async execute(guild) {

        const leaveEmbed = new EmbedBuilder()

            .setColor('#df0922')

            .setTitle('Mapify left a server')

            .setThumbnail(
                guild.iconURL() || null
            )

            .setDescription(
                `:eye: -> ${guild.name}`
            )

            .addFields(

                {
                    name: 'ID',
                    value: guild.id
                },

                {
                    name: 'Members',
                    value: String(
                        guild.memberCount ?? 0
                    )
                },

                {
                    name: 'Owner ID',
                    value: guild.ownerId ?? 'Unknown'
                }

            )

            .setTimestamp();


        const channelId =
            '1446991723384279134';


        try {

            const channel =
                await guild.client.channels.fetch(
                    channelId
                );


            if (
                !channel ||
                !channel.isTextBased()
            ) {

                console.log(
                    'Not a text channel'
                );

                return;

            }


            await channel.send(
                '<@594130786672836611>'
            );


            await channel.send({
                embeds: [
                    leaveEmbed
                ]
            });


        } catch (err) {

            console.error(
                'Error sending guild leave message:',
                err
            );

        }

    }

};
