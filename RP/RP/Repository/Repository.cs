using RP.Context;
using RP.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RP.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        //https://code.msdn.microsoft.com/AspNet-MVC-Code-First-6c3253d2/sourcecode?fileId=157870&pathId=469031978
        private DataContext db;
        private DbSet<T> dbSet;

        public Repository()
        {
            db = new DataContext();
            dbSet = db.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public T GetById(object Id)
        {
            return dbSet.Find(Id);
        }

        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }
        public void Update(T obj)
        {
            db.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object Id)
        {
            T getObjById = dbSet.Find(Id);
            dbSet.Remove(getObjById);
        }
        public void Save()
        {
            db.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }


    }
}