import {
    Events
} from 'discord.js';

import chalk from 'chalk';

export default {

    name: Events.ClientReady,

    once: true,

    async execute(readyClient) {

        console.log(

            chalk
                .rgb(255, 255, 255)
                .bgGreenBright
                .bold(
                    `{Event:ready} - ${readyClient.user.tag}!`
                )

        );

    }

};
