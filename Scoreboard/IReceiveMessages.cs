using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Scoreboard;

namespace Scoreboard
{
    public interface IReceiveMessages
    {
        void ReceiveMessage(string value);
        public string ElementName { get; }
    }
}