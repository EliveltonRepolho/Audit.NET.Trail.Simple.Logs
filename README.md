
## Creating Database
* Start Database
`docker-compose up -d`

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
  "employeeId": "1234",
  "name": "name 1234",
  "citizenship": "citizenship 1234"
}'
```