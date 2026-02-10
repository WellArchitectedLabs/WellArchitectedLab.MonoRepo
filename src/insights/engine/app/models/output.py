from pydantic import BaseModel
from datetime import datetime
from typing import Dict


class WfEngineInsightOutput(BaseModel):
    temperature: float
    wind_speed: float
    precipitation: float


class WfEngineOutput(BaseModel):
    city_id: int
    reference_date: datetime
    per_hour_prediction: Dict[datetime, WfEngineInsightOutput]
