import {
    Events,
    EmbedBuilder
} from 'discord.js';

export default {

    name: Events.GuildCreate,

    once: false,

    async execute(guild) {

        const joinEmbed = new EmbedBuilder()

            .setColor('#04c404')

            .setTitle('Mapify Joined a server')

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
                        guild.memberCount
                    )
                },

                {
                    name: 'Owner ID',
                    value: guild.ownerId
                }

            )

            .setTimestamp();


        const channelId =
            process.env.LOG_CHANNEL_ID;

        if (!channelId) {
            return;
        }


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


            const pingUserId =
                process.env.LOG_PING_USER_ID;

            if (pingUserId) {
                await channel.send(
                    `<@${pingUserId}>`
                );
            }


            await channel.send({
                embeds: [
                    joinEmbed
                ]
            });


        } catch (err) {

            console.error(
                'Error sending guild join message:',
                err
            );

        }

    }

};

