from telegram import Update, ReplyKeyboardMarkup 
from telegram.ext import ContextTypes,ConversationHandler


from .helpers import cancel_command
from .start import show_main_menu
import keyboards
from states import *
from api import operators

async def strats_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    
    await update.message.reply_text(
        "Choose the name of the map for strategy",

        reply_markup=keyboards.maps_keyboard()
    )
    return WAITING_FOR_MAP_NAME



async def handle_strat_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip().lower()

    print(user_input)
    

    
    if user_input == "cancel":
        return await cancel_command(update,context)

    if user_input in keyboards.all_maps:
        print("map in the list")

        context.user_data["map_name"] = user_input
        return await get_operator(update, context)
    
    
    await update.message.reply_text("Provide a valid map name")
    return WAITING_FOR_MAP_NAME



async def get_operator(update, context: ContextTypes.DEFAULT_TYPE):

    all_operators = await operators.get_all()


    items_keyboard = [[]]
   
    matching_items = [item.get("name") for item in all_operators]

    for i in range(len(matching_items)):
        items_keyboard[0].append(matching_items[i])
    

    context.user_data["all_operators"] = all_operators
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
    map_name = context.user_data.get("map_name")
    all_operators = context.user_data.get("all_operators")
    print(map_name)



    operator_id = [item.get("id") for item in all_operators if item.get("name") == operator_name]


    strat = await operators.get_strat_byoperator(operator_id)
 
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


