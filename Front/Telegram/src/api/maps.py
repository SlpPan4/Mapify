from .client import client
from config import URL


async def get_map(map_name):
    response = await client.get(URL+"strats/maps/byname/"+map_name)
    output = response.json()
    return output.get("data")


async def get_map_name(map_id):
    response = await client.get(URL+"strats/maps/"+map_id)
    output = response.json()
    return output.get("data").get("name")
