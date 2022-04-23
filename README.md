### DotNet EntityFramework with [Audit.NET](https://github.com/thepirat000/Audit.NET) Example

This example was intended to help understanding topic related to this issue: https://github.com/thepirat000/Audit.NET/issues/487

The use case here is using [Audit.NET](https://github.com/thepirat000/Audit.NET) to create a single final table with Audit changes, 
and marking the original Entity with `CreatedDate/ModifiedDate` or any other needed (e.g: `CreatedBy/ModifiedBy`)

#### Briefly explanation about config: 
* [AuditableDbContext](MiniDemo/Model/AuditableDbContext.cs) and [EmployeeDbContext](MiniDemo/Model/EmployeeDbContext.cs): Isolated DBContext for Auditing and Other Business related Models;
* [AuditTrail](MiniDemo/Model/AuditTrail.cs): Changes log table, all tables will be logged here, being able to track Before and After changes;
* [AuditEntity](MiniDemo/Model/AuditEntity.cs): Abstract class to inherit Fields like `CreatedDate/ModifiedDate`, our config will fill this fields automatically.

### How to run it:

* Printing my dotnet cli version for reference:
```bash
$ dotnet --version
6.0.201
```

### Creating Database
* Start Database
`docker-compose up -d`

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
<details>
  <summary>Output example</summary>

## Heading
```json
{
    "Table": "Employee",
    "Name": "Employee",
    "PrimaryKey": {
        "EmployeeId": "1"
    },
    "Action": "Insert",
    "ColumnValues": {
        "EmployeeId": "1",
        "Citizenship": "citizenship 1",
        "Name": "name 1"
    },
    "Valid": true
}
```
</details>

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
<details>
  <summary>Output example</summary>

## Heading
```json
{
  "Table": "Employee",
  "Name": "Employee",
  "PrimaryKey": {
    "EmployeeId": "1"
  },
  "Action": "Update",
  "Changes": [
    {
      "ColumnName": "Citizenship",
      "OriginalValue": "citizenship 1",
      "NewValue": "citizenship 1"
    },
    {
      "ColumnName": "Name",
      "OriginalValue": "name 1",
      "NewValue": "name 1 - new"
    }
  ],
  "ColumnValues": {
    "EmployeeId": "1",
    "Citizenship": "citizenship 1",
    "Name": "name 1 - new"
  },
  "Valid": true
}
```
</details>

#### Started project from
* https://github.com/executeautomation/ASPNETCore_MinimalAPI
