using CutelariaRetiro.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.Model
{
    public static class ServicoExt
    {
        public static decimal GetTotalGeral(this Servico serv)
        {
            var totalServicos = serv.ItemServico.ToList().Sum(i => i.Total);
            var totalMateriais = serv.MaterialServico.ToList().Sum(i => i.Total);
            return (totalServicos + totalMateriais);
        }

        public static decimal GetTotalServicos(this Servico serv)
        {
            var totalServicos = serv.ItemServico.ToList().Sum(i => i.Total);
            return totalServicos;
        }

        public static decimal GetValorAdiantado(this Servico serv)
        {
            decimal valorPago = new MovimentoCaixaBLL().GetValorPagoServico(serv.Id);
            return valorPago;
        }

        public static decimal GetValorRestantePagar(this Servico serv)
        {
            decimal valorPago = new MovimentoCaixaBLL().GetValorPagoServico(serv.Id);
            decimal falta = (serv.GetTotalGeral() - valorPago);
            return falta;
        }

        public static decimal GetTotalMateriais(this Servico serv)
        {
            var totalMateriais = serv.MaterialServico.ToList().Sum(i => i.Total);
            return totalMateriais;
        }
    }
}
