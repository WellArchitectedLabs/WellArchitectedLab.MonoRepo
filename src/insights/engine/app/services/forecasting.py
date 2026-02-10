import pandas as pd
import numpy as np
from prophet import Prophet
from statsmodels.tsa.statespace.sarimax import SARIMAX


def prophet_forecast(series: pd.Series, horizon: int) -> pd.Series:
    df = series.reset_index()
    df.columns = ["ds", "y"]

    model = Prophet(
        daily_seasonality=True,
        weekly_seasonality=True,
        yearly_seasonality=False,
    )
    model.fit(df)

    future = model.make_future_dataframe(
        periods=horizon, freq="H", include_history=False
    )
    forecast = model.predict(future)

    return forecast["yhat"].values


def sarimax_forecast(series: pd.Series, horizon: int) -> pd.Series:
    model = SARIMAX(
        series,
        order=(1, 0, 1),
        seasonal_order=(1, 0, 1, 24),
        enforce_stationarity=False,
        enforce_invertibility=False,
    )
    result = model.fit(disp=False)
    return result.forecast(horizon)
