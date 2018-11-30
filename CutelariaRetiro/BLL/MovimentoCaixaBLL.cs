using CutelariaRetiro.DAL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.BLL
{
    public class MovimentoCaixaBLL
    {
        private MovimentoCaixaDAL db;

        public MovimentoCaixaBLL()
        {
            db = new MovimentoCaixaDAL();
        }

        public void Save(MovimentoCaixa mov)
        {
            if (db.Find(mov.Id) == null)
                db.Save(mov);
            else
                db.Update(mov);

            db.Commit();
        }

        public MovimentoCaixa Find(int id)
        {
            return db.Find(id);
        }

        public void Remove(int id)
        {
            var mov = Find(id);
            db.Remove(mov);
            db.Commit();
        }

        internal decimal GetValorPagoServico(int servicoId)
        {
            return db.Where(c => c.ServicoId == servicoId)
                .Sum(c => (decimal?)c.Valor) ?? 0;
        }
    }
}
