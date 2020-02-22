using GUI.Types.Renderer;
using SteamDatabase.ValvePak;
using ValveResourceFormat;

namespace GUI.Utils
{
    public class VrfGuiContext
    {
        public string FileName { get; }

        public Package CurrentPackage { get; }

        public MaterialLoader MaterialLoader { get; }

        public ShaderLoader ShaderLoader { get; }

        private readonly FileLoader FileLoader;

        public VrfGuiContext(string fileName, Package package)
        {
            FileName = fileName;
            CurrentPackage = package;
            MaterialLoader = new MaterialLoader(this);
            ShaderLoader = new ShaderLoader();
            FileLoader = new FileLoader();
        }

        public Resource LoadFileByAnyMeansNecessary(string file) =>
            FileLoader.LoadFileByAnyMeansNecessary(file, this);

        public void ClearCache() => FileLoader.ClearCache();
    }
}