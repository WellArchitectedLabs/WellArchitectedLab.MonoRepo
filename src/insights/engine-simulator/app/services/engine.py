import pandas as pd
from datetime import datetime, time, timedelta
from app.models.input import WfEngineInput
from app.models.output import (
    WfEngineOutput,
    WfEngineInsightOutput,
)
from app.services.forecasting import prophet_forecast, sarimax_forecast


class WeatherInsightsEngineSimulator:
    def call(self, wf_input: WfEngineInput) -> WfEngineOutput:
        combined = {**wf_input.far_history, **wf_input.near_history}

        index = sorted(combined.keys())
        df = pd.DataFrame(
            {
                "temperature": [combined[t].temperature for t in index],
                "wind": [combined[t].wind_speed for t in index],
                "precip": [combined[t].precipitation for t in index],
            },
            index=pd.to_datetime(index),
        )

        horizon = 24

        def forecast_column(col):
            try:
                values = prophet_forecast(df[col], horizon)
            except Exception:
                try:
                    values = sarimax_forecast(df[col], horizon)
                except Exception:
                    values = [float(df[col].mean())] * horizon

            # Final guardrail
            if len(values) != horizon:
                values = (values + [values[-1]])[:horizon]

            return values

        temp_f = forecast_column("temperature")
        wind_f = forecast_column("wind")
        precip_f = forecast_column("precip")

        predictions = {}
        start = datetime.combine(wf_input.reference_date, time.min)

        for i in range(24):
            ts = start + timedelta(hours=i)
            predictions[ts] = WfEngineInsightOutput(
                temperature=float(temp_f[i]),
                wind_speed=float(wind_f[i]),
                precipitation=max(0.0, float(precip_f[i])),
            )

        return WfEngineOutput(
            city_id=wf_input.city_id,
            reference_date=wf_input.reference_date,
            per_hour_prediction=predictions,
        )
