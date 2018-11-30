using CutelariaRetiro.DAL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.BLL
{
    public class MaterialBLL 
    {
        private MaterialDAL db = null;

        public MaterialBLL()
        {
            db = new MaterialDAL();
        }

        public void Save(Material mat)
        {
            if (db.Find(mat.Id) == null)
                db.Save(mat);
            else
                db.Update(mat);

            db.Commit();
        }

        public Material Find(int id)
        {
            return db.Find(id);
        }

        public void Remove(int id)
        {
            var mat = Find(id);
            db.Remove(mat);
            db.Commit();
        }

        public List<MovimentoEstoque> GetMovimentosEstoque(int materialId,
            DateTime dataInicio, DateTime dataFim)
        {
            return db.GetMovimentosEstoque(materialId, dataInicio, dataFim);
        }

        public void SetInativo(int id, bool inativo)
        {
            var mat = Find(id);
            mat.Inativo = inativo;
            db.Update(mat);
            db.Commit();
        }

        public List<Material> Search(string busca)
        {
            return db.Search(busca);
        }
        
        internal bool ExisteMovimentacaoMaterial(int id)
        {
            return db.ExisteMovimentacaoMaterial(id);
        }

        internal void EntradaMaterial(EntradaMaterial entrada)
        {
            if (entrada.Quantidade <= 0)
                throw new Exception("A quantidade da entrada deve ser superior a 0 (zero)");

            db.Context.EntradaMaterial.Add(entrada);
            db.Commit();

            Material mat = Find(entrada.MaterialId);
            mat.Estoque += entrada.Quantidade;
            Save(mat);
        }

        internal void SetContext(CutelariaRetiroEntities context)
        {
            db.Context = context;
        }

        internal void CancelaEntrada(string lote)
        {
            db.Context.Database.ExecuteSqlCommand($"delete from EntradaMaterial where Lote = '{lote}'");
            db.Commit();
        }
    }
}
