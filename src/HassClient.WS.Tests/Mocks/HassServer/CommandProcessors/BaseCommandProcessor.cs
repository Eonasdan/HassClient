using System.IO;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer.CommandProcessors
{
    public abstract class BaseCommandProcessor
    {
        public abstract bool CanProcess(BaseIdentifiableMessage receivedCommand);

        public abstract BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand);

        protected BaseIdentifiableMessage CreateResultMessageWithError(ErrorInfo errorInfo) => new ResultMessage { Error = errorInfo };

        protected BaseIdentifiableMessage CreateResultMessageWithResult(JRaw result) => new ResultMessage { Success = true, Result = result };

        protected Stream GetResourceStream(string filename)
        {
            var assembly = typeof(BaseCommandProcessor).Assembly;
            var assemblyNamespace = Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);
            return assembly.GetManifestResourceStream($"{assemblyNamespace}.Mocks.Data.{filename}");
        }
    }
}
