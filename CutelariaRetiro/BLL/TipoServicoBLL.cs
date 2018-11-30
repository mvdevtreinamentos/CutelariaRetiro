using CutelariaRetiro.DAL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.BLL
{
    public class TipoServicoBLL
    {
        private TipoServicoDAL db = null;

        public TipoServicoBLL()
        {
            db = new TipoServicoDAL();
        }

        public void Save(TipoServico tipo)
        {
            if (db.Find(tipo.Id) == null)
                db.Save(tipo);
            else
                db.Update(tipo);

            db.Commit();
        }

        public TipoServico Find(int id)
        {
            return db.Find(id);
        }

        public void Remove(int id)
        {
            var tipo = Find(id);
            db.Remove(tipo);
            db.Commit();
        }

        public List<TipoServico> Search(string busca)
        {
            busca = $"{busca}";
            return db.Context.TipoServico.ToList().
                Where(t => t.Nome.ToLower().Contains(busca.ToLower())).ToList();
        }
    }
}
