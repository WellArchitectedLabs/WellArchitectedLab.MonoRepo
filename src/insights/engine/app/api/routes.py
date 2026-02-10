from fastapi import APIRouter
from app.models.input import WfEngineInput
from app.models.output import WfEngineOutput
from app.services.engine import WeatherInsightsEngineSimulator

router = APIRouter()
engine = WeatherInsightsEngineSimulator()


@router.post("/forecast", response_model=WfEngineOutput)
def forecast(input: WfEngineInput):
    return engine.call(input)
