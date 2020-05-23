namespace Polo.Commands
{
    public class RawCommand : ICommand
    {
        public string Name => "raw";

        public string ShortName => "r";

        public string Description => "Creates RAW sub-folder in the current folder and moves all RAW files to this sub-folder.";

        public void Action()
        {
            // TODO LA
            // Get current folder

            // Create RAW folder if it does not exist

            // Find all RAW files

            // Move all RAW files to the RAW folder

            // Show list of moved RAW files
        }
    }
}
