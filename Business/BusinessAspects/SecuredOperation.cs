using Castle.DynamicProxy;
using Core.Extensions;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessAspects
{
    public class SecuredOperation : MethodInterception
    {
        private string[] _roles; //Bana rolleri ver
        private IHttpContextAccessor _httpContextAccessor;
        public SecuredOperation(string roles)
        {
            _roles = roles.Split(","); //Rollerimi liste olarak getir ve virgülle ayır
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void OnBefore(IInvocation invocation)//Uygulama çalışmadan önce security aspecti çalıştırıp bu kullanıcının yetkisi olup olmadığını kontrol eder.
        {
            var roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();
            foreach (var role in _roles)
            {
                if(roleClaims.Contains(role))
                {
                    return;
                }
            }
            throw new Exception("İşlem yapmaya yetkiniz yok.");
        }
    }
}
