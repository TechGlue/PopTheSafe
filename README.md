# PopTheSafe

## Requirements
- Docker
- Docker Compose 

## Overview
![MySafeStateMachine](./Docs/MySafe.png)

## Running application containers

### Unix based environments
```bash
cd safe
make all
```

### Windows 
```powershell
cd safe
docker-compose build
docker-compose up -d
```

Once containers are done building navigate to http://localhost:4200/
