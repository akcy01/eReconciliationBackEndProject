﻿using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliationDal, ICurrencyAccountService currencyAccountService)
        {
            _accountReconciliationDal = accountReconciliationDal;
            _currencyAccountService = currencyAccountService;
        }

        public IResult Add(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Add(accountReconciliation);
            return new SuccessResult(Messages.AddedAccountReconciliation);
        }

        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int companyId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(0);
                        if (code != "Cari Kodu")
                        {
                            if (code != null)
                            {
                                string startingDate = Convert.ToDateTime(reader.GetValue(1)).ToString("dd/MM/yyyy");
                                string endingDate = Convert.ToDateTime(reader.GetValue(1)).ToString("dd/MM/yyyy");
                                string currencyId = reader.GetValue(3).ToString();
                                string debit = reader.GetValue(4).ToString();
                                string credit = reader.GetValue(5).ToString();
                                int CurrencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;

                                AccountReconciliation accountReconciliation = new AccountReconciliation()
                                {
                                    CompanyId = companyId,
                                    CurrencyAccountId = CurrencyAccountId,
                                    CurrencyCredit = Convert.ToDecimal(credit),
                                    CurrencyDebit = Convert.ToDecimal(debit),
                                    CurrencyId = Convert.ToInt16(currencyId),
                                    StartingDate = Convert.ToDateTime(startingDate),
                                    EndingDate = Convert.ToDateTime(endingDate),
                                };
                                _accountReconciliationDal.Add(accountReconciliation);
                            }
                        }
                    }
                }
            }
            return new SuccessResult(Messages.AddedAccountReconciliation);
        }

        public IResult Delete(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Delete(accountReconciliation);
            return new SuccessResult(Messages.DeletedAccountReconciliation);
        }

        public IDataResult<AccountReconciliation> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(p => p.Id == id));
        }

        public IDataResult<List<AccountReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliationDal.GetAll(p => p.CompanyId == companyId));
        }

        public IResult Update(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Update(accountReconciliation);
            return new SuccessResult(Messages.UpdatedAccountReconciliation);
        }
    }
}
