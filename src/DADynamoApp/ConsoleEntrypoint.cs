namespace DADynamoApp
{
    public static class ConsoleEntrypoint
    {
        public static void DoStuff()
        {
            Console.WriteLine("Hello, World!");

            var xx = new DAEntrypoint();
            xx.OnStartup(null);

            var handler = new DADynamoApp.DynamoADApp();
            handler.HandleDesignAutomationReadyEvent(null, new DesignAutomationFramework.DesignAutomationReadyEventArgs(null, ""));
        }
    }
}
