from telegram import Update
from telegram.ext import(ContextTypes,ConversationHandler) 


import keyboards
from .start import start_command


async def cancel_command(update: Update, context: ContextTypes.DEFAULT_TYPE) -> int:
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
            reply_markup=keyboards.start_keyboard()
        )

async def error(update: Update, context: ContextTypes.DEFAULT_TYPE):
    print(f"Update {update} caused error: {context.error}")


