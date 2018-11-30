using CutelariaRetiro.DAL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.BLL
{
    public class CaixaBLL
    {
        private CaixaDAL db;

        public CaixaBLL()
        {
            db = new CaixaDAL();
        }

        public void Save(Caixa caixa)
        {
            if (db.Find(caixa.Id) == null)
                db.Save(caixa);
            else
                db.Update(caixa);

            db.Commit();
        }

        public Caixa Find(int id)
        {
            return db.Find(id);
        }

        public bool CaixaAberto()
        {
            return (GetCaixaAberto() != null);
        }

        public Caixa GetCaixaAberto()
        {
            try
            {
                return db.Where(c => c.Aberto).ToList()
                    .OrderBy(c => c.DataAbertura).LastOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public void Remove(int id)
        {
            var cx = Find(id);
            db.Remove(cx);
            db.Commit();
        }

        internal void AbreCaixa(decimal saldoInicial, string obs)
        {
            Caixa caixa = new Caixa();
            caixa.DataAbertura = DateTime.Now;
            caixa.Aberto = true;
            caixa.Obs = obs;

            db.Context.Caixa.Add(caixa);
            db.Commit();

            MovimentoCaixa mc = new MovimentoCaixa();
            mc.CaixaId = caixa.Id;
            mc.Tipo = (int)TipoMovCaixa.Abertura;
            mc.Valor = saldoInicial;
            mc.Obs = obs;

            db.Context.MovimentoCaixa.Add(mc);
            db.Commit();
        }
    }
}
