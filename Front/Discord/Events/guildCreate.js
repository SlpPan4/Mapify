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

