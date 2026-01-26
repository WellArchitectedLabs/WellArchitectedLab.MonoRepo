-- Table: public.cities

-- DROP TABLE IF EXISTS public.cities;

CREATE TABLE IF NOT EXISTS public.cities
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    longitude numeric,
    latitude numeric,
    name text COLLATE pg_catalog."default",
    CONSTRAINT "PK_cities" PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.cities
    OWNER to postgres;

COMMENT ON COLUMN public.cities.id
    IS 'city auto incremented identifier';

COMMENT ON COLUMN public.cities.longitude
    IS 'longitude value from city GPS coordinates';

COMMENT ON COLUMN public.cities.latitude
    IS 'latitude value from city GPS coordinates';

-- Table: public.wf_actuals

-- DROP TABLE IF EXISTS public.wf_actuals;

CREATE TABLE IF NOT EXISTS public.wf_actuals
(
    timestamp_utc timestamp without time zone NOT NULL,
    city_id integer NOT NULL,
    temperature_c numeric NOT NULL,
    wind_speed numeric,
    precipitation numeric,
    CONSTRAINT "PK_wf_actuals" PRIMARY KEY (timestamp_utc, city_id),
    CONSTRAINT "FK_city_id_cities_wf_actuals" FOREIGN KEY (city_id)
        REFERENCES public.cities (id) MATCH SIMPLE
        ON UPDATE NO ACTION 

ALTER TABLE IF EXISTS public.wf_actuals
    OWNER to postgres;

COMMENT ON TABLE public.wf_actuals
    IS 'stores weather forecasts actuals';