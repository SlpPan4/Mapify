import logging
from telegram import Update, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import (ApplicationBuilder, CommandHandler, 
                          MessageHandler, filters, 
                          ContextTypes, CallbackContext,
                          CallbackQueryHandler,ConversationHandler)

TOKEN = "8593159452:AAGOQg2uUfnw9dFJ7TZmwoxaR56i6L7U4WE"
BOT_USERNAME = "@mapifyy_bot"

MENU, OPTION1, OPTION2 = range(3) 


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
    
#     if query.data == "option1":
#         await query.edit_message_text(text="You selected option 1")
#     elif query.data == "option2":
#          await query.edit_message_text(text="You selected option 2")
#     else:
#         await query.edit_message_text(text="unc")
#         return MENU
    

# async def cancel_command(update: Update, context: CallbackContext) -> int:
#     await update.message.reply_text("Operation cancelled.")
#     return ConversationHandler.END




async def help_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.message:
        await update.message.reply_text("watafak")

async def custom_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.message:
        await update.message.reply_text("custom pidar")

    
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
    #     entry_points=[CommandHandler("start", start_command)],
    #     states={
    #         MENU: [CallbackQueryHandler(button)],
    #         OPTION1: [MessageHandler(filters.TEXT & ~filters.COMMAND, cancel_command)],
    #         OPTION2: [MessageHandler(filters.TEXT & ~filters.COMMAND, cancel_command)],
    #     },
    #     fallbacks=[CommandHandler("start", start_command)]
    # )

    # commands
    # app.add_handler(conv_handler)
    app.add_handler(CommandHandler("start", start_command))
    app.add_handler(CommandHandler("help", help_command))
    app.add_handler(CommandHandler("custom", custom_command))

    # messages
    # app.add_handler(MessageHandler(filters.TEXT, handle_message))

    # errors
    app.add_error_handler(error)

    print("Polling...")
    
    app.run_polling(poll_interval=3)




if __name__ == "__main__":
    main()