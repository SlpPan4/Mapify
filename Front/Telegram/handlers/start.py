from telegram import Update
from telegram.ext import ContextTypes 
import keyboards
from states import *


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
            reply_markup=keyboards.start_keyboard()
            )




async def start_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
        await show_main_menu(update)
        return WAITING_START


