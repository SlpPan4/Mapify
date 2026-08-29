import { SlashCommandBuilder } from 'discord.js';

export default {

    data: new SlashCommandBuilder()

        .setName('strat-add')

        .setDescription('Add new strategy')


        /*
         * FUCKING REQUIRED OPTIONS
        */

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

                /*
                 * The FUCKING map list.
                 */

                .addChoices(

                    { name: 'Bank', value: 3 },
                    { name: 'Border', value: 5 },
                    { name: 'Chalet', value: 8 },
                    { name: 'Clubhouse', value: 4 },
                    { name: 'Coastline', value: 7 },
                    { name: 'Consulate', value: 2 },
                    { name: 'Emerald Plains', value: 17 },
                    { name: 'Favela', value: 18 },
                    { name: 'Fortress', value: 6 },
                    { name: 'Kafe', value: 9 },
                    { name: 'Kanal', value: 13 },
                    { name: 'Lair', value: 12 },
                    { name: 'Nighthaven Labs', value: 11 },
                    { name: 'Oregon', value: 1 },
                    { name: 'Outback', value: 10 },
                    { name: 'Presidential Plane', value: 21 },
                    { name: 'Skyscraper', value: 15 },
                    { name: 'Stadium 2020', value: 23 },
                    { name: 'Stadium Bravo', value: 22 },
                    { name: 'Theme park', value: 16 },
                    { name: 'Tower', value: 19 },
                    { name: 'Villa', value: 14 },
                    { name: 'Yacht', value: 20 }

                )

        )


        /*
         * OPTIONAL STUFF
         */

        .addStringOption(option =>
            option
                .setName('description')
                .setDescription('Strategy description')
                .setRequired(false)
        ),


    async execute(interaction) {

        try {

            const name =
                interaction.options.getString('name');

            const video =
                interaction.options.getString('video');

            const description =
                interaction.options.getString('description') || '';

            const mapId =
                interaction.options.getInteger('map');



            const mapNames = {

                1: 'Oregon',
                2: 'Consulate',
                3: 'Bank',
                4: 'Clubhouse',
                5: 'Border',
                6: 'Fortress',
                7: 'Coastline',
                8: 'Chalet',
                9: 'Kafe',
                10: 'Outback',
                11: 'Nighthaven Labs',
                12: 'Lair',
                13: 'Kanal',
                14: 'Villa',
                15: 'Skyscraper',
                16: 'Theme park',
                17: 'Emerald Plains',
                18: 'Favela',
                19: 'Tower',
                20: 'Yacht',
                21: 'Presidential Plane',
                22: 'Stadium Bravo',
                23: 'Stadium 2020'

            };



            const mapName =
                mapNames[mapId];


            if (!mapName) {

                return await interaction.reply({

                    content:
                        '❌ Invalid map selected.',

                    ephemeral: true

                });

            }

            const res = await fetch(

                'http://localhost:5000/api/strats/',

                {

                    method: 'POST',

                    headers: {

                        'Content-Type':
                            'application/json'

                    },

                    body: JSON.stringify({

                        name: name,

                        videoUrl: video,

                        mapId: mapId,

                        mapName: mapName,

                        description: description

                    })

                }

            );


            /*
             * Давайте посмотрим что нам вернёт блядский бэкенд от дмитрия
             * Я заебался писать на английском всё равно ебучий репо для нас жрите мой хуй
             */

            const data =
                await res.json();


            console.log(
                'POST /api/strats response:',
                data
            );


            if (!res.ok) {

                return await interaction.reply({

                    content:
                        `❌ Failed to add strategy.\n\n` +

                        `HTTP: ${res.status}\n` +

                        `Error: ${
                            data.error ||
                            'Unknown error'
                        }`,

                    ephemeral: true

                });

            }


            if (data.error) {

                return await interaction.reply({

                    content:
                        `❌ Failed to add strategy.\n\n` +

                        `${data.error}`,

                    ephemeral: true

                });

            }



            const stratId =
                data.data?.stratId ?? 'unknown';



            await interaction.reply({

                content:
                    `✅ Strategy **${name}** successfully added!\n\n` +

                    `🆔 ID: ${stratId}\n` +

                    `🗺️ Map: ${mapName}`

            });


        } catch (error) {



            console.error(
                'strat-add error:',
                error
            );




            if (interaction.replied) {

                return;

            }




            await interaction.reply({

                content:
                    '❌ Failed to connect to the backend.',

                ephemeral: true

            });

        }

    }

};
