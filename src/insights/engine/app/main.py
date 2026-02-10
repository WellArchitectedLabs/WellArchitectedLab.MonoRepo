from fastapi import FastAPI
from app.api.routes import router

app = FastAPI(title="Weather Insights Engine Simulator")
app.include_router(router)
