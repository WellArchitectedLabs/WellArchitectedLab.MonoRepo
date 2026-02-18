############################################
# Configuration
############################################

SRC := .
MASTER := $(SRC)/master-data
INSIGHTS := $(SRC)/insights
COMPOSE := docker compose

STAMP_DIR := .make-stamps
MASTER_DATA_MIGRATE_STAMP := $(STAMP_DIR)/master_data_migrated
INSIGHTS_DATA_MIGRATE_STAMP := $(STAMP_DIR)/insights_data_migrated
CITIES_STAMP := $(STAMP_DIR)/cities_imported
WEATHER_STAMP := $(STAMP_DIR)/weather_imported

APP_NAME := weatherly

############################################
# Colors
############################################

GREEN := \033[0;32m
BLUE := \033[0;34m
YELLOW := \033[1;33m
RESET := \033[0m

############################################
# Internal helpers
############################################

$(STAMP_DIR):
	@mkdir -p $(STAMP_DIR)

############################################
# Migration
############################################

$(MASTER_DATA_MIGRATE_STAMP): | $(STAMP_DIR)
	@printf "$(BLUE)Running migration...$(RESET)\n"
	cd $(MASTER)/data && $(COMPOSE) -p $(APP_NAME) up --build masterdata-postgres-migrate
	cd $(MASTER)/data && $(COMPOSE) down
	@touch $(MASTER_DATA_MIGRATE_STAMP)
	@printf "$(GREEN)Migration completed.$(RESET)\n"

$(INSIGHTS_DATA_MIGRATE_STAMP): | $(STAMP_DIR)
	@printf "$(BLUE)Running migration...$(RESET)\n"
	cd $(INSIGHTS)/data && $(COMPOSE) -p $(APP_NAME) up --build insights-postgres-migrate
	cd $(INSIGHTS)/data && $(COMPOSE) down
	@touch $(INSIGHTS_DATA_MIGRATE_STAMP)
	@printf "$(GREEN)Migration completed.$(RESET)\n"

master-data-postgres-migrate: $(MASTER_DATA_MIGRATE_STAMP)
insights-data-postgres-migrate: $(INSIGHTS_DATA_MIGRATE_STAMP)

############################################
# Cities Import (idempotent)
############################################

$(CITIES_STAMP): $(MASTER_DATA_MIGRATE_STAMP)
	@printf "$(BLUE)Running cities import...$(RESET)\n"
	cd $(MASTER)/staging && $(COMPOSE) -p $(APP_NAME) run --rm cities-import
	cd $(MASTER)/staging && $(COMPOSE) down
	@touch $(CITIES_STAMP)
	@printf "$(GREEN)Cities import completed.$(RESET)\n"

import-cities: $(CITIES_STAMP)

############################################
# Weather Import (idempotent)
############################################

$(WEATHER_STAMP): $(CITIES_STAMP)
	@printf "$(BLUE)Running weather actuals import...$(RESET)\n"
	cd $(MASTER)/staging && $(COMPOSE) -p $(APP_NAME) run --rm wf-actuals-import
	cd $(MASTER)/staging && $(COMPOSE) down
	@touch $(WEATHER_STAMP)
	@printf "$(GREEN)Weather import completed.$(RESET)\n"

wf-actuals-import: $(WEATHER_STAMP)

############################################
# Services
############################################

master-data-service: wf-actuals-import
	@printf "$(BLUE)Starting master data service...$(RESET)\n"
	cd $(MASTER)/service && $(COMPOSE) -p $(APP_NAME) up --build -d master-data-service
	@printf "$(GREEN)Master data service started.$(RESET)\n"

insights-engine-simulator:
	@printf "$(BLUE)Starting engine simulator...$(RESET)\n"
	cd $(INSIGHTS)/engine-simulator && $(COMPOSE) -p $(APP_NAME) up -d engine-proxy
	@printf "$(GREEN)Engine simulator running.$(RESET)\n"

insights-collector: master-data-service insights-data-postgres-migrate insights-engine-simulator
	@printf "$(BLUE)Starting insights collector...$(RESET)\n"
	cd $(INSIGHTS)/collector && $(COMPOSE) -p $(APP_NAME) up --build -d insights-collector
	@printf "$(GREEN)Insights collector started.$(RESET)\n"

insights-bff: insights-collector
	@printf "$(BLUE)Starting insights BFF...$(RESET)\n"
	cd $(INSIGHTS)/bff && $(COMPOSE) -p $(APP_NAME) up --build -d insights-bff
	@printf "$(GREEN)Insights BFF started.$(RESET)\n"


############################################
# Utilities
############################################

# Force re-run of imports
reset-imports:
	@printf "$(YELLOW)Resetting import state...$(RESET)\n"
	@rm -rf $(STAMP_DIR)

down:
	cd $(MASTER)/service && $(COMPOSE) -p $(APP_NAME) down || true
	cd $(INSIGHTS)/collector && $(COMPOSE) -p $(APP_NAME) down || true
	cd $(INSIGHTS)/engine-simulator && $(COMPOSE) -p $(APP_NAME) down || true
	cd $(INSIGHTS)/bff && $(COMPOSE) -p $(APP_NAME) down || true

clean: down
	cd $(MASTER)/data && $(COMPOSE) -p $(APP_NAME) down -v || true
	cd $(MASTER)/staging && $(COMPOSE) -p $(APP_NAME) down -v || true
	cd $(MASTER)/service && $(COMPOSE) -p $(APP_NAME) down -v || true
	cd $(INSIGHTS)/collector && $(COMPOSE) -p $(APP_NAME) down -v || true
	cd $(INSIGHTS)/engine-simulator && $(COMPOSE) -p $(APP_NAME) down -v || true
	cd $(INSIGHTS)/bff && $(COMPOSE) -p $(APP_NAME) down -v || true
	@rm -rf $(STAMP_DIR)