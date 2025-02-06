using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FalcaPOS.Common.Validations
{
    public abstract class ValidationBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsList;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errorsList.Any();

        protected void ClearErrors() { _errorsList.Clear(); IsValid = false; }

        public ValidationBase()
        {
            _errorsList = new Dictionary<string, List<string>>();
        }

        private bool _isValid = false;
        protected bool IsValid
        {
            get { return _isValid; }
            private set { SetProperty(ref _isValid, value); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                return _errorsList.ContainsKey(propertyName) ? _errorsList[propertyName] : null;
            }
            else
            {
                return _errorsList.SelectMany(err => err.Value.ToList());
            }
        }


        public void OnErrorChanged(string propName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var _validatationContext = new ValidationContext(this, null, null);

            _validatationContext.MemberName = propertyName;

            var _validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            Validator.TryValidateProperty(value, _validatationContext, _validationResults);

            if (_errorsList.ContainsKey(propertyName))
                _errorsList.Remove(propertyName);

            OnErrorChanged(propertyName);
            HandleValidationResult(_validationResults);


        }
        public void Validate()
        {

            var validationContext = new ValidationContext(this, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults, true);

            if (validationResults.Any())
            {
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }



        }

        private void HandleValidationResult(List<ValidationResult> validationResults)
        {
            var _errors = from res in validationResults
                          from names in res.MemberNames
                          group res by names into grp
                          select grp;



            foreach (var _error in _errors)
            {
                var _messages = _error.Select(x => x.ErrorMessage).ToList();
                _errorsList.Add(_error.Key, _messages);
                OnErrorChanged(_error.Key);

            }

            Validate();
        }
    }
}
