from config import URL
from .client import client


async def post_strat(user_data):
    payload = {
        "id": user_data["id"],
        "name": user_data["name"],
        "videoUrl": user_data["videoUrl"],
        "mapId": user_data["mapId"],
        "operatorIds": user_data["operatorIds"],
        "description": user_data["description"],
    }

    response = await client.post(URL+"submissions/strats",
                           json=payload)

    print(response.status_code)
    print(response.json())
