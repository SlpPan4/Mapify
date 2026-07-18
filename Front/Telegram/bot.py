from telegram import Update, ReplyKeyboardMarkup, ReplyKeyboardRemove
from telegram.ext import (ApplicationBuilder, CommandHandler, 
                          MessageHandler, filters, 
                          ContextTypes, CallbackContext,
                          CallbackQueryHandler,ConversationHandler,
                          )
# from telegram_inline_keyboard_builder import InlineKeyboardBuilder
import httpx

TOKEN = 'HUISOS' 
BOT_USERNAME = "@mapifyy_bot"
URL = "http://localhost:5000/api/"


# change this to a database later
all_maps = ["calypso","border","kafe","chalet","clubhouse",
            "bank","lair"," nighthaven"," emerald", "oregon",
            "coastline","consulate","fortress","kanal","outback","villa"]



WAITING_FOR_MAP_NAME, WAITING_FOR_OPERATOR, WAITING_START = range(3)


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

# reply keyboards
maps_keyboard = [[all_maps[10],all_maps[11],all_maps[4],"cancel"]]
start_keyboard = [["/strats","/help"]]


async def show_main_menu(update:Update):
    message = f"""
    🌈 <b>Welcome to Mapify bot!</b>

    This bot allows you to learn and create strategies for <b>Rainbow Six Siege</b>

🛑 Please, select one of the available options to continue
    /strats -- available strategies for specific maps
    /help -- FAQ
            """.strip()
    
    if update.message:
        await update.message.reply_text(
            message, 
            parse_mode="HTML",
            reply_markup=ReplyKeyboardMarkup(
                start_keyboard, 
                one_time_keyboard=True,
                input_field_placeholder="Select one of the available options to continue",
                resize_keyboard=True
            )
        )




async def start_command(update: Update, context: CallbackContext):
        await show_main_menu(update)
        return WAITING_START




async def cancel_command(update: Update, context: CallbackContext) -> int:
    await update.message.reply_text("Operation cancelled.")
    context.user_data.clear()

    await start_command(update,context)

    return ConversationHandler.END




async def help_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    message = """
    /strats -- available strategies for specific maps
    """.strip()
    if update.message:
        await update.message.reply_text(
            message,
            reply_markup=ReplyKeyboardMarkup(
                start_keyboard, 
                one_time_keyboard=True,
                input_field_placeholder="Select one of the available options to continue",
                resize_keyboard=True
            )
        )



async def strats_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    
    await update.message.reply_text(
        "Choose the name of the map for strategy",

        reply_markup=ReplyKeyboardMarkup(
            maps_keyboard, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the map or cancel the command",
            resize_keyboard=True
        )

    )
    return WAITING_FOR_MAP_NAME



async def handle_strat_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip().lower()

    print(user_input)
    

    
    if user_input == "cancel":
        return await cancel_command(update,context)

    if user_input in all_maps:
        print("map in the list")

        context.user_data["map_name"] = user_input
        return await get_operator(update, context)
    
    
    await update.message.reply_text("Provide a valid map name")
    return WAITING_FOR_MAP_NAME



async def get_operator(update, context: ContextTypes.DEFAULT_TYPE):

    async with httpx.AsyncClient(timeout=10) as client:
       
        o = await client.get(URL+"operators")
        operators = o.json()




    items_keyboard = [[]]
   
    matching_items = [item.get("name") for item in operators]

    for i in range(len(matching_items)):
        items_keyboard[0].append(matching_items[i])
    


    context.user_data["operator"] = matching_items
    await update.message.reply_text(
        f"pick operator",
        reply_markup=ReplyKeyboardMarkup(
            items_keyboard, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the number",
            resize_keyboard=True
        )
        )
    
    items_keyboard[0] = []
    
    return WAITING_FOR_OPERATOR



async def handle_operator(update: Update, context: ContextTypes.DEFAULT_TYPE):
    operator_name = update.message.text.strip()
    # operator = context.user_data.get("operator")
    map_name = context.user_data.get("map_name")

    print(map_name)

    async with httpx.AsyncClient(timeout=10) as client:
        # get map id
        m = await client.get(URL+"strats/maps/byname/"+map_name)
        map_id = m.json()
      
        o = await client.get(URL+"operators")
        all_operators = o.json()

    operator_id = [item.get("id") for item in all_operators if item.get("name") == operator_name]


    async with httpx.AsyncClient(timeout=10) as client:
        s = await client.get(URL+"strats/byoperator/"+str(operator_id[0]))
        strat = s.json()
    
    strategy = strat[0]

    try:

        message = f"""
    📌 <b>{strategy['name']}</b>

    {strategy['description'] or 'No description available.'}

    🔗 Video: {strategy['videoUrl']}
    🗺️ Map Name: {map_name}
            """.strip()

        await update.message.reply_text(message, parse_mode="HTML")


    except ValueError:
        await update.message.reply_text("Please select a number between 1 and {len(matching_items)}")



        return WAITING_FOR_OPERATOR

    
    context.user_data.clear()

    await show_main_menu(update)
    return ConversationHandler.END






async def error(update: Update, context: ContextTypes.DEFAULT_TYPE):
    print(f"Update {update} caused error: {context.error}")


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
