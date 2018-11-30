using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.Model
{
    public static class CaixaExt
    {
        public static decimal GetSaldoInicial(this Caixa cx)
        {
            var mov = cx.MovimentoCaixa.Where(mc => mc.Tipo == (int)TipoMovCaixa.Abertura).FirstOrDefault();
            if (mov != null)
                return mov.Valor;
            return 0;
        }

        public static decimal GetTotalRetirada(this Caixa cx)
        {
            var q = (from mc in cx.MovimentoCaixa.ToList()
                     where
                     mc.Tipo == (int)TipoMovCaixa.Saida
                     select mc.Valor).Sum();

            return q;
        }

        public static decimal GetTotalFormaPg(this Caixa cx, FormaPagamento fpg)
        {
            var q = (from mc in cx.MovimentoCaixa.ToList()
                     where
                     mc.FormaPagamento == (int)fpg &&
                     mc.Tipo == (int)TipoMovCaixa.Entrada
                     select mc.Valor).Sum();

            return q;
        }
    }
}
