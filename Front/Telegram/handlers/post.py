from telegram import Update, ReplyKeyboardMarkup 
from telegram.ext import ContextTypes,ConversationHandler


from .helpers import cancel_command
from .start import show_main_menu
import keyboards
from states import *
from api import operators,maps,public_post
from urllib.parse import urlparse

async def post_strat_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    
    await update.message.reply_text(
        "Specify a map for your strategy",

        reply_markup=keyboards.maps_keyboard()
    )
    return WAITING_FOR_MAP_POST_NAME

async def handle_post_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip().lower()

    print(user_input)
    

    
    if user_input == "cancel":
        return await cancel_command(update,context)

    if user_input in keyboards.all_maps:
        print("map in the list")
        context.user_data["id"] = 1
        context.user_data["mapId"] = await maps.get_map(user_input)
        return await get_post_operator(update, context)
    
    
    await update.message.reply_text("Provide a valid map name")
    return WAITING_FOR_MAP_POST_NAME



async def get_post_operator(update, context: ContextTypes.DEFAULT_TYPE):

    all_operators = await operators.get_all()


    items_keyboard = [[]]
   
    matching_items = [item.get("name") for item in all_operators]

    for i in range(len(matching_items)):
        items_keyboard[0].append(matching_items[i])
    
    items_keyboard[0].append("cancel")
    # context.user_data["all_operators"] = all_operators
    await update.message.reply_text(
        f"Pick operator for your strat",
        reply_markup=ReplyKeyboardMarkup(
            items_keyboard, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the operator",
            resize_keyboard=True
        )
        )
    
    items_keyboard[0] = []
    
    return WAITING_FOR_POST_OPERATOR



async def handle_post_operator(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip()

    if user_input == "cancel":
        return await cancel_command(update,context)

    all_operators = await operators.get_all()
    operators_names = [item.get("name") for item in all_operators]

    if user_input in operators_names:
        operator_id = [item.get("id") for item in all_operators if item.get("name") == user_input]
        context.user_data["operatorIds"] = []
        context.user_data["operatorIds"].append(operator_id[0])
        print("operator in the list")
        return await get_post_strat_link(update, context)


    await update.message.reply_text("Provide the valid operator name")
    return WAITING_FOR_POST_OPERATOR


async def get_post_strat_link(update: Update, context: ContextTypes.DEFAULT_TYPE):

    await update.message.reply_text(
        "Paste the link to your strategy. Awailable websites: YouTube, Tiktok, Instagram.",
    )
    return WAITING_FOR_STRAT_LINK_POST


async def handle_post_strat_link(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip()

    if user_input == "cancel":
        return await cancel_command(update,context)


    parsed = urlparse(user_input)

    allowed_domains = {
        "youtube.com",
        "www.youtube.com",
        "youtu.be",
        "tiktok.com",
        "www.tiktok.com",
        "instagram.com",
        "www.instagram.com",
    }


    if parsed.scheme in ("http", "https") and parsed.netloc in allowed_domains:
        print("Valid link")
        context.user_data["videoUrl"] = user_input
        return await get_post_strat_name(update,context)

    else:
        print("Invalid link")
        await update.message.reply_text("Provide the valid link")

        return WAITING_FOR_STRAT_LINK_POST



async def get_post_strat_name(update: Update, context: ContextTypes.DEFAULT_TYPE):

    answers = [["continue","cancel"]]

    await update.message.reply_text(
            "Write the name for your strategy. Otherwise press continue or cancel operation.",
            reply_markup=ReplyKeyboardMarkup(
            answers, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the option",
            resize_keyboard=True
        )
        )

    return WAITING_FOR_POST_NAME



async def handle_post_strat_name(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip()

    if user_input == "cancel":
        return await cancel_command(update,context)
    elif user_input == "continue":
        context.user_data["name"] = "-"
        return await get_post_description(update,context)
    else:
        context.user_data["name"] = user_input
        return await get_post_description(update,context)


    return WAITING_FOR_POST_NAME


async def get_post_description(update: Update, context: ContextTypes.DEFAULT_TYPE):

    answers = [["continue","cancel"]]

    await update.message.reply_text(
            "Write the description for your strategy. Otherwise press continue or cancel operation.",
            reply_markup=ReplyKeyboardMarkup(
            answers, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the option",
            resize_keyboard=True
        )
        )

    return WAITING_FOR_POST_DESCRIPTION



async def handle_post_description(update: Update, context: ContextTypes.DEFAULT_TYPE):
    user_input = update.message.text.strip()

    if user_input == "cancel":
        return await cancel_command(update,context)
    elif user_input == "continue":
        context.user_data["description"] = ""
        return await assemble_post(update,context)
    else:
        context.user_data["description"] = user_input
        return await assemble_post(update,context)


    return WAITING_FOR_POST_DESCRIPTION




async def assemble_post(update: Update, context: ContextTypes.DEFAULT_TYPE):
    print(context.user_data)


    await public_post.post_strat(context.user_data)


    strategy = context.user_data
    map_name = await maps.get_map_name(str(strategy["mapId"]))
    print(map_name)


    message = f"""
    📌 <b>YOUR STRATEGY PREVIEW</b>

    {strategy['description'] or 'No description available.'}

    🔗 Video: {strategy['videoUrl']}
    🗺️ Map Name: {map_name}
            """.strip()

    await update.message.reply_text(message, parse_mode="HTML")


    context.user_data.clear()

    await show_main_menu(update)
    return ConversationHandler.END


