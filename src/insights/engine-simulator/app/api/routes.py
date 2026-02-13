from fastapi import APIRouter, Request
from app.models.input import WfEngineInput
from app.models.output import WfEngineOutput
from app.services.engine import WeatherInsightsEngineSimulator
import gzip

router = APIRouter()
engine = WeatherInsightsEngineSimulator()


@router.post("/forecast", response_model=WfEngineOutput)
async def forecast(request: Request):
    content_encoding = request.headers.get("Content-Encoding", "")
    
    body = await request.body()
    
    if "gzip" in content_encoding:
        body = gzip.decompress(body)
    
    input = WfEngineInput.model_validate_json(body)
    return engine.call(input)
