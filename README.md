Nesse repositório disponibilizo alguns exemplos de como iniciar com observabilidade utilizando OpenTelemetry tendo como backend a stack da Grafana LGTM tendo como base aplicações utilizando dotnet 6.

- (L)oki: Grafana Loki is a set of components that can be composed into a fully featured logging stack.
- (G)rafana: Grafana is the open source analytics & monitoring solution for every database.
- (T)empo: Grafana Tempo is an open source, easy-to-use, and high-scale distributed tracing backend.
- (M)imir: Mimir is an open source, horizontally scalable, highly available, multi-tenant TSDB for long-term storage for Prometheus.

Componentes:
- WebAPI: Uma aplicação do WebAPI;
- Worker: Um aplicação do tipo Worker.

Para testar buildando as aplicações:

```powershell
> docker compose up -d --build
```

Para testar baixando as imagens das aplicações a partir do DockerHub:
```powershell
> docker compose up -d
```

A interface do grafana está disponível em `http://localhost:3000/`


In progress ...