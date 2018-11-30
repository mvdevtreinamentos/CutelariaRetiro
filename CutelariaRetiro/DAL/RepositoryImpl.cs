using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CutelariaRetiro.DAL
{
    public class RepositoryImpl<T> where T : class
    {
        private static bool DatabaseVerified { get; set; }

        public CutelariaRetiroEntities Context = null;
        private bool hasTransaction = false;
        private bool antiTrackingEnabled = true;

        private void SetupDB()
        {
            if (DatabaseVerified)
                return;

            if (Context.Database.Exists())
            {
                DatabaseVerified = true;
                return;
            }

            Context.Database.Create();
            DatabaseVerified = true;
        }


        public RepositoryImpl()
        {
            Context = new CutelariaRetiroEntities();
            SetupDB();
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool ExecSQL(string sql, SqlParameter[] parameters = null)
        {
            int retorno = (parameters == null
                ? Context.Database.ExecuteSqlCommand(sql)
                : Context.Database.ExecuteSqlCommand(sql, parameters));

            return (retorno == 1);
        }

        /**
         * *** ATENÇÃO PORRA: PRESTA ATENCAO NA FODA!
         *     OS BLOCOS CATCH QUE NAO TIVER IMPLEMENTADO,
         *     *** NAO EXCUIR ***
         *     
         *     TEM QUE TRATAR AS EXCEPTIONS ESPECÍFICAS DEPOIS!!! ***
         * */
        public void Commit()
        {
            try
            {
                Context.SaveChanges();
                hasTransaction = false;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                string msg = valEx.EntityValidationErrors.FirstOrDefault().ValidationErrors.ToList()[0].ErrorMessage;
                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public T Find(params object[] id)
        {
            try
            {
                return Context.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public void Remove(T entity)
        {
            try
            {
                Context.Set<T>().Remove(entity);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                string msg = valEx.EntityValidationErrors.FirstOrDefault().ValidationErrors.ToList()[0].ErrorMessage;
                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ForceRemove(T entity)
        {
            try
            {
                Context.Set<T>().Attach(entity);
                Context.Entry<T>(entity).State = EntityState.Deleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Save(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void DisableAntiTracking()
        {
            antiTrackingEnabled = false;
            Context.Configuration.ProxyCreationEnabled = false;
        }

        public void EnableAntiTracking()
        {
            antiTrackingEnabled = true;
        }

        public bool HasAntiTracking
        {
            get
            {
                return antiTrackingEnabled;
            }
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> query)
        {
            try
            {
                IQueryable<T> result = null;

                if (antiTrackingEnabled)
                    result = Context.Set<T>().Where(query).AsNoTracking();
                else
                {
                    Context.Configuration.ProxyCreationEnabled = true;
                    result = Context.Set<T>().Where(query);
                }

                return result;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public void Update(T entity)
        {
             Context.Entry<T>(entity).State = EntityState.Modified;
        }
    }
}
