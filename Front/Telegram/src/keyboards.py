from telegram import ReplyKeyboardMarkup

all_maps = ["calypso","border","kafe","chalet","clubhouse",
            "bank","lair"," nighthaven"," emerald", "oregon",
            "coastline","consulate","fortress","kanal","outback","villa"]


maps_keyboard_buttons= [[all_maps[10],all_maps[11],all_maps[4],"cancel"]]
start_keyboard_buttons = [["/strats","/your_strat","/help"]]

def start_keyboard():

    return ReplyKeyboardMarkup(
                start_keyboard_buttons, 
                one_time_keyboard=True,
                input_field_placeholder="Select one of the available options to continue",
                resize_keyboard=True
    )

def maps_keyboard():
    return ReplyKeyboardMarkup(
            maps_keyboard_buttons, 
            one_time_keyboard=True,
            input_field_placeholder="Choose the map or cancel the command",
            resize_keyboard=True
        )

