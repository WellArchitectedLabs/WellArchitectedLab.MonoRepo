import gzip
from fastapi import Request

async def decompress_gzip(request: Request, call_next):
    if "gzip" in request.headers.get("Content-Encoding", ""):
        body = await request.body()
        decompressed = gzip.decompress(body)
        request._body = decompressed
    return await call_next(request)