from fastapi import APIRouter, Request
from app.models.input import WfEngineInput
from app.models.output import WfEngineOutput
from app.services.engine import WeatherInsightsEngineSimulator
import gzip

router = APIRouter()
engine = WeatherInsightsEngineSimulator()


@router.post("/forecast", response_model=WfEngineOutput)
async def forecast(input: WfEngineInput):
    return engine.call(input)

@router.get("/ping")
async def ping_check():
    return {"status": "healthy"}

@router.get("/ready")
async def ready_check():
    return {"status": "healthy"}