from .client import client
from config import URL


async def get_map(map_name):
    response = await client.get(URL+"strats/maps/byname/"+map_name)
    
    return response.json()

