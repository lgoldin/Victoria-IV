namespace Victoria.ViewModelWPF
{
    public class TestViewModel : TestViewModelBase
    {
        public override void RunSimulation()
        {
            BackgroundWorker.RunInBackGround(delegate { this.simulator.Run(); });
        }
    }
}
