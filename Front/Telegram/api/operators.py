from config import URL
from .client import client

async def get_all():
       
        response = await client.get(URL+"operators")
        return response.json()

async def get_strat_byoperator(operator_id):
    
    response = await client.get(URL+"strats/byoperator/"+str(operator_id[0]))

    return response.json()

