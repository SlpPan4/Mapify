from telegram import Update, ReplyKeyboardMarkup, ReplyKeyboardRemove
from telegram.ext import (ApplicationBuilder, CommandHandler, 
                          MessageHandler, filters, 
                          ContextTypes, CallbackContext,
                          CallbackQueryHandler,ConversationHandler,
                          )
from telegram_inline_keyboard_builder import InlineKeyboardBuilder
import httpx

TOKEN = "8593159452:AAGOQg2uUfnw9dFJ7TZmwoxaR56i6L7U4WE"
BOT_USERNAME = "@mapifyy_bot"
URL = "http://localhost:5000/api/"


# change this to a database later
all_maps = ["calypso","border","kafe","chalet","clubhouse",
            "bank","lair"," nighthaven"," emerald", "oregon",
            "coastline","consulate","fortress","kanal","outback","villa"]



# MENU, OPTION1, OPTION2, OPTION3 = range(4) 
WAITING_FOR_MAP_NAME, WAITING_FOR_STRAT_NUMBER = range(2)


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

# reply keyboard
maps_keyboard = [[all_maps[10],all_maps[11],all_maps[4]]]




async def start_command(update: Update, context: CallbackContext) -> int:
    if update.message:
        await update.message.reply_text("start!!")

    

async def cancel_command(update: Update, context: CallbackContext) -> int:
    await update.message.reply_text("Operation cancelled.")
    return ConversationHandler.END




async def help_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.message:
        await update.message.reply_text("watafak")

async def strats_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    
    # old logic
    """
    args = context.args

    if not args:
        await update.message.reply_text("")
        return
    
    map = args[0]

    try:
        map_name = str(map)
        await get_strat(update,map_name)

    except ValueError:
        await update.message.reply_text("Provide a valid map name")
    """

    await update.message.reply_text(
        "Choose the name of the map for strategy",

        reply_markup=ReplyKeyboardMarkup(
            maps_keyboard, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the map",
            resize_keyboard=True
        )

    )
    return WAITING_FOR_MAP_NAME



async def handle_strat_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip().lower()

    print(user_input)
    
    if user_input == "cancel":
        await update.message.reply_text("Operation cancelled.")
        return ConversationHandler.END

    if user_input in all_maps:
        print("map in the list")
        return await get_strat(update,user_input, context)
    
    
    await update.message.reply_text("Provide a valid map name")
    return WAITING_FOR_MAP_NAME



async def get_strat(update, map_name: str, context: ContextTypes.DEFAULT_TYPE):

    async with httpx.AsyncClient(timeout=10) as client:
        # get map id
        m = await client.get(URL+"strats/maps/byname/"+map_name)
        map_id = m.json()
        
        # get all strats
        r = await client.get(URL+"strats")
        response = r.json()  
    
    items_keyboard = [[]]
    matching_items = [item for item in response if item.get("mapId") == map_id]

    for i in range(len(matching_items)):
        items_keyboard[0].append(str(i+1))


    context.user_data["matching_strats"] = matching_items
    context.user_data["map_name"] = map_name

    await update.message.reply_text(
        f"choose the strat number: from 1 to {len(matching_items)}",
        reply_markup=ReplyKeyboardMarkup(
            items_keyboard, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the number",
            resize_keyboard=True
        )
        )
    
    items_keyboard[0] = []
    
    return WAITING_FOR_STRAT_NUMBER



async def handle_strat_number(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip()
    matching_items = context.user_data.get("matching_strats")
    map_name = context.user_data.get("map_name")

    try:
        strat_number = int(user_input)

        if not matching_items or strat_number < 1:
            raise ValueError
        
        strat = matching_items[strat_number-1]
        
        
        
        message = f"""
    📌 <b>{strat['name']}</b>

    {strat['description'] or 'No description available.'}

    🔗 Video: {strat['videoUrl']}
    🗺️ Map Name: {map_name}
            """.strip()

        await update.message.reply_text(message, parse_mode="HTML")


    except ValueError:
        await update.message.reply_text("Please select a number between 1 and {len(matching_items)}")
        return WAITING_FOR_STRAT_NUMBER
    
    context.user_data.clear()
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
        entry_points=[CommandHandler("strats", strats_command)],
        states={
            WAITING_FOR_MAP_NAME: [
                MessageHandler(filters.TEXT & ~filters.COMMAND, 
                               handle_strat_input)
            ],
            WAITING_FOR_STRAT_NUMBER: [
                MessageHandler(filters.TEXT & ~filters.COMMAND, 
                               handle_strat_number)
            ]
        },
        fallbacks=[CommandHandler("cancel", cancel_command)]
    )

    # commands
    app.add_handler(conv_handler)
    app.add_handler(CommandHandler("start", start_command))
    app.add_handler(CommandHandler("help", help_command))
    app.add_handler(CommandHandler("strats", strats_command))

    # messages
    # app.add_handler(MessageHandler(filters.TEXT, handle_message))

    # errors
    app.add_error_handler(error)

    print("Polling...")
    
    app.run_polling(poll_interval=3)




if __name__ == "__main__":
    main()