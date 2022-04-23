using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniDemo.Model
{
    public class DataRepository : IDataRepository
    {
        private readonly EmployeeDbContext db;

        public DataRepository(EmployeeDbContext db)
        {
            this.db = db;
        }

        public List<Employee> GetEmployees() => db.Employee.ToList();

        public Employee PutEmployee(Employee employee)
        {
            
            var existing = GetEmployeeById(employee.EmployeeId);

            if (existing == null) {
                throw new Exception("EmployeeId not found");
            }

            existing.Citizenship = employee.Citizenship;
            existing.Name = employee.Name;

            db.Employee.Update(existing);
            db.SaveChanges();
            
            return GetEmployeeById(employee.EmployeeId);
        }

        public List<Employee> AddEmployee(Employee employee)
        {
            db.Employee.Add(employee);
            db.SaveChanges();
            return db.Employee.ToList();
        }

        public Employee GetEmployeeById(string Id)
        {
            return db.Employee.Where(x => x.EmployeeId == Id).FirstOrDefault();
        }

    }
}
