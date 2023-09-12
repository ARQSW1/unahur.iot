# Docker compose

Para iniciar solo los backing services (sqlserver, mongo y rabbit)

```bash
docker-compose --project-name iot up -d
```
Para detener todo

```bash
docker-compose --project-name iot kill
docker-compose --project-name iot rm
```

Para compilar los contenedores

```bash
docker-compose --profile dev build
# OPCION SIN CACHE
docker-compose --profile dev build --no-cache
```

Para iniciar tambien el desarrollo en los contenedores compilados

```bash
docker-compose --project-name iot --profile dev up -d
docker-compose --project-name iot --profile dev up -d
```