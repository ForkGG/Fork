using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectAvery.Logic.Services.FileServices;

public interface IFileWriterService
{
    public Task WriteEula(string folderPath);
    public Task WriteServerSettings(string folderPath, IDictionary<string, string> settings);
}