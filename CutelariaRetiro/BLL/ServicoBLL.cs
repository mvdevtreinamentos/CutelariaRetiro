using CutelariaRetiro.DAL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.BLL
{
    public class ServicoBLL
    {
        private ServicoDAL db = null;

        public ServicoBLL()
        {
            db = new ServicoDAL();
        }

        internal int GetMediaDiariaMaterial(int materialId,
            DateTime dataInico, DateTime dataFim)
        {
            return db.GetMediaDiariaMaterial(materialId,
                dataInico, dataFim);
        }

        public void Save(Servico servico)
        {
            if (db.Find(servico.Id) == null)
                db.Save(servico);
            else
            {
                db.Update(servico);
            }

            db.Commit();
        }

        public void AdicionaItem(ItemServico item)
        {
            db.Context.ItemServico.Add(item);
            db.Commit();
        }

        public void RemoveItem(int itemId)
        {
            var item = db.Context.ItemServico.Find(itemId);
            db.Context.ItemServico.Remove(item);
            db.Commit();
        }

        internal List<Servico> Search(string text)
        {
            return db.Search(text);
        }

        internal bool ExisteNumeroBloco(int numBloco)
        {
            var count = db.Where(e => e.Id == numBloco).Count();
            return (count > 0);
        }

        public Servico Find(int id)
        {
            return db.Find(id);
        }

        public void Remove(int id)
        {
            var s = Find(id);


            var q = (from mc in db.Context.MovimentoCaixa.AsNoTracking()
                     where mc.ServicoId == s.Id
                     select mc).ToList();

            q.ForEach(mc => db.Context.MovimentoCaixa.Remove(mc));

            s.ItemServico.ToList().ForEach(i => RemoveItem(i.Id));
            s.MaterialServico.ToList().ForEach(m => db.Context.MaterialServico.Remove(m));
            db.Commit();

            db.Remove(s);
            db.Commit();
        }

        public void RemoveMaterial(int materialId)
        {
            var mat = db.Context.MaterialServico.Find(materialId);
            db.Context.MaterialServico.Remove(mat);
            db.Commit();
        }

        internal void AdicionaMaterial(MaterialServico material)
        {
            db.Context.MaterialServico.Add(material);
            db.Commit();
        }
    }
}
