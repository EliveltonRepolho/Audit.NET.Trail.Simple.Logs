
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
dotnet ef database update --context EmployeeDbContext
dotnet ef database update --context AuditDbContext
```

* Creating an employee
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

* Updating an employee
```bash
curl -X 'PUT' \
  'http://localhost:5000/employee/1' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "employeeId": "1",
  "name": "name 1 - new",
  "citizenship": "citizenship 1"
}'
```

#### Started project from
* https://github.com/executeautomation/ASPNETCore_MinimalAPI
