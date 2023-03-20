using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICurrencyAccountService
    {
        IResult Add(CurrencyAccount currencyAccount);
        IResult AddToExcel(string fileName);
        IResult Update(CurrencyAccount currencyAccount);
        IResult Delete(CurrencyAccount currencyAccount);
        IDataResult<CurrencyAccount> Get(int id);
        IDataResult<List<CurrencyAccount>> GetList(int companyId);
    }
}
