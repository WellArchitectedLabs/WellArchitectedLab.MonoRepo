import { useEffect, useState } from "react";
import WeatherDashboard from "./components/WeatherDashboard";
import { loadConfig } from './configLoader';

export default function App() {
  const [forecasts, setForecasts] = useState([]);
  const [config, setConfig] = useState(null);
  const [err, setErr] = useState(null);

  useEffect(() => {
    loadConfig()
      .then(setConfig)
      .catch(e => setErr(e.message));
  }, []);

  if (err) return <div>Error loading config: {err}</div>;
  if (!config) return <div>Loading config...</div>;

  // ✅ Access backend_url here
  const backendUrl = config.backend_url;

  return (
    <WeatherDashboard backendUrl={backendUrl} />
  );
}
