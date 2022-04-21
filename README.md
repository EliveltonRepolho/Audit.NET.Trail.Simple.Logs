
## Creating Database
* Start Database
`docker-compose up -d`

```bash
$ dotnet --version
6.0.201
```

* Create DB Structure
```bash
cd MiniDemo
dotnet ef database update
```

```bash
curl -X 'POST' \
  'http://localhost:5000/employee' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "employeeId": "1",
  "name": "name 1",
  "citizenship": "citizenship 1"
}'
```

#### Started project from
* https://github.com/executeautomation/ASPNETCore_MinimalAPI
