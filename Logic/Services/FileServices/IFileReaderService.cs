using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.FileServices;

public interface IFileReaderService
{
    public Task<Dictionary<string, string>> ReadVanillaSettingsAsync(string folderPath);
    public Task<List<string>> ReadWhiteListTxt(string serverPath);
    public Task<List<string>> ReadWhiteListJson(string serverPath);
    public Task<List<string>> ReadOpListTxt(string serverPath);
    public Task<List<string>> ReadOpListJson(string serverPath);
    public Task<List<string>> ReadBanListTxt(string serverPath);
    public Task<List<string>> ReadBanListJson(string serverPath);
}