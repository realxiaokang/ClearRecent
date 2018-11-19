using Microsoft.VisualStudio.Shell;

namespace ClearRecent.Commands
{
    class ClearNonFavoriteProjects : Command
    {
        internal ClearNonFavoriteProjects(Package package) :
            base(
                package,
                0x0104, null)
        { }

        protected override bool Enabled() =>
           startPageRecents.NonFavoriteProjectsFound;

        protected override void Execute()
        {
            startPageRecents.ClearNonFavoriteProjects();
        }
    }
}
