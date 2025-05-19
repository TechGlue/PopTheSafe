all: 
	docker-compose build
	docker-compose up -d

safeservice:
	docker-compose build safe
	docker-compose up -d safe 

safeclient:
	docker-compose build mysafeclient
	docker-compose up -d mysafeclient

dropall:
	if [ -z "$(shell docker ps -a -q)" ]; then \
		echo "No containers to remove"; \
	else \
		docker stop $(shell docker ps -a -q); \
		docker rm $(shell docker ps -a -q); \
	fi
	docker-compose down -v
