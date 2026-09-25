import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';

import { getProfile, profileUrl, TrnError } from '../../api/r6tracker.js';

const PLATFORMS = [
    { name: 'PC', value: 'uplay' },
    { name: 'PlayStation', value: 'psn' },
    { name: 'Xbox', value: 'xbl' }
];

function field(name, value) {
    return {
        name,
        value: value ?? '—',
        inline: true
    };
}

export default {
    data: new SlashCommandBuilder()
        .setName('r6stats')
        .setDescription('Show Rainbow Six Siege player stats from R6 Tracker')
        .addStringOption(option =>
            option
                .setName('platform')
                .setDescription('Player platform')
                .setRequired(true)
                .addChoices(...PLATFORMS)
        )
        .addStringOption(option =>
            option
                .setName('nickname')
                .setDescription('Player nickname')
                .setRequired(true)
        )
        .addStringOption(option =>
            option
                .setName('section')
                .setDescription('What to show')
                .setRequired(false)
                .addChoices(
                    { name: 'Profile', value: 'profile' },
                    { name: 'Playlists', value: 'playlists' },
                    { name: 'Operators', value: 'operators' }
                )
        ),

    async execute(interaction) {
        await interaction.deferReply();

        const platform = interaction.options.getString('platform');
        const nickname = interaction.options.getString('nickname');
        const section = interaction.options.getString('section') || 'profile';

        try {
            const { profile, playlists, operators } = await getProfile(platform, nickname);

            const embed = new EmbedBuilder()
                .setColor(0x0099ff)
                .setTitle(`🎮 ${profile.handle}`)
                .setURL(profileUrl(platform, nickname))
                .setFooter({
                    text: 'Powered by tracker.gg • data cached for 5 minutes'
                })
                .setTimestamp();

            if (profile.avatarUrl) {
                embed.setThumbnail(profile.avatarUrl);
            }

            if (section === 'profile') {
                embed
                    .setDescription('Lifetime stats')
                    .addFields(
                        field('K/D', profile.kd),
                        field('Win rate', profile.wlPercent),
                        field('Matches', profile.matchesPlayed),
                        field('Kills', profile.kills),
                        field('Deaths', profile.deaths),
                        field('W/L', profile.wins && profile.losses
                            ? `${profile.wins} / ${profile.losses}`
                            : null),
                        field('Time played', profile.timePlayed)
                    );
            } else if (section === 'playlists') {
                embed.setDescription('Stats by playlist');

                if (playlists.length === 0) {
                    embed.addFields({
                        name: 'No data',
                        value: 'Playlist stats are not available for this player.'
                    });
                } else {
                    for (const playlist of playlists.slice(0, 9)) {
                        embed.addFields({
                            name: playlist.name,
                            value: [
                                playlist.rank && `Rank: ${playlist.rank}`,
                                playlist.mmr && `MMR: ${playlist.mmr}`,
                                playlist.kd && `K/D: ${playlist.kd}`,
                                playlist.wlPercent && `WR: ${playlist.wlPercent}`,
                                playlist.matchesPlayed && `Matches: ${playlist.matchesPlayed}`
                            ].filter(Boolean).join('\n') || '—',
                            inline: true
                        });
                    }
                }
            } else {
                embed.setDescription('Top operators by rounds played');

                if (operators.length === 0) {
                    embed.addFields({
                        name: 'No data',
                        value: 'Operator stats are not available for this player.'
                    });
                } else {
                    for (const operator of operators.slice(0, 9)) {
                        embed.addFields({
                            name: operator.name,
                            value: [
                                operator.kills && `Kills: ${operator.kills}`,
                                operator.kd && `K/D: ${operator.kd}`,
                                operator.wins && `Wins: ${operator.wins}`,
                                operator.roundsPlayed && `Rounds: ${operator.roundsPlayed}`
                            ].filter(Boolean).join('\n') || '—',
                            inline: true
                        });
                    }
                }
            }

            await interaction.editReply({ embeds: [embed] });
        } catch (error) {
            if (!(error instanceof TrnError)) {
                console.error('r6stats error:', error);
            }

            await interaction.editReply({
                content: `❌ ${error.message || 'Failed to load player stats.'}`
            });
        }
    },
};
