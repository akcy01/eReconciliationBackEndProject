using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        private readonly ICompanyDal _companyDal;
        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

        [CacheRemoveAspect("ICompanyService.Get")]
        [ValidationAspect(typeof(CompanyValidator))]
        public IResult Add(Company company)
        {
            _companyDal.Add(company);
            return new SuccessResult(Messages.AddedCompany);
        }

        [CacheRemoveAspect("ICompanyService.Get")]
        [ValidationAspect(typeof(CompanyValidator))]
        [TransactionScopeAspect]
        public IResult AddCompanyAndUserCompany(CompanyDto companyDto)
        {
            _companyDal.Add(companyDto.Company);
            _companyDal.UserCompanyAdd(companyDto.UserId, companyDto.Company.Id);
            return new SuccessResult(Messages.AddedCompany);
        }

        public IResult CompanyExist(Company company)
        {
           var result = _companyDal.Get(c => c.Name == company.Name && c.TaxDepartment == company.TaxDepartment && c.TaxIdNumber == company.TaxIdNumber && c.IdentityNumber == company.IdentityNumber);
            
            if(result != null) // Bu kayıt daha önce varsa
            {
                return new ErrorResult(Messages.CompanyAlreadyExists);
            }
            return new SuccessResult();
        }

        [CacheAspect(60)]
        public IDataResult<Company> GetById(int id)
        {
            return new SuccessDataResult<Company>(_companyDal.Get(p => p.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyDal.GetCompany(userId));
        }

        [CacheAspect(60)]
        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetAll(),"Listeleme işlemi başarılı.");
        }

        [CacheRemoveAspect("ICompanyService.Get")]
        public IResult Update(Company company)
        {
            _companyDal.Update(company);
            return new SuccessResult(Messages.UpdatedCompany);
        }

        [CacheRemoveAspect("ICompanyService.Get")]
        public IResult UserCompanyAdd(int userId, int companyId)
        { 
            _companyDal.UserCompanyAdd(userId, companyId);
            return new SuccessResult();
        }
    }
}
