using Prism.Mvvm;
using Prism.Navigation;

namespace FalcaPOS.Common.ViewModel
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
