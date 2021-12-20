using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.FileServices;

public interface IFileReaderService
{
    public Task<Dictionary<string, string>> ReadVanillaSettingsAsync(string folderPath);
}