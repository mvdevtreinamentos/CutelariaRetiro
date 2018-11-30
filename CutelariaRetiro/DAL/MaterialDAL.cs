using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.DAL
{
    public class MaterialDAL : RepositoryImpl<Material>
    {
        internal List<Material> Search(string busca)
        {
            busca = busca.ToLower();
            var q = (from mat in Context.Material.ToList()
                     where mat.Descricao.ToLower().Contains(busca) ||
                     mat.Referencia.ToLower().Contains(busca)
                     select mat);

            return q.ToList();
        }

        internal bool ExisteMovimentacaoMaterial(int id)
        {
            bool existeEntradas = (from mat in Context.EntradaMaterial
                                  where mat.MaterialId == id
                                  select mat.Id).Count() > 0;

            bool existeSaidas = (from mat in Context.MaterialServico
                                  where mat.MaterialId == id
                                  select mat.Id).Count() > 0;

            return (existeEntradas || existeSaidas);
        }

        internal List<MovimentoEstoque> GetMovimentosEstoque(int materialId,
            DateTime dataInicio, DateTime dataFim)
        {
            List<MovimentoEstoque> result = new List<MovimentoEstoque>();

            var entradas = (from entrada in Context.EntradaMaterial.AsNoTracking()
                            where entrada.MaterialId == materialId &&
                            entrada.Data >= dataInicio &&
                            entrada.Data <= dataFim
                            select entrada).ToList();

            entradas.ForEach(e => result.Add(new MovimentoEstoque(e)));

            var materiaisServicos = (from materialServico in Context.MaterialServico.AsNoTracking()
                            where materialServico.MaterialId == materialId &&
                            materialServico.Servico.Data >= dataInicio &&
                            materialServico.Servico.Data <= dataFim
                            select materialServico).ToList();

            materiaisServicos.ForEach(m => result.Add(new MovimentoEstoque(m)));

            return result.OrderBy(m => m.Data).ToList();
        }
    }
}
