using AutoMapper;
using FalcaPOS.Entites.Reports;
using FalcaPOS.Reports.Model;
using FalcaPOS.Reports.View;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Reports
{
    public class ReportsModule:IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) {
            var _region = containerProvider.Resolve<IRegionManager>();
            _region.RegisterViewWithRegion("Report", typeof(Report));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {

        }
    }
}
