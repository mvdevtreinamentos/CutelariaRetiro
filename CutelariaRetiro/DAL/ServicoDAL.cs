using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.DAL
{
    public class ServicoDAL : RepositoryImpl<Servico>
    {
        internal List<Servico> Search(string text)
        {
            int id = 0;
            int.TryParse(text, out id);
            text = text.ToUpper();
            var q = (from serv in Context.Servico.ToList()
                     where serv.Cliente.Contains(text) ||
                     serv.Telefone.Contains(text) ||
                     serv.Id == id ||
                     serv.Obs.Contains(text) ||
                     serv.ItemServico.Any(i => i.TipoServico.Nome.Contains(text)) ||
                     serv.ItemServico.Any(i => i.Observacoes.Contains(text))

                     select serv);

            return q.ToList();
        }

        public int GetMediaDiariaMaterial(int materialId, DateTime dataInicio,
            DateTime dataFim)
        {
            int dias = (from serv in Context.Servico.AsNoTracking().ToList()
                        where serv.MaterialServico.Any(m => m.MaterialId == materialId) &&
                        serv.Data >= dataInicio &&
                        serv.Data <= dataFim
                        select serv.Data.Day).Distinct().ToList().Count;

            var totalQuantVendida = (from mat in Context.MaterialServico.AsNoTracking()
                                     where mat.MaterialId == materialId &&
                                     mat.Servico.Data >= dataInicio &&
                                     mat.Servico.Data <= dataFim
                                     select (int?)mat.Quantidade).Sum()??0;

            int result = 0;
            try
            {
                result= (totalQuantVendida / dias);
            }
            catch { }
            return result;
        }
    }
}
