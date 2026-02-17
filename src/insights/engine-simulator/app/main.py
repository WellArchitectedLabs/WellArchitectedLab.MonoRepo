from fastapi import FastAPI
from app.api.routes import router
from app.api.middleware import decompress_gzip

app = FastAPI(title="Weather Insights Engine Simulator")

app.middleware("http")(decompress_gzip)

app.include_router(router)