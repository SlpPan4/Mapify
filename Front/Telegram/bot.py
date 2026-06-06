from telegram import Update, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import (ApplicationBuilder, CommandHandler, 
                          MessageHandler, filters, 
                          ContextTypes, CallbackContext,
                          CallbackQueryHandler,ConversationHandler)
import httpx

TOKEN = "8593159452:AAGOQg2uUfnw9dFJ7TZmwoxaR56i6L7U4WE"
BOT_USERNAME = "@mapifyy_bot"
URL = "http://localhost:5000/api/"

# MENU, OPTION1, OPTION2, OPTION3 = range(4) 
WAITING_FOR_USER_INPUT = range(1)

async def start_command(update: Update, context: CallbackContext) -> int:
    if update.message:
        await update.message.reply_text("start!!")
#     keyboard = [
#         [InlineKeyboardButton("Option 1", callback_data="option1")],
#         [InlineKeyboardButton("Option 2", callback_data="option2")]
#     ]

#     reply_markup = InlineKeyboardMarkup(keyboard)

#     await update.message.reply_text(
#         "Choose an option:", reply_markup=reply_markup
#     )

#     return MENU


# async def button(update: Update, context: CallbackContext) -> int:
#     query = update.callback_query
#     await query.answer()
    
#     if query.data == "strat1":
#         await get_strat(query,1)
#     elif query.data == "strat2":
#         await get_strat(query,2)
#     elif query.data == "strat3":
#         await get_strat(query,3)
#     else:
#         await query.edit_message_text(text="unc")
#         return MENU
    

async def cancel_command(update: Update, context: CallbackContext) -> int:
    await update.message.reply_text("Operation cancelled.")
    return ConversationHandler.END




async def help_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.message:
        await update.message.reply_text("watafak")

async def strats_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
        # keyboard = [
        #     [InlineKeyboardButton("Strat 1", callback_data="strat1")],
        #     [InlineKeyboardButton("Strat 2", callback_data="strat2")],
        #     [InlineKeyboardButton("Strat 3", callback_data="strat3")],
        # ]

        # reply_markup = InlineKeyboardMarkup(keyboard)

        # await update.message.reply_text(
        #     "Choose an option:", reply_markup=reply_markup
        # )

        # return MENU
    
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

    await update.message.reply_text("Write the name of the map for strategy")
    return WAITING_FOR_USER_INPUT



async def handle_strat_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip().capitalize()

    try:
        map_name = str(user_input)
        await get_strat(update,map_name)
    
    except ValueError:
        await update.message.reply_text("Provide a valid map name")
        return WAITING_FOR_USER_INPUT

    return ConversationHandler.END



async def get_strat(query, map_name: str):

    async with httpx.AsyncClient(timeout=10) as client:
        r = await client.get(URL+"strats")
        response = r.json()  
    
    for item in response:
        if item.get("id") == strat_id:
            strat = item
    
    
    
    
    
    
    message = f"""
📌 <b>{strat['name']}</b>

{strat['description'] or 'No description available.'}

🔗 Video: {strat['videoUrl']}
🗺️ Map ID: {strat['mapId']}
        """.strip()

    await query.message.reply_text(message, parse_mode="HTML")





# def handle_responses(text: str) -> str:
#     process: str = text.lower()

#     if "hello" in process:
#         return "Hi"
    
#     if "dinahu" in process:
#         return "TI CHE AHUEL"
    

#     return "ja nie ponimaju"



# async def handle_message(update: Update, context: ContextTypes.DEFAULT_TYPE):
#     text: str = update.message.text

#     print(f"User: ({update.message.chat.id}): '{text}'")

#     response: str = handle_responses(text)    

#     print(f"Bot: {response}")
    
#     await update.message.reply_text(response)



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

    # conv_handler = ConversationHandler(
    #     entry_points=[CommandHandler("strat", strats_command)],
    #     states={
    #         MENU: [CallbackQueryHandler(button)],
    #         OPTION1: [MessageHandler(filters.TEXT & ~filters.COMMAND, cancel_command)],
    #         OPTION2: [MessageHandler(filters.TEXT & ~filters.COMMAND, cancel_command)],
    #         OPTION3: [MessageHandler(filters.TEXT & ~filters.COMMAND, cancel_command)],
    #     },
    #     fallbacks=[CommandHandler("start", start_command)]
    # )

    # commands
    # app.add_handler(conv_handler)
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