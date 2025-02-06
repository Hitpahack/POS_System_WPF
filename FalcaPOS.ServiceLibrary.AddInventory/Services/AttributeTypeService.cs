using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class AttributeTypeService : IAttributeTypeService
    {
        private readonly IEventAggregator _eventAggregator;

        public AttributeTypeService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public async Task<AttributeType> CreateAttribute(AttributeType attributeType)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<AttributeType, Response<AttributeType>>(AppConstants.ATTRIBUTETYPE_CREATE, attributeType, AppConstants.ACCESS_TOKEN);

                if (_result != null && _result.IsSuccess)
                {
                    return _result.Data;
                }
                else if (_result != null && !_result.IsSuccess)
                {
                    ShowAlert(_result.Error, NotificationType.Error);
                }
                return default(AttributeType);

            }
            catch (Exception _ex)
            {
                ShowAlert(_ex?.Message, NotificationType.Error);

            }
            return null;
        }

        public async Task<IEnumerable<Entites.Attributes.AttributeType>> GetAttributeTypes(string querry = null)
        {
            try
            {
                string _url = AppConstants.ATTRIBUTETYPE_GETALL;

                if (!string.IsNullOrEmpty(querry))
                {
                    _url = string.Format("{0}?{1}", _url, querry);
                }

                var _result = await HttpHelper.GetAsync<IEnumerable<Entites.Attributes.AttributeType>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {
                ShowAlert(_ex?.Message, NotificationType.Error);
            }
            return null;

        }

        private void ShowAlert(string msg, NotificationType notification)
        {

            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = msg,
                MessageType = notification
            });
        }
    }
}
