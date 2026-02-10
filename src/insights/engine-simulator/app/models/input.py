from pydantic import BaseModel
from datetime import datetime
from typing import Dict


class WfEngineInputActualItem(BaseModel):
    temperature: float
    wind_speed: float
    precipitation: float


class WfEngineInput(BaseModel):
    city_id: int
    reference_date: datetime
    far_history: Dict[datetime, WfEngineInputActualItem]
    near_history: Dict[datetime, WfEngineInputActualItem]
