using AmnesiaPreprocessor.Core.Entities;

namespace AmnesiaPreprocessor.Core.Directives
{
    public interface IDirective
    {
        void Execute(CustomStory customStory);
    }
}
