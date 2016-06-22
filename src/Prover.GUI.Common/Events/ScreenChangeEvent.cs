using Caliburn.Micro.ReactiveUI;

namespace Prover.Client.Events
{
    public class ScreenChangeEvent
    {
        public ReactiveScreen ViewModel;

        public ScreenChangeEvent(ReactiveScreen viewModel)
        {
            ViewModel = viewModel;
        }
    }
}