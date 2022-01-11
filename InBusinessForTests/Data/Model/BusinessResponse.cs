using System.Collections.Generic;
using System.Linq;

namespace InBusinessForTests.Data.Model
{
    public class BusinessResponse<T>
    {
        public static BusinessResponse<T> Success => new BusinessResponse<T> { _succeeded = true };

        public static BusinessResponse<T> SuccessWithEntity(T entity)
        {
            return new BusinessResponse<T> { _succeeded = true, _entity = entity };
        }

        public static BusinessResponse<T> Failed(params BusinessError[] errors)
        {
            var result =  new BusinessResponse<T>
            {
                _succeeded = false,
            };
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }

        private bool _succeeded;
        private List<BusinessError> _errors = new List<BusinessError>();
        private T _entity;

        public bool Succeeded => _succeeded;
        public IEnumerable<BusinessError> Errors => _errors;
        public T Entity => _entity;
        
        public override string ToString()
        {
            return Succeeded ? 
                "Succeeded" : 
                string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
    
    public class BusinessError
    {
        public string Code { get; set; }

        public string Description { get; set; }
    }
}