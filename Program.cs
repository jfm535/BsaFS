using System;
using Mutagen.Bethesda;
using NC.DokanFS;

namespace BsaFS
{
    class Program
    {
        static void Main(string[] args)
        {
            var testing = Mutagen.Bethesda.Archives.Archive.CreateReader(GameRelease.SkyrimSE, args[0]);
            var crossdrive = new FSBackendDisk(testing);
            var dokan = new DokanFrontend(crossdrive, "CrossDrive");
            dokan.Mount(@"H:\");

            Console.WriteLine("Press ENTER to Exit.");
            Console.Read();

            dokan.Unmount();
        }
    }
}