from telegram.ext import (ApplicationBuilder, CommandHandler, 
                          MessageHandler, filters, 
                          ConversationHandler,
                          )
# from telegram_inline_keyboard_builder import InlineKeyboardBuilder
from config import TOKEN
from states import *
from handlers.start import start_command
from handlers.helpers import cancel_command,help_command,error
from handlers.strats import strats_command, handle_strat_input, handle_operator



# inline keyboard (no logic yet)
"""
maps_keyboard = (
    InlineKeyboardBuilder(buttons_per_row=5)
    .add_callback_button(all_maps[10],"Coastline")
    .add_callback_button(all_maps[11],"Consulate")
    .add_callback_button(all_maps[4],"Club house")
    .build()
)
"""




def main():
    print("starting bot")

    app = (ApplicationBuilder()
           .token(TOKEN)
           .read_timeout(10)
           .write_timeout(10)
           .concurrent_updates(True)
           .build()
           )

    conv_handler = ConversationHandler(
        entry_points=[
            # CommandHandler("start", start_command),
            CommandHandler("strats", strats_command)
            ],
        states={
            # WAITING_START: [
            #     MessageHandler(filters.TEXT & ~filters.COMMAND,
            #                    handle_start_input)
            # ],
            WAITING_FOR_MAP_NAME: [
                MessageHandler(filters.TEXT & ~filters.COMMAND, 
                               handle_strat_input)
            ],
            WAITING_FOR_OPERATOR: [
                MessageHandler(filters.TEXT & ~filters.COMMAND, 
                               handle_operator)
            ]
        },
        fallbacks=[CommandHandler("cancel", cancel_command)]
    )

    # messages
    app.add_handler(conv_handler)


    # commands
    app.add_handler(CommandHandler("start", start_command))
    app.add_handler(CommandHandler("help", help_command))
    app.add_handler(CommandHandler("strats", strats_command))


    # errors
    app.add_error_handler(error)

    print("Polling...")
    
    app.run_polling(poll_interval=3)




if __name__ == "__main__":
    main()
