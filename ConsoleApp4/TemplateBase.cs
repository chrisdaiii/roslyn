using Microsoft.AspNetCore.Components.Rendering;

namespace ConsoleApp3
{
    public class TemplateBase
    {
        public void Write(object obj = null)
        {
            Console.WriteLine(obj);
        }

        public void WriteLiteral(string literal = null)
        {
            Console.WriteLine(literal);
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void BuildRenderTree(RenderTreeBuilder builder)
        {

        }
    }
}
